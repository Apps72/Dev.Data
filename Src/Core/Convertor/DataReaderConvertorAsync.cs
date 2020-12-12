using Apps72.Dev.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Apps72.Dev.Data.Convertor
{
    internal static partial class DataReaderConvertor
    {
        internal static async Task<ColumnsAndRows<T>> ToTypeAsync<T>(DbDataReader reader)
        {
            var hasRow = await reader.ReadAsync();

            // No data
            if (!hasRow)
            {
                return new ColumnsAndRows<T>
                {
                    Columns = new DataColumn[0],
                    Rows = new T[0]
                };
            }

            // Class Properties
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.CanWrite);

            // DataTable Columns 
            var names = Enumerable.Range(0, reader.FieldCount)
                                  .Select(i => reader.GetName(i))
                                  .ToArray();

            // Columns existing in properties
            var columns = new Dictionary<DataColumn, PropertyInfo>();
            for (int i = 0; i < names.Length; i++)
            {
                var property = properties.GetFirstOrDefaultWithAttributeOrName(names[i]);

                if (property != null)
                {
                    var column = new DataColumn
                                     (
                                        ordinal: i,
                                        columnName: names[i],
                                        sqlType: reader.GetDataTypeName(i),
                                        dataType: reader.GetFieldType(i),
                                        isNullable: await reader.IsDBNullAsync(i)
                                     );

                    columns.Add(column, property);
                }
            }

            // Convert all rows
            var rows = new List<T>();
            do
            {
                var newItem = Activator.CreateInstance<T>();
                foreach (var item in columns)
                {
                    DataColumn column = item.Key;
                    PropertyInfo property = item.Value;

                    object value = reader.GetValue(column.Ordinal);
                    property.SetValue(newItem, value == DBNull.Value ? null : value, null);
                }
                rows.Add(newItem);
            } while (await reader.ReadAsync());

            // Return
            return new ColumnsAndRows<T>()
            {
                Columns = columns.Keys,
                Rows = rows
            };
        }

        internal static async Task<IEnumerable<T>> ToAnonymousAsync<T>(DbDataReader reader)
        {
            bool hasRow = await reader.ReadAsync();

            // No data
            if (!hasRow)
            {
                return new T[0];
            }

            // Read and convert all rows
            var fieldCount = reader.FieldCount;
            var data = new object[fieldCount];
            var rows = new List<T>();

            try
            {
                do
                {
                    reader.GetValues(data);
                    RemoveDBNullValues(data, fieldCount);
                    var row = (T)Activator.CreateInstance(typeof(T), data);
                    rows.Add(row);
                } while (await reader.ReadAsync());
            }
            catch (MissingMethodException ex)
            {
                throw new MissingMethodException("Properties of your anonymous class must be in the same type and same order of your SQL Query.", ex);
            }

            // Return
            return rows;
        }

        internal static async Task<IEnumerable<T>> ToPrimitivesAsync<T>(DbDataReader reader)
        {
            bool hasRow = await reader.ReadAsync();

            // No data
            if (!hasRow)
            {
                return new T[0];
            }

            // Read and convert all rows
            var rows = new List<T>();

            do
            {
                var data = reader.GetValue(0);
                data = data == DBNull.Value ? null : data;
                rows.Add((T)data);
            } while (await reader.ReadAsync());

            // Return
            return rows;
        }

        internal static async Task<IEnumerable<T>> ToDynamicAsync<T>(DbDataReader reader)
        {
            bool hasRow = await reader.ReadAsync();

            // No data
            if (!hasRow)
            {
                return new T[0];
            }

            // DataTable Columns 
            var columns = Enumerable.Range(0, reader.FieldCount)
                                    .ToDictionary(i => reader.GetName(i),
                                                  i => reader.IsDBNull(i)
                                                          ? typeof(Nullable<>).MakeGenericType(reader.GetFieldType(i))
                                                          : reader.GetFieldType(i));

            // Get Type
            var type = DynamicConvertor.GetDynamicType(DynamicConvertor.DYNAMIC_CLASS_NAME, columns);

            // Class Properties
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Convert all rows
            var rows = new List<object>();
            do
            {
                var newItem = Activator.CreateInstance(type);
                for (int i = 0; i < columns.Count; i++)
                {
                    PropertyInfo property = properties[i];
                    object value = reader.GetValue(i);
                    property.SetValue(newItem, value == DBNull.Value ? null : value, null);
                }
                rows.Add(newItem);
            } while (await reader.ReadAsync());

            // Return
            return rows.Cast<T>();
        }

        internal static async Task<DataTable> ToDataTableAsync(DbDataReader reader)
        {
            int fieldCount = reader.FieldCount;
            var table = new DataTable();

            bool hasRow = await reader.ReadAsync();

            // No data
            if (!hasRow)
            {
                return new DataTable();
            }

            // DataTable Columns 
            table.Columns = Enumerable.Range(0, fieldCount)
                                      .Select(i => new DataColumn
                                          (
                                             ordinal: i,
                                             columnName: reader.GetName(i),
                                             sqlType: reader.GetDataTypeName(i),
                                             dataType: reader.GetFieldType(i),
                                             isNullable: reader.IsDBNull(i)
                                          ))
                                      .ToArray();
            // DataTable Rows
            var data = new object[fieldCount];
            var rows = new List<DataRow>();
            do
            {
                reader.GetValues(data);
                rows.Add(new DataRow(table, data));

            } while (await reader.ReadAsync());
            table.Rows = rows.ToArray();

            // Return
            return table;
        }

        internal static async  Task<Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>>> ToMultipleTypesAsync<T, U, V, W, X>(DbDataReader dr, bool forAnonymousTypes = false)
        {
            // Dataset #0 for type T
            var dataset0 = await ToPrimitiveOrTypeOrDynamicAsync<T>(dr);
            var hasNextResult0 = await dr.NextResultAsync();

            // Dataset #1 for type U
            var dataset1 = hasNextResult0 ? await ToPrimitiveOrTypeOrDynamicAsync<U>(dr) : null;
            var hasNextResult1 = hasNextResult0 ? await dr.NextResultAsync() : false;

            // Dataset #2 for type V
            var dataset2 = hasNextResult1 ? await ToPrimitiveOrTypeOrDynamicAsync<V>(dr) : null;
            var hasNextResult2 = hasNextResult1 ? await dr.NextResultAsync() : false;

            // Dataset #3 for type W
            var dataset3 = hasNextResult2 ? await ToPrimitiveOrTypeOrDynamicAsync<W>(dr) : null;
            var hasNextResult3 = hasNextResult2 ? await dr.NextResultAsync() : false;

            // Dataset #4 for type X
            var dataset4 = hasNextResult3 ? await ToPrimitiveOrTypeOrDynamicAsync<X>(dr) : null;
            var hasNextResult4 = hasNextResult3 ? await dr.NextResultAsync() : false;

            // Return
            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>>
                (
                    dataset0,
                    dataset1,
                    dataset2,
                    dataset3,
                    dataset4
                );

            // Depending of type of T, gets the Primitive, Dynamic or Typed data.
            async Task<IEnumerable<MyType>> ToPrimitiveOrTypeOrDynamicAsync<MyType>(DbDataReader datareader)
            {
                // Primitive type: Executable<string>()
                if (TypeExtension.IsPrimitive(typeof(MyType)))
                    return await DataReaderConvertor.ToPrimitivesAsync<MyType>(datareader);

                // Dynamic type: Executable<dynamic>()
                else if (DynamicConvertor.IsDynamic(typeof(MyType)))
                    return await DataReaderConvertor.ToDynamicAsync<MyType>(datareader);

                // Anonymous type: Executable(new { Name = "" })
                else if (forAnonymousTypes == true)
                    return await DataReaderConvertor.ToAnonymousAsync<MyType>(datareader);

                // Object type: Executable<Employee>()
                else
                    return (await DataReaderConvertor.ToTypeAsync<MyType>(datareader)).Rows;
            }
        }

        internal static System.Data.DataSet ToSystemDataSetAsync(DbDataReader dr)
        {
            var dataset = new System.Data.DataSet();
            bool hasNextResult = false;

            do
            {
                var tableName = GetTableName(dr);
                var table = new System.Data.DataTable(tableName);
                table.Load(dr);
                dataset.Tables.Add(table);

                hasNextResult = !dr.IsClosed;
            } while (hasNextResult);

            return dataset;

            // Get the Table Name
            string GetTableName(DbDataReader dataReader)
            {
                var schema = dataReader.GetSchemaTable();
                if (schema.Rows.Count > 0 && schema.Columns.Contains("BaseTableName"))
                {
                    string tableName = Convert.ToString(schema.Rows[0]["BaseTableName"]);

                    // All columns must be set from the same table
                    foreach (System.Data.DataRow row in schema.Rows)
                    {
                        if (Convert.ToString(row["BaseTableName"]) != tableName)
                            return string.Empty;
                    }

                    return tableName;
                }
                return String.Empty;
            }
        }
    }
}

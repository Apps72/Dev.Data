using Apps72.Dev.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Convertor
{
    internal static class DataReaderConvertor
    {
        internal static ColumnsAndRows<T> ToType<T>(DbDataReader reader)
        {
            reader.Read();

            // Class Properties
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

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
                                        isNullable: reader.IsDBNull(i)
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
            } while (reader.Read());

            // Return
            return new ColumnsAndRows<T>()
            {
                Columns = columns.Keys,
                Rows = rows
            };
        }

        internal static IEnumerable<T> ToAnonymous<T>(DbDataReader reader)
        {
            reader.Read();

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
                } while (reader.Read());
            }
            catch (MissingMethodException ex)
            {
                throw new MissingMethodException("Properties of your anonymous class must be in the same type and same order of your SQL Query.", ex);
            }

            // Return
            return rows;
        }

        internal static IEnumerable<T> ToPrimitives<T>(DbDataReader reader)
        {
            reader.Read();

            // Read and convert all rows
            var rows = new List<T>();

            do
            {
                var data = reader.GetValue(0);
                data = data == DBNull.Value ? null : data;
                rows.Add((T)data);
            } while (reader.Read());

            // Return
            return rows;
        }

        internal static IEnumerable<T> ToDynamic<T>(DbDataReader reader)
        {
            reader.Read();

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
            } while (reader.Read());

            // Return
            return rows.Cast<T>();
        }

        internal static IEnumerable<T> ToTypeOrDynamic<T>(DbDataReader reader)
        {
            if (DynamicConvertor.IsDynamic(typeof(T)))
                return DataReaderConvertor.ToDynamic<T>(reader);
            else
                return DataReaderConvertor.ToType<T>(reader).Rows;
        }

        internal static DataTable ToDataTable(DbDataReader reader)
        {
            int fieldCount = reader.FieldCount;
            var table = new DataTable();

            reader.Read();

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

            } while (reader.Read());
            table.Rows = rows.ToArray();

            // Return
            return table;
        }

        private static void RemoveDBNullValues(object[] data, int fieldCount)
        {
            for (int i = 0; i < fieldCount; i++)
            {
                if (data[i] == DBNull.Value) data[i] = null;
            }
        }

        private static PropertyInfo GetFirstOrDefaultWithAttributeOrName(this PropertyInfo[] properties, string columnName)
        {
            return properties.FirstOrDefault(prop => String.Compare(Annotations.ColumnAttribute.GetColumnAttributeName(prop), columnName, StringComparison.InvariantCultureIgnoreCase) == 0 && prop.CanWrite)
                   ??
                   properties.FirstOrDefault(prop => String.Compare(prop.Name, columnName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }

    internal class ColumnsAndRows<T>
    {
        public IEnumerable<DataColumn> Columns { get; set; }
        public IEnumerable<T> Rows { get; set; }
    }
}

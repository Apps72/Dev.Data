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
                var property = properties.FirstOrDefault(x => String.Compare(x.Name, names[i], StringComparison.InvariantCultureIgnoreCase) == 0);
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

    }

    internal class ColumnsAndRows<T>
    {
        public IEnumerable<DataColumn> Columns { get; set; }
        public IEnumerable<T> Rows { get; set; }
    }
}

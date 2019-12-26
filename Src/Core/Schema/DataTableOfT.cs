using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Schema
{
    public class DataTable<T>
    {
        internal DataTable(DbDataReader reader,
                           Func<DbDataReader, ColumnsAndRows<T>> actionToGetRows)
        {
            var colProps = actionToGetRows.Invoke(reader);

            this.Columns = colProps.Columns;
            this.Rows = colProps.Rows;
        }

        public IEnumerable<DataColumn> Columns { get; private set; }

        public IEnumerable<T> Rows { get; private set; }

        internal Func<DbDataReader, ColumnsAndRows<T>> GetRows { get; set; }

        internal static ColumnsAndRows<T> ConvertReaderToType<T>(DbDataReader reader)
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
    }

    internal class ColumnsAndRows<T>
    {
        public IEnumerable<DataColumn> Columns { get; set; }
        public IEnumerable<T> Rows { get; set; }
    }
}

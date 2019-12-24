using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apps72.Dev.Data.Schema
{
    public class DataTable<T>
    {
        private IEnumerable<ColumnProperty> _columnProperties;

        public DataTable(DbDataReader reader)
        {
            reader.Read();
            _columnProperties = ColumnsIntersectProperties(reader);

            this.Columns = _columnProperties.Select(i => i.Column);
            this.Rows = GetRows(reader);
        }

        private IEnumerable<ColumnProperty> ColumnsIntersectProperties(IDataRecord record)
        {
            // Class Properties
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // DataTable Columns 
            var names = Enumerable.Range(0, record.FieldCount)
                                  .Select(i => record.GetName(i))
                                  .ToArray();

            // Columns existing in properties
            var columns = new List<ColumnProperty>();
            for (int i = 0; i < names.Length; i++)
            {
                var property = properties.FirstOrDefault(x => String.Compare(x.Name, names[i], StringComparison.InvariantCultureIgnoreCase) == 0);
                if (property != null)
                {
                    columns.Add(new ColumnProperty()
                    {
                        ColumnIndex = i,
                        Column = new DataColumn
                                     (
                                        ordinal: i,
                                        columnName: names[i],
                                        sqlType: record.GetDataTypeName(i),
                                        dataType: record.GetFieldType(i),
                                        isNullable: record.IsDBNull(i)
                                     ),
                        Property = property,
                    });
                }
            }

            return columns;
        }

        public IEnumerable<DataColumn> Columns { get; private set; }

        public IEnumerable<T> Rows { get; private set; }

        private IEnumerable<T> GetRows(DbDataReader reader)
        {
            var result = new List<T>();

            do
            {
                var newItem = Activator.CreateInstance<T>();
                foreach (var item in _columnProperties)
                {
                    object value = reader.GetValue(item.ColumnIndex);
                    item.Property.SetValue(newItem, value == DBNull.Value ? null : value, null);
                }
                //yield return newItem;
                result.Add(newItem);
            } while (reader.Read());

            return result;
        }

        class ColumnProperty
        {
            public int ColumnIndex { get; set; }
            public DataColumn Column { get; set; }
            public PropertyInfo Property { get; set; }
        }
    }
}

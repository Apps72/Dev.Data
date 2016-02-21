using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Internal
{
    /// <summary>
    /// Description of a Data Row
    /// </summary>
    internal class DataRow : IEnumerable
    {
        private object[] _rowValues = null;
        private DataTable _table = null;

        /// <summary>
        /// Initializes a new instance of a DataRow
        /// </summary>
        /// <param name="table"></param>
        /// <param name="values"></param>
        public DataRow(DataTable table, object values)
        {
            // Simple value type
            if (Convertor.TypeExtension.IsPrimitive(values.GetType()))
            {
                _rowValues = new object[] { values };
            }

            // Complex values type
            else
            {
                PropertyInfo[] properties = values.GetType().GetProperties();
                _rowValues = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    _rowValues[i] = properties[i].GetValue(values);
                }
                _table = table;
            }
        }

        /// <summary>
        /// Gets the data value for this column index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                return _rowValues[index];
            }
        }

        /// <summary>
        /// Gets the data value for this column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object this[string columnName]
        {
            get
            {
                int? index = _table.Columns.FirstOrDefault(c => c.ColumnName == columnName)?.Ordinal;
                if (index != null)
                    return _rowValues[index.Value];
                else
                    throw new ArgumentException($"The ColumnName '{columnName}' doesn't exist in this table.");
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _rowValues.GetEnumerator();
        }

        /// <summary>
        /// Creates a new instance of T type and sets all row values to the new T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ConvertTo<T>()
        {
            return this.ConvertTo<T>(default(T));
        }

        /// <summary>
        /// If item is null, creates a new instance of T type and sets all row values to the new T properties.
        /// If item is not null, sets all row values to item object properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public T ConvertTo<T>(T item)
        {
            Type type = typeof(T);

            // For anonymous type, creates a new instance and sets all data row values to this new object.
            if (Convertor.TypeExtension.IsAnonymousType(type))
            {
                object newItem = Activator.CreateInstance(type, _rowValues);
                return (T)newItem;
            }

            // For defined type, creates a new instance and fill all data row values to this new object.
            else
            {
                object newItem = null;

                // Creates or gets an instance of T
                if (EqualityComparer<T>.Default.Equals(item, default(T)))
                    newItem = Activator.CreateInstance(type, null);
                else
                    newItem = item;

                // List of all properties of T
                List<PropertyInfo> properties = new List<PropertyInfo>();
                properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

                // Check all columns values
                foreach (DataColumn column in _table.Columns)
                {
                    PropertyInfo property = null;

                    // Gets the first property with correct Column attribute name
                    if (property == null)
                    {
                        property = properties.FirstOrDefault(p => String.Compare(Annotations.ColumnAttribute.GetColumnAttributeName(p), column.ColumnName, true) == 0 && p.CanWrite);
                    }

                    // If not found, gets the first property with corrct name
                    if (property == null)
                    {
                        property = properties.FirstOrDefault(p => String.Compare(p.Name, column.ColumnName, true) == 0 && p.CanWrite);
                    }

                    if (property != null)
                    {
                        object value = this[column.ColumnName];
                        property.SetValue(newItem, value == System.DBNull.Value ? null : value, null);
                    }
                }

                return (T)newItem;
            }

        }

        /// <summary>
        /// Gets all values as an Array of objects
        /// </summary>
        public object[] ItemArray
        {
            get
            {
                return _rowValues;
            }
        }

    }
}

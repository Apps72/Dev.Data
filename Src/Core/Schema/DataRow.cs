using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Schema
{
    /// <summary>
    /// Represents a row of data in a DataTable
    /// </summary>
    public class DataRow : IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of a DataRow with an array os simple item
        /// </summary>
        /// <param name="table"></param>
        /// <param name="values"></param>
        internal DataRow(DataTable table, object[] values)
        {
            this.ItemArray = values.Select(i => i == DBNull.Value ? null : i).ToArray();
            this.Table = table;
        }

        /// <summary>
        /// Gets all values as an Array of objects
        /// </summary>
        public object[] ItemArray { get; }

        /// <summary>
        /// Gets the System.Data.DataTable to which the column belongs to.
        /// </summary>
        public DataTable Table { get; }

        /// <summary>
        /// Gets the data value for this column index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                return this.ItemArray[index];
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
                int? index = this.Table
                                 .Columns
                                 .FirstOrDefault(c => String.Compare(c.ColumnName, columnName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                ?.Ordinal;

                if (index != null)
                    return this.ItemArray[index.Value];
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
            return this.ItemArray.GetEnumerator();
        }

        /// <summary>
        /// Provides strongly-typed access of the column values in the specified row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public T Field<T>(string columnName)
        {
            return (T)this[columnName];
        }

        /// <summary>
        /// Provides strongly-typed access of the column values in the specified row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public T Field<T>(int columnIndex)
        {
            return (T)this[columnIndex];
        }

        /// <summary>
        /// If item is null, creates a new instance of T type and sets all row values to the new T properties.
        /// If item is not null, sets all row values to item object properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        internal T ConvertTo<T>(T item)
        {
            Type type = typeof(T);
            bool isDynamicType = Convertor.DynamicConvertor.IsDynamic(type);

            if (isDynamicType)
            {
                var columns = this.Table.Columns.ToDictionary(c => c.ColumnName, c => c.IsNullable ? typeof(Nullable<>).MakeGenericType(c.DataType) : c.DataType);
                type = Convertor.DynamicConvertor.GetDynamicType(Convertor.DynamicConvertor.DYNAMIC_CLASS_NAME, columns);
            }

            // For anonymous type, creates a new instance and sets all data row values to this new object.
            if (Convertor.TypeExtension.IsAnonymousType(type))
            {
                try
                {
                    object newItem = Activator.CreateInstance(type, this.ItemArray);
                    return (T)newItem;
                }
                catch (MissingMethodException ex)
                {
                    throw new MissingMethodException("Properties of your anonymous class must be in the same type and same order of your SQL Query.", ex);
                }

            }

            // For defined type, creates a new instance and fill all data row values to this new object.
            else
            {
                object newItem = null;

                // Creates or gets an instance of T
                if (isDynamicType)
                    newItem = Activator.CreateInstance(type);
                if (EqualityComparer<T>.Default.Equals(item, default(T)))
                    newItem = Activator.CreateInstance(type, null);
                else
                    newItem = item;

                // List of all properties of T
                List<PropertyInfo> properties = new List<PropertyInfo>();
                properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

                // Check all columns values
                foreach (DataColumn column in this.Table.Columns)
                {
                    PropertyInfo property = null;

                    // Gets the first property with correct Column attribute name
                    if (property == null)
                    {
                        property = properties.FirstOrDefault(p => String.Compare(Apps72.Dev.Data.Annotations.ColumnAttribute.GetColumnAttributeName(p), column.ColumnName, true) == 0 && p.CanWrite);
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
    }
}

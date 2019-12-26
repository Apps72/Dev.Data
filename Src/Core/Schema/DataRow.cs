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
                int? index = this.Table.Columns.FirstOrDefault(c => c.ColumnName == columnName)?.Ordinal;
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
    }
}

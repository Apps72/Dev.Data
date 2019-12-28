using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Schema
{
    /// <summary>
    /// Represents one table of in-memory data.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{SchemaAndName}")]
    public partial class DataTable
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new instance of DataTable
        /// </summary>
        public DataTable()
        {
            this.Columns = null;
            this.Rows = null;
        }

        /// <summary>
        /// Initialize a new instance of DataTable,
        /// based on a single Row/Col value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="firstColRowValue"></param>
        internal DataTable(string tableName, string columnName, object firstColRowValue)
        {
            this.Name = tableName;
            this.Columns = new DataColumn[] { new DataColumn
                (
                    columnName: columnName,
                    dataType: firstColRowValue != null ? firstColRowValue.GetType() : typeof(object),
                    isNullable: true
                )};

            this.Rows = new DataRow[] { new DataRow(this, new object[] { firstColRowValue }) };
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the name of this Table
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the Schema of this table
        /// </summary>
        public string Schema { get; internal set; }

        /// <summary>
        /// Gets the Schema and the Name of this table, separated by an underscore.
        /// </summary>
        public string SchemaAndName
        {
            get
            {
                return $"{Schema}_{Name}";
            }
        }

        /// <summary>
        /// Gets True if this 'Table' is a View.
        /// Not developed (always False)
        /// </summary>
        public bool IsView { get; internal set; }

        /// <summary>
        /// Gets the Columns properties
        /// </summary>
        public DataColumn[] Columns { get; internal set; }

        /// <summary>
        /// Gets all Rows values
        /// </summary>
        public DataRow[] Rows { get; internal set; }

        #endregion
    }
}

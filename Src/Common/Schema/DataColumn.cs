using System;
using System.Diagnostics;

namespace Apps72.Dev.Data.Schema
{
    /// <summary>
    /// Represents the schema of a column in a Table.
    /// </summary>
    [DebuggerDisplay("{ColumnName}")]
    public partial class DataColumn
    {
        /// <summary>
        /// Initializes a new instance of a Column
        /// </summary>
        /// <param name="table"></param>
        internal DataColumn(DataTable table)
        {
            this.Table = table;
        }

        /// <summary>
        /// Gets the System.Data.DataTable to which the column belongs to.
        /// </summary>
        public DataTable Table { get; set; }

        /// <summary>
        /// Gets the (zero-based) position of the column in the Columns collection.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets the name of the column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets the Original SQL DataType retrieve in the database (ex. INTEGER)
        /// </summary>
        public string SqlType { get; set; }

        /// <summary>
        /// Gets the type of data stored in the column.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets a value that indicates whether null values are allowed in this column
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets the Database type stored in the column.
        /// </summary>
        public System.Data.DbType DbType => Convertor.DataTypedConvertor.ToDbType(DataType);

        /// <summary>
        /// Returns the C# type suffixed by ?, if allowed
        /// Ex. Int32  -> Int32?
        ///     String -> String
        /// </summary>
        /// <param name="csharpType"></param>
        /// <returns></returns>
        private string GetCSharpTypeNullable(string csharpType)
        {
            if (this.IsNullable &&
                   String.Compare(csharpType, "System.String", ignoreCase: true) != 0 &&
                   String.Compare(csharpType, "System.Object", ignoreCase: true) != 0 &&
                   String.Compare(csharpType, "System.Byte[]", ignoreCase: true) != 0 &&
                   String.Compare(csharpType, "String", ignoreCase: true) != 0 &&
                   String.Compare(csharpType, "Object", ignoreCase: true) != 0 &&
                   String.Compare(csharpType, "Byte[]", ignoreCase: true) != 0)
            {
                return $"{csharpType}?";
            }
            else
            {
                return csharpType;
            }
        }
    }
}

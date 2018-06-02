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
        public DataTable Table { get; internal set; }

        /// <summary>
        /// Gets the (zero-based) position of the column in the Columns collection.
        /// </summary>
        public int Ordinal { get; internal set; }

        /// <summary>
        /// Gets the name of the column
        /// </summary>
        public string ColumnName { get; internal set; }

        /// <summary>
        /// Gets the Original SQL DataType retrieve in the database (ex. INTEGER)
        /// </summary>
        public string SqlType { get; internal set; }

        /// <summary>
        /// Gets the type of data stored in the column.
        /// </summary>
        public Type DataType { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether null values are allowed in this column
        /// </summary>
        public bool IsNullable { get; internal set; }

        /// <summary>
        /// Gets the DataType formated as a string
        /// </summary>
        public string DotNetType => DataType.ToString().Replace("System.", String.Empty);

        /// <summary>
        /// Gets the DataType formated as a string
        /// </summary>
        public string CSharpType => Convertor.DbTypeMap.DotNetToCSharpType(DataType);

        /// <summary>
        /// Gets the DataType formated as a string suffixed by an option "?"
        /// </summary>
        public string DotNetTypeNullable => GetTypeNullable(false);

        /// <summary>
        /// Gets the DataType formated as a string suffixed by an option "?"
        /// </summary>
        public string CSharpTypeNullable => GetTypeNullable(true);

        /// <summary>
        /// Returns the C# type suffixed by ?, if allowed
        /// Ex. Int32  -> Int32?
        ///     String -> String
        /// </summary>
        /// <returns></returns>
        private string GetTypeNullable(bool isCSharp)
        {
            string dataType = DataType.ToString();
            string type = isCSharp ? Convertor.DbTypeMap.DotNetToCSharpType(this.DataType)
                                   : dataType;

            if (this.IsNullable &&
                dataType != "System.String" &&
                dataType != "System.Object" &&
                dataType != "System.Byte[]")
            {
                return $"{type}?".Replace("System.", String.Empty);
            }
            else
            {
                return type.Replace("System.", String.Empty);
            }
        }
    }
}

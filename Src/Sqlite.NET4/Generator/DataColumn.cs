using System;
using System.Diagnostics;

namespace Apps72.Dev.Data.Schema
{
    /// <summary />
    [DebuggerDisplay("{ColumnName} {SqlType}")]
    public partial class DataColumn
    {
        /// <summary>
        /// Gets the C# type associated to the SqlType (ex. Int32)
        /// </summary>
        public string CSharpType { get; internal set; }

        /// <summary>
        /// Gets the C# type associated to the SqlType 
        /// suffixed by ? if the field is nullable and if it's not a String or Byte[]
        /// (ex. "Int32?" but "string")
        /// </summary>
        public string CSharpTypeNullable => GetCSharpTypeNullable(CSharpType);
    }
}

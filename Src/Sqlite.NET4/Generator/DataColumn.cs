using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Apps72.Dev.Data.Schema
{
    /// <summary />
    [DebuggerDisplay("{ColumnName} {SqlType}")]
    public partial class DataColumn
    {
        private Dictionary<string, string> _mapping;

        /// <summary>
        /// Initializes a new instance of a Column
        /// </summary>
        /// <param name="table"></param>
        /// <param name="mapping"></param>
        internal DataColumn(DataTable table, Dictionary<string, string> mapping)
        {
            _mapping = mapping;
            this.Table = table;
        }

        /// <summary>
        /// Gets the Original SQL DataType retrieve in the database (ex. INTEGER)
        /// </summary>
        public string SqlType { get; set; }

        /// <summary>
        /// Gets the C# type associated to the SqlType (ex. Int32)
        /// </summary>
        public string CSharpType 
        {
            get
            {
                if (_mapping.ContainsKey(this.SqlType))
                    return _mapping[this.SqlType];
                else
                    return "Object";
            }
        }

        /// <summary>
        /// Gets the C# type associated to the SqlType 
        /// suffixed by ? if the field is nullable and if it's not a String or Byte[]
        /// (ex. "Int32?" but "string")
        /// </summary>
        public string CSharpTypeNullable
        {
            get
            {
                if (this.IsNullable &&
                    String.Compare(this.CSharpType, "System.String", ignoreCase: true) != 0 &&
                    String.Compare(this.CSharpType, "System.Object", ignoreCase: true) != 0 &&
                    String.Compare(this.CSharpType, "System.Byte[]", ignoreCase: true) != 0 &&
                    String.Compare(this.CSharpType, "String", ignoreCase: true) != 0 &&
                    String.Compare(this.CSharpType, "Object", ignoreCase: true) != 0 &&
                    String.Compare(this.CSharpType, "Byte[]", ignoreCase: true) != 0)
                {
                    return $"{this.CSharpType}?";
                }
                else
                {
                    return this.CSharpType;
                }
            }
        }
    }
}

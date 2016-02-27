using System;
using System.Diagnostics;

namespace Apps72.Dev.Data.Internal
{
    /// <summary>
    /// Description of a Data Column
    /// </summary>
    [DebuggerDisplay("{ColumnName}")]
    internal class DataColumn
    {
        /// <summary />
        public int Ordinal { get; set; }

        /// <summary />
        public string ColumnName { get; set; }

        /// <summary />
        public Type ColumnType { get; set; }

        /// <summary />
        public bool IsNullable { get; set; }
    }
}

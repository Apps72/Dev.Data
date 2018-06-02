using System;
using System.Collections.Generic;
using System.Text;

namespace Apps72.Dev.Data.Generator
{
    /// <summary />
    public class TableAndColumn
    {
        /// <summary />
        public int SequenceNumber { get; internal set; }
        /// <summary />
        public string SchemaName { get; internal set; }
        /// <summary />
        public string TableName { get; internal set; }
        /// <summary />
        public string ColumnName { get; internal set; }
        /// <summary />
        public string ColumnType { get; internal set; }
        /// <summary />
        public int ColumnSize { get; internal set; }
        /// <summary />
        public bool IsColumnNullable { get; internal set; }
        /// <summary />
        public bool IsView { get; internal set; }
    }
}

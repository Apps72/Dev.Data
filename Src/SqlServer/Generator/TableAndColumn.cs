using System;

namespace Apps72.Dev.Data.Generator
{
    /// <summary />
    internal class TableAndColumn
    {
        /// <summary />
        public string SchemaName { get; set; }
        /// <summary />
        public string TableName { get; set; }
        /// <summary />
        public string ColumnName { get; set; }
        /// <summary />
        public string ColumnType { get; set; }
        /// <summary />
        public int ColumnSize { get; set; }
        /// <summary />
        public bool IsColumnNullable { get; set; }
        /// <summary />
        public bool IsView { get; set; }
    }
}

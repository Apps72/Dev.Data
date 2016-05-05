using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.Generator
{
    /// <summary />
    [System.Diagnostics.DebuggerDisplay("{Schema}.{Name}")]
    public class Table
    {
        /// <summary />
        public string Name { get; set; }
        /// <summary />
        public string Schema { get; set; }
        /// <summary />
        public bool IsView { get; set; }
        /// <summary />
        public IEnumerable<Column> Columns { get; set; }
    }
}

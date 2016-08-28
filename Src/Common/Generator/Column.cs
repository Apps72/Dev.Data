using System;
using System.Linq;

namespace Apps72.Dev.Data.Generator
{
    /// <summary />
    [System.Diagnostics.DebuggerDisplay("{Name} {SqlType}")]
    public partial class Column
    {
        /// <summary />
        public string Name { get; set; }
        /// <summary />
        public bool IsNullable { get; set; }
        /// <summary />
        public string SqlType { get; set; }
    }
}

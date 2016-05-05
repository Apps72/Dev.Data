using System;
using System.Linq;

namespace Apps72.Dev.Data.Generator
{
    /// <summary />
    [System.Diagnostics.DebuggerDisplay("{Name} {SqlType}")]
    public class Column
    {
        /// <summary />
        public string Name { get; set; }
        /// <summary />
        public bool IsNullable { get; set; }
        /// <summary />
        public string SqlType { get; set; }

#if NET451
        /// <summary />
        public System.Data.SqlDbType? SqlDbType
        {
            get
            {
                System.Data.SqlDbType sqlDbType;
                if (Enum.TryParse(this.SqlType, true, out sqlDbType))
                {
                    return sqlDbType;
                }
                else if (String.Compare(this.SqlType, "numeric", true) == 0)
                {
                    return System.Data.SqlDbType.Decimal;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary />
        public string CSharpType
        {
            get
            {
                System.Data.SqlDbType? sqlDbType = this.SqlDbType;
                if (sqlDbType.HasValue)
                    return Convertor.DataTypedConvertor.ToNetType(sqlDbType.Value).Name;
                else
                    return "Object";
            }
        }
        /// <summary />
        public string CSharpTypeNullable
        {
            get
            {
                if (this.IsNullable &&
                    String.Compare(this.CSharpType, "String", ignoreCase: true) == 0 &&
                    String.Compare(this.CSharpType, "Byte[]", ignoreCase: true) == 0)
                {
                    return this.CSharpType + "?";
                }
                else
                {
                    return this.CSharpType;
                }
            }
        }
#endif
    }
}

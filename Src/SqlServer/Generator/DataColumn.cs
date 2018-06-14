using System;
using System.Diagnostics;
using System.Linq;

namespace Apps72.Dev.Data.Schema
{
    /// <summary />
    public partial class DataColumn
    {
        /// <summary>
        /// Gets the Original SqlDbType DataType
        /// </summary>
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
    }
}

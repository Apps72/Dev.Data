using System;
using System.Diagnostics;
using System.Linq;

namespace Apps72.Dev.Data.Schema
{
    /// <summary />
    [DebuggerDisplay("{ColumnName} {SqlType}")]
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

        /// <summary>
        /// Gets the C# type associated to the SqlType (ex. Int32)
        /// </summary>
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

        /// <summary>
        /// Gets the C# type associated to the SqlType 
        /// suffixed by ? if the field is nullable and if it's not a String or Byte[]
        /// (ex. "Int32?" but "string")
        /// </summary>
        public string CSharpTypeNullable => GetCSharpTypeNullable(CSharpType);

    }
}

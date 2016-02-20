using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Apps72.Dev.Data.Convertor
{
    public static partial class DataTypedConvertor
    {
        /// <summary>
        /// Convert db type to .Net data type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static Type ToNetType(System.Data.DbType dbType)
        {
            DbTypeMapEntry entry = DbTypeMap.First(t => t.DbType == dbType);
            return entry.Type;
        }

        /// <summary>
        /// Convert .Net type to Db type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static System.Data.DbType ToDbType(Type type)
        {
            DbTypeMapEntry entry = DbTypeMap.First(t => t.Type == type);
            return entry.DbType;
        }
       
    }
}

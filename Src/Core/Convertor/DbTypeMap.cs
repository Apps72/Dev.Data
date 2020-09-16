using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.Convertor
{
    /// <summary>
    /// DbType Mapping, using connection.GetSchema("DataTypes").
    /// This schema collection exposes information about the data types that are supported by the database 
    /// that the .NET Framework managed provider is currently connected to
    /// see https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/common-schema-collections
    /// </summary>
    internal static class DbTypeMap
    {
        private static DbTypeMapEntry DEFAULT_ENTRY;
        private static readonly List<DbTypeMapEntry> _dbProviderTypeList = new List<DbTypeMapEntry>();
        private static readonly List<Type2DbType> _dbTypeList = FillDbTypeList();

        /// <summary>
        /// Initialize the DbTypeMap
        /// </summary>
        public static void Initialize(DbConnection connection)
        {
            if (_dbProviderTypeList.Count == 0)
            {
                // Providers Data Types
                DataTable allTypes = connection.GetSchema("DataTypes");
                foreach (DataRow row in allTypes.Rows)
                {
                    if (String.Compare(Convert.ToString(row["TypeName"]), "tinyint", ignoreCase: true) == 0)
                    {
                        row["DataType"] = "System.Byte";    // TinyInt = System.Byte: Error in Microsoft SQL Server Type.
                    }

                    _dbProviderTypeList.Add(new DbTypeMapEntry(Convert.ToString(row["TypeName"]),
                                                               Convert.ToInt32(row["ProviderDbType"]),
                                                               Convert.ToString(row["DataType"])));
                }

                DEFAULT_ENTRY = _dbProviderTypeList.FirstOrDefault(i => String.Compare(i.SqlTypeName, "VARCHAR", ignoreCase: true) == 0 ||
                                                                        String.Compare(i.SqlTypeName, "VARCHAR2", ignoreCase: true) == 0);

                if (DEFAULT_ENTRY == null)
                    DEFAULT_ENTRY = _dbProviderTypeList.First();
            }
        }

        /// <summary>
        /// Returns the CSharp type of a .NET Type
        /// String.Int32 => int
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string DotNetToCSharpType(Type type)
        {
            if (type == typeof(Boolean) || type == typeof(Boolean?)) return "bool";
            if (type == typeof(Byte) || type == typeof(Byte?)) return "byte";
            if (type == typeof(SByte) || type == typeof(SByte?)) return "sbyte";
            if (type == typeof(Char) || type == typeof(Char?)) return "char";
            if (type == typeof(Decimal) || type == typeof(Decimal?)) return "decimal";
            if (type == typeof(Double) || type == typeof(Double?)) return "double";
            if (type == typeof(Single) || type == typeof(Single?)) return "float";
            if (type == typeof(Int32) || type == typeof(Int32?)) return "int";
            if (type == typeof(UInt32) || type == typeof(UInt32?)) return "uint";
            if (type == typeof(Int64) || type == typeof(Int64?)) return "long";
            if (type == typeof(UInt64) || type == typeof(UInt64?)) return "ulong";
            if (type == typeof(Object)) return "object";
            if (type == typeof(Int16) || type == typeof(Int16?)) return "short";
            if (type == typeof(UInt16) || type == typeof(UInt16?)) return "ushort";
            if (type == typeof(String)) return "string";
            if (type == typeof(DateTime) || type == typeof(DateTime?)) return "DateTime";
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?)) return "TimeSpan";
            if (type == typeof(Guid) || type == typeof(Guid?)) return "Guid";
            if (type.ToString() == "System.Byte[]") return "byte[]";
            return "object";
        }

        /// <summary>
        /// Returns the first element using the <paramref name="sqlType"/>.
        /// </summary>
        /// <param name="sqlType">Name of SQL type to search.</param>
        /// <returns></returns>

        public static Type FirstType(string sqlType)
        {
            return _dbProviderTypeList.FirstOrDefault(i => String.Compare(i.SqlTypeName, sqlType, ignoreCase: true) == 0)?.DotNetType ?? typeof(System.Object);
        }
        public static DbType FirstDbType(Type type)
        {
            if (type == null) return DbType.Object;
            return _dbTypeList.First(i => i.Type == type).DbType;
        }
        public static DbTypeMapEntry FirstMappedType(DbType dbType)
        {
            return _dbProviderTypeList.FirstOrDefault(i => i.DbType == dbType) ?? DEFAULT_ENTRY;
        }

        public static bool IsStringRepresentation(DbType dbType)
        {
            if (_dbProviderTypeList.Any(i => i.DbType == dbType) == false)
                return true;

            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.Binary:
                case DbType.Object:
                case DbType.String:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    return true;
                default:
                    return false;
            }
        }

        static List<Type2DbType> FillDbTypeList()
        {
            var list = new List<Type2DbType>();

            list.Add(new Type2DbType(typeof(Int16), DbType.Int16));
            list.Add(new Type2DbType(typeof(Int32), DbType.Int32));
            list.Add(new Type2DbType(typeof(Int64), DbType.Int64));
            list.Add(new Type2DbType(typeof(UInt16), DbType.UInt16));
            list.Add(new Type2DbType(typeof(UInt32), DbType.UInt32));
            list.Add(new Type2DbType(typeof(UInt64), DbType.UInt64));
            list.Add(new Type2DbType(typeof(Boolean), DbType.Boolean));
            list.Add(new Type2DbType(typeof(Byte), DbType.Byte));
            list.Add(new Type2DbType(typeof(SByte), DbType.SByte));
            list.Add(new Type2DbType(typeof(Decimal), DbType.Decimal));
            list.Add(new Type2DbType(typeof(Decimal), DbType.Single));
            list.Add(new Type2DbType(typeof(Double), DbType.Double));
            list.Add(new Type2DbType(typeof(Decimal), DbType.Currency));
            list.Add(new Type2DbType(typeof(Double), DbType.VarNumeric));
            list.Add(new Type2DbType(typeof(Single), DbType.Single));

            list.Add(new Type2DbType(typeof(Char), DbType.String));
            list.Add(new Type2DbType(typeof(String), DbType.String));
            list.Add(new Type2DbType(typeof(String), DbType.AnsiString));
            list.Add(new Type2DbType(typeof(String), DbType.AnsiStringFixedLength));
            list.Add(new Type2DbType(typeof(String), DbType.Xml));

            list.Add(new Type2DbType(typeof(DateTime), DbType.DateTime));
            list.Add(new Type2DbType(typeof(DateTime), DbType.Date));
            list.Add(new Type2DbType(typeof(DateTime), DbType.DateTime2));
            list.Add(new Type2DbType(typeof(DateTime), DbType.DateTime));
            list.Add(new Type2DbType(typeof(DateTimeOffset), DbType.DateTimeOffset));
            list.Add(new Type2DbType(typeof(DateTime), DbType.Time));
            list.Add(new Type2DbType(typeof(TimeSpan), DbType.Time));

            list.Add(new Type2DbType(typeof(Guid), DbType.Guid));
            list.Add(new Type2DbType(typeof(Object), DbType.Object));
            list.Add(new Type2DbType(typeof(Byte[]), DbType.Binary));
            list.Add(new Type2DbType(typeof(Object), DbType.Object));

            return list;
        }
    }

    /// <summary>
    /// Mapping type structure to convert C# type to DbType, or to SqlDbType
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{SqlTypeName} - {DotNetDataType}")]
    internal class DbTypeMapEntry
    {
        /// <summary />
        public DbTypeMapEntry(string sqlTypeName, int enumProviderDbType, string dotNetDataType)
        {
            this.SqlTypeName = sqlTypeName;
            this.EnumProviderDbType = enumProviderDbType;
            this.DotNetDataType = dotNetDataType;
            this.DotNetType = Type.GetType(DotNetDataType);
            this.DbType = DbTypeMap.FirstDbType(DotNetType);
        }

        /// <summary />
        public string SqlTypeName { get; }              // tinyint
        /// <summary />
        public int EnumProviderDbType { get; }          // 20 => SqlDbType.TinyInt
        /// <summary />
        public string DotNetDataType { get; }           // System.SByte
        /// <summary />
        public Type DotNetType { get; }
        /// <summary />
        public DbType DbType { get; }
        /// <summary />
        public T GetProviderDbType<T>() where T : struct, IConvertible
        {
            return (T)Convert.ChangeType(EnumProviderDbType, typeof(T));
        }

    }

    /// <summary />
    internal class Type2DbType
    {
        /// <summary />
        public Type2DbType(Type type, DbType dbType)
        {
            Type = type;
            DbType = dbType;
        }

        /// <summary />
        public Type Type { get; }
        /// <summary />
        public DbType DbType { get; }
    }
}

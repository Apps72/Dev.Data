using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Convert DataRows to Typed objects or Typed objects to DataRows.
    /// </summary>
    public class DataTypedConvertor
    {
        private static readonly ArrayList _dbTypeList = new ArrayList();

        static DataTypedConvertor()
        {
            FillDbTypeList();
        }

        /// <summary>
        /// Creates a new instance of T type and sets all row values to the new T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static T[] DataTableTo<T>(System.Data.DataTable table)
        {
            T[] results = new T[table.Rows.Count];

            // If is Primitive type (string, int, ...)
            if (DataTypedConvertor.IsPrimitive(typeof(T)))
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    object scalar = table.Rows[i][0];
                    if (scalar == null || scalar == DBNull.Value)
                        results[i] = default(T);
                    else
                        results[i] = (T)scalar;
                }
            }

            // If is Complex type (class)
            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    results[i] = DataTypedConvertor.DataRowTo<T>(table.Rows[i]);
                }
            }

            return results;
        }

        /// <summary>
        /// Creates a new instance of T type and sets all row values to the new T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T DataRowTo<T>(System.Data.DataRow row)
        {
            return DataTypedConvertor.DataRowTo<T>(row, default(T));
        }

        /// <summary>
        /// If item is null, creates a new instance of T type and sets all row values to the new T properties.
        /// If item is not null, sets all row values to item object properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public static T DataRowTo<T>(System.Data.DataRow row, T item)
        {
            if (row != null)
            {
                Type type = typeof(T);
                object[] values = row.ItemArray;

                // For anonymous type, creates a new instance and sets all data row values to this new object.
                if (IsAnonymousType(type))
                {
                    object newItem = Activator.CreateInstance(type, values);
                    return (T)newItem;
                }

                // For defined type, creates a new instance and fill all data row values to this new object.
                else
                {
                    object newItem = null;

                    // Creates or gets an instance of T
                    if (EqualityComparer<T>.Default.Equals(item, default(T)))
                        newItem = Activator.CreateInstance(type, null);
                    else
                        newItem = item;

                    // List of all properties of T
                    List<PropertyInfo> properties = new List<PropertyInfo>();
                    properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

                    // Check all columns values
                    foreach (System.Data.DataColumn column in row.Table.Columns)
                    {
                        PropertyInfo property = null;

                        // Gets the first property with correct Column attribute name
                        if (property == null)
                        {
                            property = properties.FirstOrDefault(p => String.Compare(Annotations.ColumnAttribute.GetColumnAttributeName(p), column.ColumnName, true) == 0 && p.CanWrite);
                        }

                        // If not found, gets the first property with corrct name
                        if (property == null)
                        {
                            property = properties.FirstOrDefault(p => String.Compare(p.Name, column.ColumnName, true) == 0 && p.CanWrite);
                        }

                        if (property != null)
                        {
                            object value = row[column.ColumnName];
                            property.SetValue(newItem, value == System.DBNull.Value ? null : value, null);
                        }
                    }

                    return (T)newItem;
                }
            }
            else
            {
                return default(T);
            }

        }

        /// <summary>
        /// Creates a new instance of DataTabmle and sets all row values from the T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> array)
        {
            if (array != null)
            {
                DataTable table = new DataTable();
                Dictionary<string, string> columnNames = new Dictionary<string, string>();  // List of PropertyName / ColumnName
                bool isPrimitiveOnly = IsPrimitive(typeof(T));

                if (isPrimitiveOnly)
                {
                    columnNames.Add("Default", String.Empty);
                    table.Columns.Add(new DataColumn("Default", GetNullableType(typeof(T))));
                }
                else
                {
                    // Sets DataTable.Columns
                    foreach (PropertyInfo property in typeof(T).GetProperties())
                    {
                        if (IsPrimitive(property.PropertyType))
                        {
                            string attribute = Annotations.ColumnAttribute.GetColumnAttributeName(property);
                            columnNames.Add(property.Name, attribute);

                            if (string.IsNullOrEmpty(attribute))
                            {
                                table.Columns.Add(new DataColumn(property.Name, GetNullableType(property.PropertyType)));
                            }
                            else
                            {
                                table.Columns.Add(new DataColumn(attribute, GetNullableType(property.PropertyType)));
                            }
                        }
                    }
                }

                // Sets all values
                foreach (T item in array)
                {
                    DataRow newRow = table.NewRow();
                    foreach (var prop in columnNames)
                    {
                        string columnName = string.IsNullOrEmpty(prop.Value) ? prop.Key : prop.Value;
                        object value = isPrimitiveOnly ? item : typeof(T).GetProperty(prop.Key).GetValue(item, null);
                        newRow[columnName] = value == null ? DBNull.Value : value;
                    }
                    table.Rows.Add(newRow);
                }

                return table;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Creates a new instance of IDataParameter[] with ParameterName, Value and IsNullable properties 
        /// sets to value's properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<IDataParameter> ToParameterCollection<T>(T value)
        {
            if (IsPrimitive(typeof(T)))
            {
                throw new ArgumentException("The value can not be a simple type (string, int, ...), but an object with simple properties.", "value");
            }
            else
            {
                List<DataParameter> parameters = new List<DataParameter>();
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    if (IsPrimitive(property.PropertyType))
                    {
                        // Data type
                        Type propType = GetNullableType(property.PropertyType);

                        // Value
                        DataParameter parameter = new DataParameter();
                        parameter.Value = typeof(T).GetProperty(property.Name).GetValue(value, null);
                        parameter.IsNullable = IsNullable(propType);
                        parameter.DbType = DataTypedConvertor.ToDbType(propType);

                        // Parameter name
                        string attribute = Annotations.ColumnAttribute.GetColumnAttributeName(property);
                        if (string.IsNullOrEmpty(attribute))
                        {
                            parameter.ParameterName = property.Name;
                        }
                        else
                        {
                            parameter.ParameterName = attribute;
                        }

                        parameters.Add(parameter);
                    }
                }

                return parameters.AsEnumerable();
            }

        }

        /// <summary>
        /// Returns True if this object is a simple type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsPrimitive(Type type)
        {
            return type == typeof(DateTime) ||
                   type == typeof(Decimal) ||
                   type == typeof(String) ||
                   GetNullableType(type).IsPrimitive;
        }

        /// <summary>
        /// Returns True if the specified type is an AnonymousType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsAnonymousType(Type type)
        {
            bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        /// <summary>
        /// Returns True if the specified type is nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Returns the sub-type if specified type is null or
        /// returns the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Type GetNullableType(Type type)
        {
            if (DataTypedConvertor.IsNullable(type))
            {
                return type.GetGenericArguments()[0];
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// Fill all dbTypeList entries
        /// See https://gist.github.com/abrahamjp
        /// </summary>
        private static void FillDbTypeList()
        {
            DbTypeMapEntry dbTypeMapEntry
            = new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(byte), DbType.Double, SqlDbType.TinyInt);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Image);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(Decimal), DbType.Decimal, SqlDbType.Decimal);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(Int16), DbType.Int16, SqlDbType.SmallInt);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(Int32), DbType.Int32, SqlDbType.Int);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(Int64), DbType.Int64, SqlDbType.BigInt);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant);
            _dbTypeList.Add(dbTypeMapEntry);

            dbTypeMapEntry
            = new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar);
            _dbTypeList.Add(dbTypeMapEntry);
        }

        #region SqlDbType and DBType Convertors

        /// <summary>
        /// Convert db type to .Net data type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static Type ToNetType(DbType dbType)
        {
            DbTypeMapEntry entry = Find(dbType);
            return entry.Type;
        }

        /// <summary>
        /// Convert TSQL type to .Net data type
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static Type ToNetType(SqlDbType sqlDbType)
        {
            DbTypeMapEntry entry = Find(sqlDbType);
            return entry.Type;
        }

        /// <summary>
        /// Convert .Net type to Db type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(Type type)
        {
            DbTypeMapEntry entry = Find(type);
            return entry.DbType;
        }

        /// <summary>
        /// Convert TSQL data type to DbType
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static DbType ToDbType(SqlDbType sqlDbType)
        {
            DbTypeMapEntry entry = Find(sqlDbType);
            return entry.DbType;
        }

        /// <summary>
        /// Convert .Net type to TSQL data type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(Type type)
        {
            DbTypeMapEntry entry = Find(type);
            return entry.SqlDbType;
        }

        /// <summary>
        /// Convert DbType type to TSQL data type
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(DbType dbType)
        {
            DbTypeMapEntry entry = Find(dbType);
            return entry.SqlDbType;
        }

        private static DbTypeMapEntry Find(Type type)
        {
            object retObj = null;
            for (int i = 0; i < _dbTypeList.Count; i++)
            {
                DbTypeMapEntry entry = (DbTypeMapEntry)_dbTypeList[i];
                if (entry.Type == type)
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
            {
                throw
                new ApplicationException("Referenced an unsupported Type");
            }

            return (DbTypeMapEntry)retObj;
        }

        private static DbTypeMapEntry Find(DbType dbType)
        {
            object retObj = null;
            for (int i = 0; i < _dbTypeList.Count; i++)
            {
                DbTypeMapEntry entry = (DbTypeMapEntry)_dbTypeList[i];
                if (entry.DbType == dbType)
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
            {
                throw
                new ApplicationException("Referenced an unsupported DbType");
            }

            return (DbTypeMapEntry)retObj;
        }

        private static DbTypeMapEntry Find(SqlDbType sqlDbType)
        {
            object retObj = null;
            for (int i = 0; i < _dbTypeList.Count; i++)
            {
                DbTypeMapEntry entry = (DbTypeMapEntry)_dbTypeList[i];
                if (entry.SqlDbType == sqlDbType)
                {
                    retObj = entry;
                    break;
                }
            }
            if (retObj == null)
            {
                throw
                new ApplicationException("Referenced an unsupported SqlDbType");
            }

            return (DbTypeMapEntry)retObj;
        }

        private struct DbTypeMapEntry
        {
            public Type Type;
            public DbType DbType;
            public SqlDbType SqlDbType;
            public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
            {
                this.Type = type;
                this.DbType = dbType;
                this.SqlDbType = sqlDbType;
            }

        };

        #endregion

        /// <summary>
        /// Class implementing IDataParameter
        /// </summary>
        public class DataParameter : IDataParameter
        {
            /// <summary>
            /// <see cref="IDataParameter.DbType"/>
            /// </summary>
            public DbType DbType { get; set; }

            /// <summary>
            /// <see cref="IDataParameter.IsNullable"/>
            /// </summary>
            public bool IsNullable { get; set; }

            /// <summary>
            /// <see cref="IDataParameter.ParameterName"/>
            /// </summary>
            public string ParameterName { get; set; }

            /// <summary>
            /// <see cref="IDataParameter.Value"/>
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// <see cref="IDataParameter.Direction"/>
            /// </summary>
            public ParameterDirection Direction { get; set; }

            /// <summary>
            /// <see cref="IDataParameter.SourceColumn"/>
            /// </summary>
            public string SourceColumn { get; set; }

            /// <summary>
            /// <see cref="IDataParameter.SourceVersion"/>
            /// </summary>
            public DataRowVersion SourceVersion { get; set; }
           
        }

        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Apps72.Dev.Data.Convertor
{
    /// <summary>
    /// Convert DataRows to Typed objects or Typed objects to DataRows.
    /// </summary>
    public static partial class DataTypedConvertor
    {
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
            if (TypeExtension.IsPrimitive(typeof(T)))
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
        public static T DataRowTo<T>(System.Data.DataRow row, T item)
        {
            if (row != null)
            {
                Type type = typeof(T);
                object[] values = row.ItemArray;

                // For anonymous type, creates a new instance and sets all data row values to this new object.
                if (TypeExtension.IsAnonymousType(type))
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
        public static System.Data.DataTable ToDataTable<T>(IEnumerable<T> array)
        {
            if (array != null)
            {
                System.Data.DataTable table = new System.Data.DataTable();
                Dictionary<string, string> columnNames = new Dictionary<string, string>();  // List of PropertyName / ColumnName
                bool isPrimitiveOnly = TypeExtension.IsPrimitive(typeof(T));

                if (isPrimitiveOnly)
                {
                    columnNames.Add("Default", String.Empty);
                    table.Columns.Add(new System.Data.DataColumn("Default", TypeExtension.GetNullableSubType(typeof(T))));
                }
                else
                {
                    // Sets DataTable.Columns
                    foreach (PropertyInfo property in typeof(T).GetProperties())
                    {
                        if (TypeExtension.IsPrimitive(property.PropertyType))
                        {
                            string attribute = Annotations.ColumnAttribute.GetColumnAttributeName(property);
                            columnNames.Add(property.Name, attribute);

                            if (string.IsNullOrEmpty(attribute))
                            {
                                table.Columns.Add(new System.Data.DataColumn(property.Name, TypeExtension.GetNullableSubType(property.PropertyType)));
                            }
                            else
                            {
                                table.Columns.Add(new System.Data.DataColumn(attribute, TypeExtension.GetNullableSubType(property.PropertyType)));
                            }
                        }
                    }
                }

                // Sets all values
                foreach (T item in array)
                {
                    System.Data.DataRow newRow = table.NewRow();
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

        ///// <summary>
        ///// Convert TSQL type to .Net data type
        ///// </summary>
        ///// <param name="sqlDbType"></param>
        ///// <returns></returns>
        //public static Type ToNetType(System.Data.SqlDbType sqlDbType)
        //{
        //    DbTypeMapEntry entry = DbTypeMap.First(t => t.SqlDbType == sqlDbType);
        //    return entry.Type;
        //}

        ///// <summary>
        ///// Convert .Net type to TSQL data type
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static System.Data.SqlDbType ToSqlDbType(Type type)
        //{
        //    DbTypeMapEntry entry = DbTypeMap.First(t => t.Type == type);
        //    return entry.SqlDbType;
        //}

        ///// <summary>
        ///// Convert DbType type to TSQL data type
        ///// </summary>
        ///// <param name="dbType"></param>
        ///// <returns></returns>
        //public static System.Data.SqlDbType ToSqlDbType(System.Data.DbType dbType)
        //{
        //    DbTypeMapEntry entry = DbTypeMap.First(t => t.DbType == dbType);
        //    return entry.SqlDbType;
        //}

        ///// <summary>
        ///// Convert TSQL data type to DbType
        ///// </summary>
        ///// <param name="sqlDbType"></param>
        ///// <returns></returns>
        //public static System.Data.DbType ToDbType(System.Data.SqlDbType sqlDbType)
        //{
        //    DbTypeMapEntry entry = DbTypeMap.First(t => t.SqlDbType == sqlDbType);
        //    return entry.DbType;
        //}
    }
}
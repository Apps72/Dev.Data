using System;
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

    }
}

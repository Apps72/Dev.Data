using System;
using System.Reflection;

namespace Apps72.Dev.Data.Convertor
{
    /// <summary />
    internal static class DataRowConvertor
    {
        /// <summary />
        internal static T ToType<T>(this Schema.DataRow row)
        {
            var rowType = typeof(T);

            // *** Primitive type
            if (TypeExtension.IsPrimitive(rowType))
            {
                return (T)Convert.ChangeType(row[0], typeof(T));
            }

            // *** Class
            else
            {
                var properties = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .ToDictionaryWithAttributeOrName();
                var newItem = Activator.CreateInstance<T>();

                foreach (var column in row.Table.Columns)
                {
                    var name = column.ColumnName;
                    var property = properties.GetValueOrDefault(name);
                    if (property != null)
                    {
                        var value = row[name];
                        property.SetValue(newItem, value == DBNull.Value ? null : value, null);
                    }
                }

                return (T)Convert.ChangeType(newItem, typeof(T));
            }
        }
    }
}

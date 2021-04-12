using Apps72.Dev.Data.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

            // Get or add the T properties in a small cache
            TypeCached typeDetails = DataRow.MAPTO_CACHED_CLASSES_MAXIMUM > 0 
                                   ? GetOrAddCachedPropertiesOf<T>()        // Using the cache
                                   : new TypeCached(rowType);               // If MAPTO_CACHED_CLASSES = 0, cache is disabled

            // *** Primitive type
            if (typeDetails.IsPrimitive)
            {
                return (T)Convert.ChangeType(row[0], typeof(T));
            }

            // *** Class
            else
            {
                var properties = typeDetails.Properties;
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

        /// <summary />
        private static TypeCached GetOrAddCachedPropertiesOf<T>()
        {
            Type rowType = typeof(T);

            return _type_cached.GetOrAdd(rowType.FullName, (key) =>
            {
                TypeCached typeCached;
                bool found = _type_cached.TryGetValue(key, out typeCached);

                if (found)
                {
                    return typeCached;
                }
                else
                {
                    if (_type_cached.Count > DataRow.MAPTO_CACHED_CLASSES_MAXIMUM)
                    {
                        var keyToDelete = _type_cached.ToArray().OrderByDescending(i => i.Value.Recorded).First().Key;
                        _type_cached.TryRemove(keyToDelete, out typeCached);
                    }
                    return new TypeCached(typeof(T));
                }
            });
        }

        #region Cached Properties

        /// <summary />
        private static ConcurrentDictionary<string, TypeCached> _type_cached = new ConcurrentDictionary<string, TypeCached>();

        /// <summary />
        internal static IEnumerable<string> GetCachedClassNames() => _type_cached.ToArray().Select(i => i.Key);

        /// <summary />
        internal static void CleanCache() => _type_cached.Clear();

        /// <summary />
        class TypeCached
        {
            /// <summary />
            public TypeCached(Type type)
            {
                Name = type.FullName;
                Recorded = DateTime.Now;
                IsPrimitive = TypeExtension.IsPrimitive(type);
                if (IsPrimitive == false)
                {
                    Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .ToDictionaryWithAttributeOrName();
                }
            }

            /// <summary />
            public string Name { get; set; }
            /// <summary />
            public DateTime Recorded { get; }
            /// <summary />
            public bool IsPrimitive { get; }
            /// <summary />
            public IDictionary<string, PropertyInfo> Properties { get; }
        }

        #endregion
    }
}

using System.Collections.Generic;

namespace Apps72.Dev.Data
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Tries to get the value associated with the specified key in the dictionary.
        /// <paramref name="defaultValue"/> is returned if the key does not exists.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionary.</typeparam>
        /// <typeparam name="TValue">The value type of the dictionary.</typeparam>
        /// <param name="dictionary">The dictionnary to lookup.</param>
        /// <param name="key">The search to search for.</param>
        /// <param name="defaultValue">The default value if no key is found.</param>
        /// <returns>The value associated to the specified key. If the key does not exists, <paramref name="defaultValue"/> is returned.</returns>
        internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue)) where TValue : struct
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }
    }
}

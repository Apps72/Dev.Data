using System;
using System.Collections.Generic;
using System.Linq;

namespace Apps72.Dev.Data.Generator.Tools
{
    /// <summary>
    /// Source: https://github.com/Apps72/Dev.Configuration/
    /// </summary>
    public class CommandLine
    {
        private const char GUILLEMET = '"';
        private readonly string[] SEPARATORS = new string[] { "=", ":" };
        private readonly string[] PREFIX = new string[] { "--", "-", "/" };     // Needed to have "--" before "-"

        /// <summary>
        /// Initializes a new instance of CommandLine with arguments received from the Main method.
        /// </summary>
        /// <param name="args"></param>
        public CommandLine(string[] args)
        {
            this.Arguments = GetArguments(args);
        }

        /// <summary>
        /// Gets the value associated to the command line arguments, for the key, or null if this key is not found.
        /// </summary>
        /// <param name="key">Key name (not case sensitive)</param>
        /// <returns></returns>
        public string GetValue(params string[] key)
        {
            foreach (var arg in Arguments)
            {
                if (key.Any(i => i.IsEqualTo(arg.Key)))
                    return arg.Value;
            }
            return null;
        }

        /// <summary>
        /// Determines whether the command line arguments contains an element with the specified key.
        /// </summary>
        /// <param name="key">Key name (not case sensitive)</param>
        /// <returns></returns>
        public bool ContainsKey(params string[] key)
        {
            foreach (var arg in Arguments)
            {
                if (key.Any(i => i.IsEqualTo(arg.Key)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a list of all arguments splitted to a dictionary of key/value
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> Arguments { get; private set; }

        /// <summary>
        /// Returns a list of all arguments splitted to a dictionary of key/value
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private IDictionary<string, string> GetArguments(string[] args)
        {
            var allItems = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var seperatorFound = arg.IndexOfAny(SEPARATORS);
                int keyLength = seperatorFound.Index;
                int valueStartIndex = seperatorFound.Index + seperatorFound.Separator.Length;

                if (seperatorFound.Index < 0)
                {
                    keyLength = arg.Length;
                    valueStartIndex = arg.Length;
                }

                if (keyLength >= 0 && valueStartIndex > 0)
                {
                    string key = arg.Substring(0, keyLength).Trim();
                    string value = arg.Substring(valueStartIndex).Trim();

                    // Remove the first chars (prefix) to have the key
                    var prefixFound = key.IndexOfAny(PREFIX);
                    if (prefixFound.Index >= 0)
                        key = key.Substring(prefixFound.Separator.Length).Trim();

                    // Remove optional guillemets to have the value
                    if (value.Length > 2 && value.StartsWith(GUILLEMET) && value.EndsWith(GUILLEMET))
                        value = value.Substring(1, value.Length - 2);

                    if (!String.IsNullOrEmpty(key) && !allItems.ContainsKey(key))
                    {
                        allItems.Add(key, value);
                    }
                }
            }

            return allItems;
        }

    }
}

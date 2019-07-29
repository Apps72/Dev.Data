using System;
using System.Text.RegularExpressions;

namespace Apps72.Dev.Data.Generator.Tools
{
    /// <summary>
    /// Represents a wildcard running on the
    /// <see cref="System.Text.RegularExpressions"/> engine.
    /// </summary>
    /// <remarks>
    /// See https://www.codeproject.com/articles/11556/converting-wildcards-to-regexes
    /// </remarks>
    public class Wildcard : Regex
    {
        /// <summary>
        /// Initializes a wildcard with the given search pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        public Wildcard(string pattern)
         : base(WildcardToRegex(pattern))
        {
        }

        /// <summary>
        /// Initializes a wildcard with the given search pattern and options.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="options">A combination of one or more
        /// <see cref="System.Text.RegexOptions"/>.</param>
        public Wildcard(string pattern, RegexOptions options)
         : base(WildcardToRegex(pattern), options)
        {
        }

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace("\\*", ".*")
                              .Replace("\\?", ".") + "$";
        }

        public static bool HasWildcard(string pattern)
        {
            return pattern.Contains("*") || pattern.Contains("?");
        }
    }
}

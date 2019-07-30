using System;
using System.IO;

namespace Apps72.Dev.Data.Generator.Tools
{
    public static class StringExtensions
    {
        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any
        /// character in a specified array of Unicode characters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separators"></param>
        /// <returns>
        /// First argument: the zero-based index position of value if that string is found, or -1 if it is not.
        /// Second argument: the separator found, or String.Empty if it is not.
        /// </returns>
        public static (int Index, string Separator) IndexOfAny(this string text, string[] separators)
        {
            int indexFound = -1;
            foreach (var separator in separators)
            {
                indexFound = text.IndexOf(separator);
                if (indexFound >= 0) return (indexFound, separator);
            }
            return (-1, string.Empty);
        }

        /// <summary>
        /// Returns True if the current string is equal to <paramref name="value"/>.
        /// The comparaison is based on Invariant Culture and Ignore Case.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEqualTo(this string text, string value)
        {
            return String.Compare(text, value, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns True if the current string is NOT equal to <paramref name="value"/>.
        /// The comparaison is based on Invariant Culture and Ignore Case.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEqualTo(this string text, string value)
        {
            return !IsEqualTo(text, value);
        }

        /// <summary>
        /// Returns the first <paramref name="length"/> first characters.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string text, int length)
        {
            if (text.Length > length)
                return text.Substring(0, length);
            else
                return text;
        }

        /// <summary>
        /// Returns the file name only, without extension.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string NameWithoutExtension(this FileInfo file)
        {
            return file.Name.Substring(0, file.Name.Length - file.Extension.Length);
        }
    }
}
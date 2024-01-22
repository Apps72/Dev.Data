using System;
using System.Text;

namespace Apps72.Dev.Data.Convertor
{
    internal static class TypeExtension
    {
        /// <summary>
        /// Returns True if the specified type is an AnonymousType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(Type type)
        {
            return type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
        }

        /// <summary>
        /// Returns True if this object is a simple type.
        /// See https://msdn.microsoft.com/en-us/library/system.type.isprimitive.aspx
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitive(Type type)
        {
            return type == typeof(DateTime) || type == typeof(Nullable<DateTime>) ||
                   type == typeof(Decimal) || type == typeof(Nullable<Decimal>) ||
                   type == typeof(String) ||
                   type == typeof(Boolean) || type == typeof(Nullable<Boolean>) ||
                   type == typeof(Byte) || type == typeof(Nullable<Byte>) ||
                   type == typeof(SByte) || type == typeof(Nullable<SByte>) ||
                   type == typeof(Int16) || type == typeof(Nullable<Int16>) ||
                   type == typeof(UInt16) || type == typeof(Nullable<UInt16>) ||
                   type == typeof(Int32) || type == typeof(Nullable<Int32>) ||
                   type == typeof(UInt32) || type == typeof(Nullable<UInt32>) ||
                   type == typeof(Int64) || type == typeof(Nullable<Int64>) ||
                   type == typeof(UInt64) || type == typeof(Nullable<UInt64>) ||
                   type == typeof(IntPtr) || type == typeof(Nullable<IntPtr>) ||
                   type == typeof(UIntPtr) || type == typeof(Nullable<UIntPtr>) ||
                   type == typeof(Char) || type == typeof(Nullable<Char>) ||
                   type == typeof(Double) || type == typeof(Nullable<Double>) ||
                   type == typeof(Single) || type == typeof(Nullable<Single>) ||
                   type == typeof(Guid) || type == typeof(Nullable<Guid>);
        }

        /// <summary>
        /// Returns True if the specified type is nullable
        /// See http://stackoverflow.com/questions/8939939/correct-way-to-check-if-a-type-is-nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null || !IsPrimitive(type);
        }

        /// <summary>
        /// Returns the sub-type if specified type is null or
        /// returns the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNullableSubType(Type type)
        {
            Type subType = Nullable.GetUnderlyingType(type);
            return subType == null ? type : subType;
        }

        /// <summary>
        /// Remove invalid chars for CSharp class and property names.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/gg615485.aspx </remarks>
        public static string RemoveExtraChars(this string name)
        {
            StringBuilder newName = new StringBuilder();
            int ascii = 0;

            // Keep only digits, letters or underscore
            foreach (char c in name)
            {
                // Ascii code of the current Char
                ascii = (int)c;

                // 0 .. 9, A .. Z, a .. z, _
                if (ascii >= 48 && ascii <= 57 ||
                    ascii >= 65 && ascii <= 90 ||
                    ascii >= 97 && ascii <= 122 ||
                    ascii == 95)
                {
                    newName.Append(c);
                }
                else
                {
                    newName.Append('_');
                }
            }

            // Name without extra chars (including '_')
            string tinyName = newName.ToString().Trim('_');

            // First char must be a letter or underscore
            if (tinyName.Length > 0)
            {
                ascii = (int)newName[0];
                if (ascii >= 65 && ascii <= 90 ||
                    ascii >= 97 && ascii <= 122 ||
                    ascii == 95)
                {
                    return newName.ToString();
                }
                else
                {
                    return $"_{newName.ToString()}";
                }
            }
            else
            {
                return $"__{Guid.NewGuid().ToString().Replace('-', '_')}";
            }

        }

        /// <summary>
        /// Convert the string to a boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string value)
        {
            switch (value.ToUpper())
            {
                case "YES":
                case "Y":
                case "TRUE":
                    return true;

                case "NO":
                case "N":
                case "FALSE":
                    return false;

                default:
                    return false;
            }
        }
    }

    internal class NoType
    {

    }
}

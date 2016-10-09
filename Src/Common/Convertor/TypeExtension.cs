using System;

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
                   type == typeof(Single) || type == typeof(Nullable<Single>);
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
    }
}

using System;

namespace Apps72.Dev.Data.Annotations
{
    /// <summary>
    /// Specifies that the property shouldn't be part of the database mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}

using System;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Annotations
{
    /// <summary>
    /// Specifies the database column that a property is mapped to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        readonly string _name;

        /// <summary>
        /// Initializes a new instance of the ColumnAttribute.
        /// </summary>
        /// <param name="name">The name of the column the property is mapped to.</param>
        public ColumnAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Gets the name of the column the property is mapped to.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Returns the Column.Name attribute for the specified property.
        /// </summary>
        /// <param name="property">Property</param>
        /// <returns>Column.Name attribute or String.Empty if not found</returns>
        internal static string GetColumnAttributeName(PropertyInfo property)
        {
            var customAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);
            var attribute = customAttributes?.FirstOrDefault(a => a is ColumnAttribute) as ColumnAttribute;
            return attribute?.Name;
        }
    }

}
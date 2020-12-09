using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Tools.Generator.Tests
{
    /// <summary>
    /// DataAnnotation extension
    /// </summary>
    public static class DataValidator
    {
        /// <summary>
        /// Validate the specified object, using DataAnnotations.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Don't call this method from IValidatableObject.Validate to avoid StackOverflow exception.
        /// </remarks>
        public static IEnumerable<ValidationResult> ValidateObject(object value)
        {
            return ValidateObject(value, extraValidations: null);
        }

        /// <summary>
        /// Validate the specified object, using DataAnnotations.
        /// And call an extra method to extra validations.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="extraValidations"></param>
        /// <returns></returns>
        /// <remarks>
        /// Don't call this method from IValidatableObject.Validate to avoid StackOverflow exception.
        /// </remarks>
        public static IEnumerable<ValidationResult> ValidateObject(object value, Action<List<ValidationResult>> extraValidations)
        {
            var context = new ValidationContext(value, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(value, context, results, validateAllProperties: true);
            extraValidations?.Invoke(results);
            return results;
        }

        /// <summary>
        /// Truncates all strings of an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T TruncateObjectProperties<T>(T item)
        {
            // Strings properties
            var properties = typeof(T).GetProperties()
                                      .Where(p => p.GetCustomAttribute(typeof(StringLengthAttribute)) != null &&
                                                  p.CanWrite && p.CanRead);
            // Set values
            foreach (var stringProperty in properties)
            {
                var stringAttribute = (StringLengthAttribute)stringProperty.GetCustomAttributes(typeof(StringLengthAttribute)).First();
                var maximumLength = stringAttribute.MaximumLength;
                stringProperty.SetValue(item, stringProperty.Name.Left(maximumLength), null);
            }
            return item;
        }

        // <summary>
        /// Truncate a string with maximum <paramref name="length"/> characters if needed.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string Left(this string item, int length)
        {
            if (item.Length > length)
                return item.Substring(0, length);
            else
                return item;
        }
    }
}

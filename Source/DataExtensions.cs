using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Helper Extensions to simplify data management
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Convert the parameter value to a DBNull.Value if this value is null.
        /// </summary>
        /// <param name="parameter"></param>
        public static void ConvertToDBNull(this DbParameter parameter)
        {
            if (parameter.Value == null)
            {
                parameter.Value = DBNull.Value;
            }
        }

        /// <summary>
        /// Adds a value to the end of the System.Data.SqlClient.SqlParameterCollection.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value to be added. Null value will be replaced by System.DBNull.Value.</param>
        /// <returns>A System.Data.SqlClient.SqlParameter object.</returns>
        public static SqlParameter AddWithValueOrDBNull(this SqlParameterCollection parameters, string parameterName, object value)
        {
            if (parameters != null)
            {
                return parameters.AddWithValue(parameterName, value == null ? System.DBNull.Value : value);
            }
            return null;
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator
        /// to the end of the current System.Text.StringBuilder object.
        /// </summary>
        /// <param name="builder">A stringBuilder to updtate</param>
        /// <param name="format">A composite format string to append.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.
        /// </exception>
        public static StringBuilder AppendLineFormat(this StringBuilder builder, string format, params object[] args)
        {
            if (builder != null)
            {
                if (format == null || args == null || args.Length <= 0)
                {
                    return builder.AppendLine(format);
                }
                else
                {
                    return builder.AppendLine(String.Format(format, args));
                } 
            }
            else
            {
                return null;
            }
        }
       
    }
}

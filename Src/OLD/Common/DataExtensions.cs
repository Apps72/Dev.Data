using System;
using System.Data.Common;
using System.Text;

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

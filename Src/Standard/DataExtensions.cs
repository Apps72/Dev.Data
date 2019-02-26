using System;
using System.Data.Common;

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
}

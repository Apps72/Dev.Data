using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

/// <summary>
/// Helper Extensions to simplify data management
/// </summary>
public static class DataExtensions
{
    /// <summary>
    /// Convert the parameter value to a DBNull.Value if this value is null.
    /// </summary>
    /// <param name="parameter"></param>
    public static DbParameter ConvertToDBNull(this DbParameter parameter)
    {
        if (parameter.Value == null)
        {
            parameter.Value = DBNull.Value;
        }
        return parameter;
    }

    /// <summary>
    /// Returns the internal DbTransaction associated to the <paramref name="connection"/>.
    /// </summary>
    /// <param name="connection">Connection to retrieve internal Transaction.</param>
    /// <returns></returns>
    public static DbTransaction GetTransaction(IDbConnection connection)
    {
        var info = connection.GetType().GetProperty("InnerConnection", BindingFlags.NonPublic | BindingFlags.Instance);
        var internalConn = info?.GetValue(connection, null);
        var currentTransactionProperty = internalConn?.GetType().GetProperty("CurrentTransaction", BindingFlags.NonPublic | BindingFlags.Instance);
        var currentTransaction = currentTransactionProperty?.GetValue(internalConn, null);
        var realTransactionProperty = currentTransaction?.GetType().GetProperty("Parent", BindingFlags.NonPublic | BindingFlags.Instance);
        var realTransaction = realTransactionProperty?.GetValue(currentTransaction, null);
        return (DbTransaction)realTransaction;
    }
}

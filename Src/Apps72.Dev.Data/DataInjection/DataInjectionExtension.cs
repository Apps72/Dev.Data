using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Extension method to inject data in requests (for Unit Testing)
    /// </summary>
    public static class DataInjectionExtension
    {
        private static readonly Dictionary<int, Func<DataInjectionDbCommand, object>> _listOfActions = new Dictionary<int, Func<DataInjectionDbCommand, object>>();

        #region METHODS

        /// <summary>
        /// Set the function to execute when the DatabaseCommand class need to retrieve data,
        /// and bypass the Database Server request process.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        public static void DefineDataInjection<T>(this DbConnection connection, Func<DataInjectionDbCommand, IEnumerable<T>> action)
        {
            _listOfActions.Add(connection.GetHashCode(), action);
        }

        #endregion

        #region PRIVATES

        /// <summary>
        /// Check if an action was previously registered for this DbConnection
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal static bool ContainsDataInjectionDataTable(this DbConnection connection)
        {
            return _listOfActions.ContainsKey(connection.GetHashCode());
        }

        /// <summary>
        /// Invoke the previously registered action and return the DataTable
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static IEnumerable<T> InvokeAndReturnData<T>(this DbConnection connection, DatabaseCommandBase command)
        {
            var item = _listOfActions[connection.GetHashCode()];
            return item.Invoke(new DataInjectionDbCommand(command.Command)) as IEnumerable<T>;
        }

        #endregion
    }
}

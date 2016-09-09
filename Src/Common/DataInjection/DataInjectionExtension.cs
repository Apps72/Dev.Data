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
        private static readonly Dictionary<int, Action<DataInjectionDbCommand>> _listOfActions = new Dictionary<int, Action<DataInjectionDbCommand>>();

        #region METHODS

        /// <summary>
        /// Set the function to execute when the DatabaseCommand class need to retrieve data,
        /// and bypass the Database Server request process.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        [Obsolete()]
        public static void DefineDataInjection(this DbConnection connection, Action<DataInjectionDbCommand> action)
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
        internal static DataInjectionDbCommand InvokeAndReturnData(this DbConnection connection, DatabaseCommandBase command)
        {
            var item = _listOfActions[connection.GetHashCode()];
            var dataInjectionCommand = new DataInjectionDbCommand(command);
            item?.Invoke(dataInjectionCommand);
            return dataInjectionCommand;
        }

        #endregion
    }
}

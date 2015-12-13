using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Extension method to inject data in requests (for Unit Testing)
    /// </summary>
    public static class DataInjectionExtension
    {
        private static readonly Dictionary<int, Func<DataInjectionDbCommand, DataTable>> _listOfActions = new Dictionary<int, Func<DataInjectionDbCommand, DataTable>>();

        #region METHODS

        /// <summary>
        /// Set the function to execute when the DatabaseCommand class need to retrieve data,
        /// and bypass the Database Server request process.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        public static void DefineDataInjection(this DbConnection connection, Func<DataInjectionDbCommand, DataTable> action)
        {
            _listOfActions.Add(connection.GetHashCode(), action);
        }

        #endregion

        #region PRIVATES

        /// <summary>
        /// Returns the Function associated to this DbConnection to retrieve data.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal static Func<DataInjectionDbCommand, DataTable> GetRetrieveDataInjectionDataTable(this DbConnection connection)
        {
            return _listOfActions[connection.GetHashCode()];
        }

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
        internal static DataTable InvokeAndReturnData(this DbConnection connection, DbCommand command)
        {
            var item = _listOfActions[connection.GetHashCode()];
            return item.Invoke(new DataInjectionDbCommand(command));
        }

        #endregion
    }
}

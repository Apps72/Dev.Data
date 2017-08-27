using System;
using System.Data.Common;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Database command management
    /// </summary>
    public class DatabaseCommand : DatabaseCommandBase
    {
        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        public DatabaseCommand(DbConnection connection) : this(connection, null, -1)            
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="commandText">SQL query</param>
        public DatabaseCommand(DbConnection connection, string commandText) : this(connection, null, commandText, -1)
        {
            base.CommandText = new System.Text.StringBuilder(commandText);
        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        public DatabaseCommand(DbConnection connection, DbTransaction transaction) : this(connection, transaction, -1)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        public DatabaseCommand(DbConnection connection, DbTransaction transaction, string commandText) : this(connection, transaction, commandText, -1)
        {
            
        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public DatabaseCommand(DbConnection connection, DbTransaction transaction, int commandTimeout)
            : base(connection.CreateCommand(), transaction, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public DatabaseCommand(DbConnection connection, DbTransaction transaction, string commandText, int commandTimeout)
            : base(connection.CreateCommand(), transaction, commandTimeout)
        {
            base.CommandText = new System.Text.StringBuilder(commandText);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Base Interface to manage all DataBaseCommands
    /// </summary>
    [Obsolete("Use IDatabaseCommand interface.")]
    public partial interface IDatabaseCommandBase : IDatabaseCommand
    {

    }

    /// <summary>
    /// Base class with common methods to retrieve or manage data.
    /// </summary>
    [Obsolete("Use DatabaseCommand class.")]
    public partial class DatabaseCommandBase : DatabaseCommand
    {
        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        public DatabaseCommandBase(DbConnection connection) 
            : base(connection)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="commandText">SQL query</param>
        public DatabaseCommandBase(DbConnection connection, string commandText) 
            : base(connection, commandText)
        {
        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        public DatabaseCommandBase(DbConnection connection, DbTransaction transaction) 
            : base(connection, transaction)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        public DatabaseCommandBase(DbConnection connection, DbTransaction transaction, string commandText) 
            : base(connection, transaction, commandText)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public DatabaseCommandBase(DbConnection connection, DbTransaction transaction, int commandTimeout)
            : base(connection, transaction, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public DatabaseCommandBase(DbConnection connection, DbTransaction transaction, string commandText, int commandTimeout)
            : base(connection, transaction, commandText, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="command">Active command with predefined CommandText and Connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandTimeout">the wait time (in seconds) before terminating the attempt to execute a command and generating an error.</param>
        protected DatabaseCommandBase(DbCommand command, DbTransaction transaction, int commandTimeout)
            : base(command, transaction, commandTimeout)
        {

        }
    }
}

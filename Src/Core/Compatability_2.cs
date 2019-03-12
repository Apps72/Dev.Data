using System;
using System.Data.Common;

namespace Apps72.Dev.Data
{
    /// <summary />
    public partial class DatabaseCommand
    {
        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="commandText">SQL query</param>
        [Obsolete("Use DatabaseCommand(connection) constructor and set CommandText next.")]
        public DatabaseCommand(DbConnection connection, SqlString commandText)
            : this(connection?.CreateCommand(), null, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        [Obsolete("Use DatabaseCommand(connection) constructor and set CommandTimeout next.")]
        public DatabaseCommand(DbConnection connection, int commandTimeout)
            : this(connection?.CreateCommand(), null, null, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction">Active transaction</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        [Obsolete("Use DatabaseCommand(transaction) constructor and set CommandTimeout next.")]
        public DatabaseCommand(DbConnection connection, DbTransaction transaction, int commandTimeout)
            : this(transaction?.Connection?.CreateCommand(), transaction, null, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="transaction"/>
        /// </summary>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        [Obsolete("Use DatabaseCommand(transaction) constructor and set CommandText next.")]
        public DatabaseCommand(DbTransaction transaction, string commandText)
            : this(transaction?.Connection?.CreateCommand(), transaction, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="transaction"/>
        /// </summary>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        [Obsolete("Use DatabaseCommand(transaction) constructor and set CommandTimeout next.")]
        public DatabaseCommand(DbTransaction transaction, int commandTimeout)
            : this(transaction?.Connection?.CreateCommand(), transaction, null, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="transaction"/>
        /// </summary>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        [Obsolete("Use DatabaseCommand(transaction) constructor and set CommandText and CommandTimeout next.")]
        public DatabaseCommand(DbTransaction transaction, string commandText, int commandTimeout)
            : this(transaction?.Connection?.CreateCommand(), transaction, commandText, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="transaction"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        /// <param name="transaction"></param>
        [Obsolete("Use DatabaseCommand(transaction) constructor.")]
        public DatabaseCommand(DbConnection connection, DbTransaction transaction)
            : this(connection?.CreateCommand(), transaction, null, -1)
        {

        }

        /// <summary>
        /// Returns a Fluent Query tool to execute SQL request.
        /// </summary>
        [Obsolete("Use .Query(commandText) method.")]
        public FluentQuery Query()
        {
            return new FluentQuery(this);
        }

        /// <summary>
        /// Returns a Fluent Query tool to execute SQL request.
        /// </summary>
        /// <param name="commandText">Sql query</param>
        /// <param name="values">Paremeters</param>
        [Obsolete("Use .Query().ForSql(commandText).AddParameter(values) methods.")]
        public FluentQuery Query<T>(SqlString commandText, T values)
        {
            return new FluentQuery(this).ForSql(commandText).AddParameter(values);
        }
    }

    /// <summary>
    /// Base Interface to manage all DataBaseCommands
    /// </summary>
    public partial interface IDatabaseCommand
    {
        /// <summary>
        /// Returns a Fluent Query tool to execute SQL request.
        /// </summary>
        [Obsolete("Use .Query(commandText) method.")]
        FluentQuery Query();
    }
}

using Microsoft.Data.Sqlite;
using System;

namespace Apps72.Dev.Data
{
    public class SqliteDatabaseCommand : DatabaseCommandBase
    {
        private bool _mustAutoDisconnect = false;

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        public SqliteDatabaseCommand(SqliteConnection connection)
            : this(connection, String.Empty)
        {
        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="commandText">SQL query</param>
        public SqliteDatabaseCommand(SqliteConnection connection, string commandText)
            : this(connection, null, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        public SqliteDatabaseCommand(SqliteConnection connection, SqliteTransaction transaction)
            : this(connection, transaction, String.Empty, -1)
        {

        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        public SqliteDatabaseCommand(SqliteConnection connection, SqliteTransaction transaction, string commandText)
            : this(connection, transaction, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a temporary SQL Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        public SqliteDatabaseCommand(string connectionString)
            : this(connectionString, String.Empty)
        { }

        /// <summary>
        /// Create a command for a temporary SQL Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">SQL query</param>
        public SqliteDatabaseCommand(string connectionString, string commandText)
            : this(connectionString, commandText, -1)
        { }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public SqliteDatabaseCommand(SqliteConnection connection, SqliteTransaction transaction, string commandText, int commandTimeout)
            : base(new SqliteCommand(commandText, connection), transaction, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a temporary SQL Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public SqliteDatabaseCommand(string connectionString, string commandText, int commandTimeout)
            : base(null, null, commandTimeout)
        {
            this.Connection = new SqliteConnection(connectionString);
            this.Connection.Open();
            this.Command = new SqliteCommand(commandText, this.Connection as SqliteConnection);

            if (commandTimeout >= 0)
                this.Command.CommandTimeout = commandTimeout;

            _mustAutoDisconnect = true;
        }

        /// <summary>
        /// Gets or sets the current transaction
        /// </summary>
        public virtual new SqliteTransaction Transaction
        {
            get
            {
                return base.Transaction as SqliteTransaction;
            }
            set
            {
                base.Transaction = value;
            }
        }

        /// <summary>
        /// Gets sql parameters of the query
        /// </summary>
        public virtual new SqliteParameterCollection Parameters
        {
            get
            {
                return base.Parameters as SqliteParameterCollection;
            }
        }

        /// <summary>
        /// Gets the last raised exception 
        /// </summary>
        public virtual new SqliteException Exception
        {
            get
            {
                return base.Exception as SqliteException;
            }
        }

        /// <summary>
        /// Begin a transaction into the database
        /// </summary>
        /// <returns>Transaction</returns>
        public virtual new SqliteTransaction TransactionBegin()
        {
            return base.TransactionBegin() as SqliteTransaction;
        }

        /// <summary>
        /// Dispose the object and free ressources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_mustAutoDisconnect)
            {
                if (this.Connection.State != System.Data.ConnectionState.Closed)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                }
            }
        }
    }
}

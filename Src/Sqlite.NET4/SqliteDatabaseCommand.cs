using System;
using System.Data.SQLite;
using System.Data;

namespace Apps72.Dev.Data.Sqlite
{
    /// <summary>
    /// Database command management
    /// </summary>
    /// <example>
    /// <code>
    /// public class SqliteDatabaseCommand : DatabaseCommandBase&lt;OracleConnection, OracleCommand, OracleParameterCollection, OracleTransaction, OracleException&gt;
    /// {
    ///     public SqliteDatabaseCommand(OracleConnection connection) : base(connection) { }
    /// }
    /// </code>
    /// </example>
    public class SqliteDatabaseCommand : DatabaseCommandBase
    {
        private bool _mustAutoDisconnect = false;
        
        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        public SqliteDatabaseCommand(SQLiteConnection connection)
            : this(connection, String.Empty)
        {
        }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="commandText">SQL query</param>
        public SqliteDatabaseCommand(SQLiteConnection connection, string commandText)
            : this(connection, null, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        public SqliteDatabaseCommand(SQLiteConnection connection, SQLiteTransaction transaction)
            : this(connection, transaction, String.Empty, -1)
        {

        }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        public SqliteDatabaseCommand(SQLiteConnection connection, SQLiteTransaction transaction, string commandText)
            : this(connection, transaction, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a temporary Oracle Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the Oracle Server database.</param>
        public SqliteDatabaseCommand(string connectionString)
            : this(connectionString, String.Empty)
        { }

        /// <summary>
        /// Create a command for a temporary Oracle Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the Oracle Server database.</param>
        /// <param name="commandText">SQL query</param>
        public SqliteDatabaseCommand(string connectionString, string commandText)
            : this(connectionString, commandText, -1)
        { }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public SqliteDatabaseCommand(SQLiteConnection connection, SQLiteTransaction transaction, string commandText, int commandTimeout)
            : base(new SQLiteCommand(commandText, connection), transaction, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a temporary Oracle Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the Oracle Server database.</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public SqliteDatabaseCommand(string connectionString, string commandText, int commandTimeout)
            : base(null, null, commandTimeout)
        {
            this.Connection = new SQLiteConnection(connectionString);
            this.Connection.Open();
            this.CommandText = new System.Text.StringBuilder(commandText);
            this.Command = new SQLiteCommand(commandText, this.Connection as SQLiteConnection);            

            if (commandTimeout >= 0)
                this.Command.CommandTimeout = commandTimeout;

            _mustAutoDisconnect = true;
        }

        /// <summary>
        /// Gets or sets the current transaction
        /// </summary>
        public virtual new SQLiteTransaction Transaction
        {
            get
            {
                return base.Transaction as SQLiteTransaction;
            }
            set
            {
                base.Transaction = value;
            }
        }

        /// <summary>
        /// Gets sql parameters of the query
        /// </summary>
        public virtual new SQLiteParameterCollection Parameters
        {
            get
            {
                return base.Parameters as SQLiteParameterCollection;
            }
        }

        /// <summary>
        /// Gets the last raised exception 
        /// </summary>
        public virtual new SQLiteException Exception
        {
            get
            {
                return base.Exception as SQLiteException;
            }
        }

        /// <summary>
        /// Begin a transaction into the database
        /// </summary>
        /// <returns>Transaction</returns>
        public virtual new SQLiteTransaction TransactionBegin()
        {
            return base.TransactionBegin() as SQLiteTransaction;
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
                if (this.Connection != null && this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                }
            }
        }

    }
}

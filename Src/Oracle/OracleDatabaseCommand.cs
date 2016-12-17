using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace Apps72.Dev.Data.Oracle
{
    /// <summary>
    /// Database command management
    /// </summary>
    /// <example>
    /// <code>
    /// public class OracleDatabaseCommand : DatabaseCommandBase&lt;OracleConnection, OracleCommand, OracleParameterCollection, OracleTransaction, OracleException&gt;
    /// {
    ///     public OracleDatabaseCommand(OracleConnection connection) : base(connection) { }
    /// }
    /// </code>
    /// </example>
    public class OracleDatabaseCommand : DatabaseCommandBase
    {
        private bool _mustAutoDisconnect = false;

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        public OracleDatabaseCommand(OracleConnection connection)
            : this(connection, String.Empty)
        {
        }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="commandText">SQL query</param>
        public OracleDatabaseCommand(OracleConnection connection, string commandText)
            : this(connection, null, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        public OracleDatabaseCommand(OracleConnection connection, OracleTransaction transaction)
            : this(connection, transaction, String.Empty, -1)
        {

        }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        public OracleDatabaseCommand(OracleConnection connection, OracleTransaction transaction, string commandText)
            : this(connection, transaction, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a temporary Oracle Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the Oracle Server database.</param>
        public OracleDatabaseCommand(string connectionString)
            : this(connectionString, String.Empty)
        { }

        /// <summary>
        /// Create a command for a temporary Oracle Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the Oracle Server database.</param>
        /// <param name="commandText">SQL query</param>
        public OracleDatabaseCommand(string connectionString, string commandText)
            : this(connectionString, commandText, -1)
        { }

        /// <summary>
        /// Create a command for a Oracle Server connection
        /// </summary>
        /// <param name="connection">Active Oracle Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public OracleDatabaseCommand(OracleConnection connection, OracleTransaction transaction, string commandText, int commandTimeout)
            : base(new OracleCommand(commandText, connection) { BindByName = true }, transaction, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a temporary Oracle Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the Oracle Server database.</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public OracleDatabaseCommand(string connectionString, string commandText, int commandTimeout)
            : base(null, null, commandTimeout)
        {
            this.Connection = new OracleConnection(connectionString);
            this.Connection.Open();
            this.CommandText = new System.Text.StringBuilder(commandText);
            this.Command = new OracleCommand(commandText, this.Connection as OracleConnection)
            {
                BindByName = true
            };

            if (commandTimeout >= 0)
                this.Command.CommandTimeout = commandTimeout;

            _mustAutoDisconnect = true;
        }

        /// <summary>
        /// Gets or sets the current transaction
        /// </summary>
        public virtual new OracleTransaction Transaction
        {
            get
            {
                return base.Transaction as OracleTransaction;
            }
            set
            {
                base.Transaction = value;
            }
        }

        /// <summary>
        /// Gets sql parameters of the query
        /// </summary>
        public virtual new OracleParameterCollection Parameters
        {
            get
            {
                return base.Parameters as OracleParameterCollection;
            }
        }

        /// <summary>
        /// Gets the last raised exception 
        /// </summary>
        public virtual new OracleException Exception
        {
            get
            {
                return base.Exception as OracleException;
            }
        }

        /// <summary>
        /// Begin a transaction into the database
        /// </summary>
        /// <returns>Transaction</returns>
        public virtual new OracleTransaction TransactionBegin()
        {
            return base.TransactionBegin() as OracleTransaction;
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
                if (this.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                    this.Connection.Dispose();
                }
            }
        }

    }
}
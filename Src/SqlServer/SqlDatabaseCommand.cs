using System;
using System.Data;
using System.Data.SqlClient;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Database command management
    /// </summary>
    /// <example>
    /// <code>
    /// public class SqlDatabaseCommand : DatabaseCommandBase&lt;SqlConnection, SqlCommand, SqlParameterCollection, SqlTransaction, SqlException&gt;
    /// {
    ///     public SqlDatabaseCommand(SqlConnection connection) : base(connection) { }
    /// }
    /// </code>
    /// </example>
    public class SqlDatabaseCommand : DatabaseCommand
    {
        private SqlDatabaseRetryExceptions _retryIfExceptionsOccured = null;
        private bool _mustAutoDisconnect = false;

        #region CONSTRUCTORS

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        public SqlDatabaseCommand(SqlConnection connection)
            : this(connection, String.Empty)
        {
            
        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="commandText">SQL query</param>
        public SqlDatabaseCommand(SqlConnection connection, string commandText)
            : this(connection, null, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        public SqlDatabaseCommand(SqlConnection connection, SqlTransaction transaction)
            : this(connection, transaction, String.Empty, -1)
        {

        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        public SqlDatabaseCommand(SqlConnection connection, SqlTransaction transaction, string commandText)
            : this(connection, transaction, commandText, -1)
        {

        }

        /// <summary>
        /// Create a command for a temporary SQL Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        public SqlDatabaseCommand(string connectionString)
            : this(connectionString, String.Empty)
        { }

        /// <summary>
        /// Create a command for a temporary SQL Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">SQL query</param>
        public SqlDatabaseCommand(string connectionString, string commandText)
            : this(connectionString, commandText, -1)
        { }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="connection">Active SQL Server connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public SqlDatabaseCommand(SqlConnection connection, SqlTransaction transaction, string commandText, int commandTimeout)
            : base(new SqlCommand(commandText, connection), transaction, commandTimeout)
        {

        }

        /// <summary>
        /// Create a command for a temporary SQL Server connection, when given a string that contains the connection string
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">SQL query</param>
        /// <param name="commandTimeout">Maximum timeout of the queries</param>
        public SqlDatabaseCommand(string connectionString, string commandText, int commandTimeout)
            : base(null, null, commandTimeout)
        {
            this.Connection = new SqlConnection(connectionString);
            this.Connection.Open();
            this.CommandText = new System.Text.StringBuilder(commandText);
            this.Command = new SqlCommand(commandText, this.Connection as SqlConnection);

            if (commandTimeout >= 0)
                this.Command.CommandTimeout = commandTimeout;

            _mustAutoDisconnect = true;
        }

        #endregion

        #region EXECUTES

        /// <summary>
        /// Execute query and return results by using a Datatable
        /// </summary>
        /// <returns>DataTable of results</returns>
        public override System.Data.DataTable ExecuteTable()
        {
            return this.ExecuteCommandOrRetryIfErrorOccured(() =>
            {
                return base.ExecuteTable();
            });
        }

        /// <summary>
        /// Execute the query and return an internal DataTable with all data.
        /// </summary>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        internal override Schema.DataTable ExecuteInternalDataTable(bool firstRowOnly)
        {
            return this.ExecuteCommandOrRetryIfErrorOccured(() =>
            {
                return base.ExecuteInternalDataTable(firstRowOnly);
            });
        }

        /// <summary>
        /// Execute the query and return the count of modified rows
        /// </summary>
        /// <returns>Count of modified rows</returns>
        public override int ExecuteNonQuery()
        {
            return this.ExecuteCommandOrRetryIfErrorOccured(() =>
            {
                return base.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Execute the query and return the first column of the first row of results
        /// </summary>
        /// <returns>Object - Result</returns>
        public override object ExecuteScalar()
        {
            return this.ExecuteCommandOrRetryIfErrorOccured(() =>
            {
                return base.ExecuteScalar();
            });
        }

        #endregion

        #region OTHERS

        /// <summary>
        /// Gets or sets the current transaction
        /// </summary>
        public virtual new SqlTransaction Transaction
        {
            get
            {
                return base.Transaction as SqlTransaction;
            }
            set
            {
                base.Transaction = value;
            }
        }

        /// <summary>
        /// Gets sql parameters of the query
        /// </summary>
        public virtual new SqlParameterCollection Parameters
        {
            get
            {
                return base.Parameters as SqlParameterCollection;
            }
        }

        /// <summary>
        /// Gets how to retry query executions if a SqlException occured.
        /// To use this feature, set the default error number to retry, via RetryIfExceptionsOccured.SetDeadLockCodes()
        /// By default, a maximum of 3 retries and a waiting time of 1 second between two retries, is set.
        /// </summary>
        public virtual SqlDatabaseRetryExceptions RetryIfExceptionsOccured
        {
            get
            {
                return _retryIfExceptionsOccured ?? (_retryIfExceptionsOccured = new SqlDatabaseRetryExceptions());
            }
        }

        /// <summary>
        /// Gets the last raised exception 
        /// </summary>
        public virtual new SqlException Exception
        {
            get
            {
                return base.Exception as SqlException;
            }
        }

        /// <summary>
        /// Begin a transaction into the database
        /// </summary>
        /// <returns>Transaction</returns>
        public virtual new SqlTransaction TransactionBegin()
        {
            return base.TransactionBegin() as SqlTransaction;
        }

        #endregion

        #region PRIVATES

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
                    try
                    {
                        this.Connection.Close();
                    }
                    finally
                    {
                        this.Connection.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Execute the specified method (ExecuteTable, ExecuteNonQuery or ExecuteScalar)
        /// And retry x times if asked by RetryIfExceptionsOccured property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        private T ExecuteCommandOrRetryIfErrorOccured<T>(Func<T> action)
        {
            if (this.RetryIfExceptionsOccured != null && this.RetryIfExceptionsOccured.IsDefined())
            {
                SqlException lastException = null;
                bool toRetry = false;

                do
                {
                    try
                    {
                        // Execute the query
                        T data = action.Invoke();

                        // Check if a unknown Exception has occurend and is ThrowException = false
                        if (this.Exception == null || this.RetryIfExceptionsOccured.IsNotAnExceptionToRetry(this.Exception))
                            return data;
                        else
                            lastException = this.Exception;
                    }
                    catch (SqlException ex)
                    {
                        // Check if a unknown Exception
                        if (this.RetryIfExceptionsOccured.IsNotAnExceptionToRetry(ex))
                            throw;
                        else
                            lastException = ex;
                    }

                    // Need to execute this command (action) again
                    toRetry = this.RetryIfExceptionsOccured.IsMustRetryAndWait();

                    // Trace the error occured
                    if (toRetry && this.Log != null)
                    {
                        this.Log.Invoke(String.Format("Retry activated. SqlException #{1} was: \"{0}\".", this.Exception.Message, this.RetryIfExceptionsOccured.RetryCount - 1));
                    }

                } while (toRetry);

                // If exeed the number of retries... So, throw this last exception
                throw lastException;
            }
            else
            {
                return action.Invoke();
            }
        }

#if SQL_CLR
        /// <summary>
        /// Gets the current context connection
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetContextConnection()
        {
            SqlConnection connection = new SqlConnection("context connection=true");
            connection.Open();
            return connection;
        }
#endif

        #endregion

    }
}

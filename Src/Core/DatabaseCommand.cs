﻿using System;
using System.Data.Common;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace Apps72.Dev.Data
{
    /// <summary>
    ///  Base class with common methods to retrieve or manage data.
    /// </summary>

    [DebuggerDisplay("{Command.CommandText}")]
    public partial class DatabaseCommand : IDatabaseCommand
    {
        private DatabaseRetry _retry = null;

        #region EVENTS

        /// <summary>
        /// Signature for ExceptionOccured event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ExceptionOccuredEventHandler(object sender, ExceptionOccuredEventArgs e);

        /// <summary>
        /// Event raised when an SQL Exception occured (in Execute Methods)
        /// </summary>
        public event ExceptionOccuredEventHandler ExceptionOccured;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Create a command for a specified <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">Active connection</param>
        public DatabaseCommand(DbConnection connection)
            : this(connection?.CreateCommand(), null, null, -1)
        {

        }

        /// <summary>
        /// Create a command for a specified <paramref name="transaction"/>
        /// </summary>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        public DatabaseCommand(DbTransaction transaction)
            : this(transaction?.Connection?.CreateCommand(), transaction, null, -1)
        {

        }

        /// <summary>
        /// Create a command for a SQL Server connection
        /// </summary>
        /// <param name="command">Active command with predefined CommandText and Connection</param>
        /// <param name="transaction">The transaction in which the SQL Query executes</param>
        /// <param name="commandText">The command text to execute</param>
        /// <param name="commandTimeout">the wait time (in seconds) before terminating the attempt to execute a command and generating an error.</param>
        protected DatabaseCommand(DbCommand command, DbTransaction transaction, SqlString commandText, int commandTimeout)
        {
            if (command == null)
                throw new ArgumentException("The DbCommand must be set. Check if your DbConnection or your DbTransaction is correctly defined.");

            if (transaction != null && command.Connection != transaction.Connection)
                throw new ArgumentException("Internal error: the DbTransaction and the DbCommand must be share the same DbConnection.");

            this.ThrowException = true;
            this.Command = command;
            this.Tags = new List<string>();
            this.Transaction = transaction;

            if (commandTimeout >= 0)
                this.Command.CommandTimeout = commandTimeout;

            this.CommandText = commandText ?? new SqlString();
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets the sql query
        /// </summary>
        public virtual SqlString CommandText { get; set; }

        /// <summary>
        /// Returns a Fluent Query tool to execute SQL request.
        /// </summary>
        /// <param name="commandText">SQL query command</param>
        public FluentQuery Query(SqlString commandText) => new FluentQuery(this).ForSql(commandText);

        /// <summary>
        /// Gets the last raised exception 
        /// </summary>
        public virtual DbException Exception { get; private set; }

        /// <summary>
        /// Gets or sets the command type
        /// </summary>
        public virtual CommandType CommandType
        {
            get
            {
                return this.Command.CommandType;
            }
            set
            {
                if (this.Command != null)
                    this.Command.CommandType = value;
            }
        }

        /// <summary>
        /// Gets or sets the wait time in seconds, before terminating the attempt to execute a command
        /// and generating an error.
        /// </summary>
        public virtual int CommandTimeout
        {
            get
            {
                return this.Command.CommandTimeout;
            }
            set
            {
                this.Command.CommandTimeout = value;
            }
        }

        /// <summary>
        /// Gets options for the automatic Retry process.
        /// </summary>
        public virtual DatabaseRetry Retry => _retry ?? (_retry = new DatabaseRetry(this));

        /// <summary>
        /// Gets or sets the current active connection
        /// </summary>
#if SQL_CLR
        public virtual DbConnection Connection { get; set; }
#else
        protected virtual DbConnection Connection { get { return Command.Connection; } }
#endif

        /// <summary>
        /// Gets the current DbCommand
        /// </summary>
        protected virtual DbCommand Command { get; private set; }

        /// <summary>
        /// Gets or sets the current transaction
        /// </summary>
        public virtual DbTransaction Transaction
        {
            get
            {
                return this.Command.Transaction;
            }
            set
            {
                this.Command.Transaction = value;
            }
        }

        /// <summary>
        /// Gets sql parameters of the query
        /// </summary>
        public virtual DbParameterCollection Parameters
        {
            get
            {
                return this.Command.Parameters;
            }
        }

        /// <summary>
        /// Gets a list of tags, used to annotate the SQL query (using SQL comments)
        /// </summary>
        public virtual List<string> Tags { get; private set; }

        /// <summary>
        /// Enable or disable the raise of exceptions when queries are executed.
        /// Default is True (Enabled).
        /// </summary>
        public virtual bool ThrowException { get; set; }

        /// <summary>
        /// Set this property to log the SQL generated by this class to the given delegate. 
        /// For example, to log to the console, set this property to Console.Write.
        /// </summary>
        public virtual Action<string> Log { get; set; }

        /// <summary>
        /// Set this property to execute an action immediately BEFORE the database request.
        /// </summary>
        public virtual Action<DatabaseCommand> ActionBeforeExecution { get; set; }

        /// <summary>
        /// Set this property to execute an action immediately AFTER the database request,
        /// and before the type convertions.
        /// </summary>
        public virtual Action<DatabaseCommand, IEnumerable<Schema.DataTable>> ActionAfterExecution { get; set; }

        /// <summary>
        /// Gets the <see cref="CommandText"/> where parameters are filled by values.
        /// </summary>
        //public virtual string FormattedCommandText => GetCommandTextFormatted(QueryFormat.Text);
        public virtual CommandTextFormatted Formatted
        {
            get
            {
                return new CommandTextFormatted(this);
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Annotate the SQL query with a tag (as a SQL comment)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual DatabaseCommand WithTag(string name)
        {
            if (name.Contains(Environment.NewLine))
            {
                foreach (var item in name.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    this.Tags.Add(item);
                }
            }
            else
                this.Tags.Add(name);

            return this;
        }

        /// <summary>
        /// Delete the CommandText and the all sql parameters
        /// </summary>
        public virtual DatabaseCommand Clear()
        {
            this.Tags.Clear();
            this.CommandText.Clear();
            this.Parameters.Clear();
            return this;
        }

        /// <summary>
        /// Prepare a query
        /// </summary>
        public virtual DatabaseCommand Prepare()
        {
            this.Command.CommandText = GetCommandTextWithTags();
            this.Command.Prepare();
            return this;
        }

        /// <summary>
        /// Begin a transaction into the database
        /// </summary>
        /// <returns>Transaction</returns>
        public virtual DbTransaction TransactionBegin()
        {
            if (this.Log != null)
                this.Log.Invoke("BEGIN TRANSACTION");

            this.Command.Transaction = this.Command.Connection.BeginTransaction();
            return this.Command.Transaction;
        }

        /// <summary>
        /// Commit the current transaction to the database
        /// </summary>
        public virtual void TransactionCommit()
        {
            if (this.Command != null && this.Command.Transaction != null)
            {
                if (this.Log != null)
                    this.Log.Invoke("COMMIT");

                this.Command.Transaction.Commit();
            }
        }

        /// <summary>
        /// Rollback the current transaction 
        /// </summary>
        public virtual void TransactionRollback()
        {
            if (this.Command != null && this.Command.Transaction != null)
            {
                if (this.Log != null)
                    this.Log.Invoke("ROLLBACK");

                this.Command.Transaction.Rollback();
            }
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>> ExecuteDataSet<T, U>()
        {
            var dataset = this.ExecuteInternalDataSet(firstRowOnly: false).ToArray();

            return new Tuple<IEnumerable<T>, IEnumerable<U>>
                (
                dataset.Length >= 0 ? dataset[0].ConvertTo<T>() : null,
                dataset.Length >= 1 ? dataset[1].ConvertTo<U>() : null
                );
            ;
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <typeparam name="V">Object type for third table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>> ExecuteDataSet<T, U, V>()
        {
            var dataset = this.ExecuteInternalDataSet(firstRowOnly: false).ToArray();

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>>
                (
                dataset.Length >= 0 ? dataset[0].ConvertTo<T>() : null,
                dataset.Length >= 1 ? dataset[1].ConvertTo<U>() : null,
                dataset.Length >= 2 ? dataset[2].ConvertTo<V>() : null
                );
            ;
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <typeparam name="V">Object type for third table</typeparam>
        /// <typeparam name="W">Object type for fourth table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>> ExecuteDataSet<T, U, V, W>()
        {
            var dataset = this.ExecuteInternalDataSet(firstRowOnly: false).ToArray();

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>>
                (
                dataset.Length >= 0 ? dataset[0].ConvertTo<T>() : null,
                dataset.Length >= 1 ? dataset[1].ConvertTo<U>() : null,
                dataset.Length >= 2 ? dataset[2].ConvertTo<V>() : null,
                dataset.Length >= 3 ? dataset[3].ConvertTo<W>() : null
                );
            ;
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <typeparam name="V">Object type for third table</typeparam>
        /// <typeparam name="W">Object type for fourth table</typeparam>
        /// <typeparam name="X">Object type for fifth table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>> ExecuteDataSet<T, U, V, W, X>()
        {
            var dataset = this.ExecuteInternalDataSet(firstRowOnly: false).ToArray();

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>>
                (
                dataset.Length >= 0 ? dataset[0].ConvertTo<T>() : null,
                dataset.Length >= 1 ? dataset[1].ConvertTo<U>() : null,
                dataset.Length >= 2 ? dataset[2].ConvertTo<V>() : null,
                dataset.Length >= 3 ? dataset[3].ConvertTo<W>() : null,
                dataset.Length >= 4 ? dataset[4].ConvertTo<X>() : null
                );
            ;
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>> ExecuteDataSet<T, U>(T typeOfItem1, U typeOfItem2)
        {
            return ExecuteDataSet<T, U>();
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <typeparam name="V">Object type for third table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>> ExecuteDataSet<T, U, V>(T typeOfItem1, U typeOfItem2, V typeOfItem3)
        {
            return ExecuteDataSet<T, U, V>();
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <typeparam name="V">Object type for third table</typeparam>
        /// <typeparam name="W">Object type for fourth table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>> ExecuteDataSet<T, U, V, W>(T typeOfItem1, U typeOfItem2, V typeOfItem3, W typeOfItem4)
        {
            return ExecuteDataSet<T, U, V, W>();
        }

        /// <summary>
        /// Execute the query and return a list or array of new instances of typed results filled with data table results.
        /// </summary>
        /// <typeparam name="T">Object type for first table</typeparam>
        /// <typeparam name="U">Object type for second table</typeparam>
        /// <typeparam name="V">Object type for third table</typeparam>
        /// <typeparam name="W">Object type for fourth table</typeparam>
        /// <typeparam name="X">Object type for fifth table</typeparam>
        /// <returns>List of array of typed results</returns>
        /// <example>
        /// <code>
        ///   var data = cmd.ExecuteDataSet&lt;Employee, Department&gt;();
        ///   var employees = data.Item1;
        ///   var departments = data.Item2;
        /// </code>
        /// </example>
        public virtual Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>> ExecuteDataSet<T, U, V, W, X>(T typeOfItem1, U typeOfItem2, V typeOfItem3, W typeOfItem4, X typeOfItem5)
        {
            return ExecuteDataSet<T, U, V, W, X>();
        }

        /// <summary>
        /// Execute the query and return an array of new instances of typed results filled with data table result.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>Array of typed results</returns>
        /// <example>
        /// <code>
        ///   Employee[] emp = cmd.ExecuteTable&lt;Employee&gt;();
        ///   var x = cmd.ExecuteTable&lt;Employee&gt;();
        /// </code>
        /// <remarks>
        ///   Result object property (ex. Employee.Name) may be tagged with the ColumnAttribute 
        ///   to set which column name (ex. [Column("Name")] must be associated to this property.
        /// </remarks>
        /// </example>
        public virtual IEnumerable<T> ExecuteTable<T>()
        {
            Schema.DataTable table = this.ExecuteInternalDataTable(firstRowOnly: false);
            return table?.ConvertTo<T>();
        }

        /// <summary>
        /// Execute the query and return an array of new instances of typed results filled with data table result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns>Array of typed results</returns>
        public virtual IEnumerable<T> ExecuteTable<T>(Func<Schema.DataRow, T> converter)
        {
            Schema.DataTable table = this.ExecuteInternalDataTable(firstRowOnly: false);
            int rowCount = table.Rows.Length;

            var results = new T[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                results[i] = converter.Invoke(table.Rows[i]);
            }

            return results;
        }

        /// <summary>
        /// Execute the query and return an array of new instances of typed results filled with data table result.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="itemOftype"></param>
        /// <returns>Array of typed results</returns>
        /// <example>
        /// <code>
        ///   Employee emp = new Employee();
        ///   var x = cmd.ExecuteTable(new { emp.Age, emp.Name });
        ///   var y = cmd.ExecuteTable(new { Age = 0, Name = "" });
        /// </code>
        /// <remarks>
        ///   Result object property (ex. Employee.Name) may be tagged with the ColumnAttribute 
        ///   to set which column name (ex. [Column("Name")] must be associated to this property.
        /// </remarks>
        /// </example>
        public virtual IEnumerable<T> ExecuteTable<T>(T itemOftype)
        {
            return ExecuteTable<T>();
        }

        /// <summary>
        /// Execute the query and return a new instance of T with the first row of results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>First row of results</returns>
        /// <example>
        /// <code>
        ///   Employee emp = cmd.ExecuteRow&lt;Employee&gt;();
        /// </code>
        /// <remarks>
        ///   Result object property (ex. Employee.Name) may be tagged with the ColumnAttribute 
        ///   to set which column name (ex. [Column("Name")] must be associated to this property.
        /// </remarks>
        /// </example>
        public virtual T ExecuteRow<T>()
        {
            return this.ExecuteRow<T>(default(T));
        }

        /// <summary>
        /// Execute the query and fill the specified T object with the first row of results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="itemOftype"></param>
        /// <returns>First row of results</returns>
        /// <example>
        /// <code>
        ///   Employee emp = new Employee();
        ///   var x = cmd.ExecuteRow(new { emp.Age, emp.Name });
        ///   var y = cmd.ExecuteRow(new { Age = 0, Name = "" });
        ///   var z = cmd.ExecuteRow(emp);
        /// </code>
        /// <remarks>
        ///   Result object property (ex. Employee.Name) may be tagged with the ColumnAttribute 
        ///   to set which column name (ex. [Column("Name")] must be associated to this property.
        /// </remarks>
        /// </example>
        public virtual T ExecuteRow<T>(T itemOftype)
        {
            if (Convertor.TypeExtension.IsPrimitive(typeof(T)))
            {
                return this.ExecuteScalar<T>();
            }
            else
            {
                Schema.DataTable table = this.ExecuteInternalDataTable(firstRowOnly: true);
                if (table != null && table.Rows.Length > 0)
                    return table.Rows[0].ConvertTo<T>(itemOftype);
                else
                    return default(T);
            }
        }

        /// <summary>
        /// Execute the query and fill the specified T object with the first row of results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="converter"></param>
        /// <returns>First row of results</returns>
        public virtual T ExecuteRow<T>(Func<Schema.DataRow, T> converter)
        {
            if (Convertor.TypeExtension.IsPrimitive(typeof(T)))
            {
                return this.ExecuteScalar<T>();
            }
            else
            {
                Schema.DataTable table = this.ExecuteInternalDataTable(firstRowOnly: true);
                if (table != null && table.Rows.Length > 0)
                    return converter.Invoke(table.Rows[0]);
                else
                    return default(T);
            }
        }

        /// <summary>
        /// Execute the query and return the count of modified rows
        /// </summary>
        /// <returns>Count of modified rows</returns>
        public virtual int ExecuteNonQuery()
        {
            ResetException();

            try
            {
                Update_CommandDotCommandText_If_CommandText_IsNew();

                // Action Before Execution
                if (this.ActionBeforeExecution != null)
                {
                    this.ActionBeforeExecution.Invoke(this);
                    Update_CommandDotCommandText_If_CommandText_IsNew();
                }

                // Log
                if (this.Log != null)
                    this.Log.Invoke(this.Command.CommandText);

                // Send the request to the Database server
                int rowsAffected = 0;

                if (this.Command.CommandText.Length > 0)
                {
                    if (Retry.IsActivated())
                        rowsAffected = Retry.ExecuteCommandOrRetryIfErrorOccured(() => this.Command.ExecuteNonQuery());
                    else
                        rowsAffected = this.Command.ExecuteNonQuery();
                }

                // Action After Execution
                if (this.ActionAfterExecution != null)
                {
                    var tables = new Schema.DataTable[]
                    {
                        new Schema.DataTable("ExecuteNonQuery", "Result", rowsAffected)
                    };
                    this.ActionAfterExecution.Invoke(this, tables);
                    int? newValue = tables[0].Rows[0][0] as int?;
                    rowsAffected = newValue.HasValue ? newValue.Value : 0;
                }

                return rowsAffected;
            }
            catch (DbException ex)
            {
                return ThrowSqlExceptionOrDefaultValue<int>(ex);
            }

        }

        /// <summary>
        /// Execute the query and return the first column of the first row of results
        /// </summary>
        /// <returns>Object - Result</returns>
        public virtual object ExecuteScalar()
        {
            ResetException();

            try
            {

                Update_CommandDotCommandText_If_CommandText_IsNew();

                // Action Before Execution
                if (this.ActionBeforeExecution != null)
                {
                    this.ActionBeforeExecution.Invoke(this);
                    Update_CommandDotCommandText_If_CommandText_IsNew();
                }

                // Log
                if (this.Log != null)
                    this.Log.Invoke(this.Command.CommandText);

                // Send the request to the Database server
                object result = null;

                if (this.Command.CommandText.Length > 0)
                {
                    if (Retry.IsActivated())
                        result = Retry.ExecuteCommandOrRetryIfErrorOccured(() => this.Command.ExecuteScalar());
                    else
                        result = this.Command.ExecuteScalar();
                }

                // Action After Execution
                if (this.ActionAfterExecution != null)
                {
                    var tables = new Schema.DataTable[]
                    {
                        new Schema.DataTable("ExecuteNonQuery", "Result", result)
                    };
                    this.ActionAfterExecution.Invoke(this, tables);
                    result = tables[0].Rows[0][0];
                }

                return result;
            }
            catch (DbException ex)
            {
                return ThrowSqlExceptionOrDefaultValue<object>(ex);
            }

        }

        /// <summary>
        /// Execute the query and return the first column of the first row of results
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <returns>Result</returns>
        public virtual T ExecuteScalar<T>()
        {
            object scalar = this.ExecuteScalar();

            if (scalar == null || scalar == DBNull.Value)
                return default(T);
            else
                return (T)scalar;

        }

        /// <summary>
        /// Adds a value to the end of the <see cref="DatabaseCommand.Parameters"/> property.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to be added. Null value will be replaced by System.DBNull.Value.</param>
        /// <param name="type">Type of parameter.</param>
        /// <param name="size">Size of parameter</param>
        /// <returns></returns>
        public virtual DatabaseCommand AddParameter(string name, object value, DbType? type, int? size)
        {
            var dbCommand = this.Command;
            var param = dbCommand.CreateParameter();

            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            if (type.HasValue) param.DbType = type.Value;
            if (size.HasValue && size > 0) param.Size = size.Value;

            dbCommand.Parameters.Add(param);

            return this;
        }


        /// <summary>
        /// Adds a value to the end of the <see cref="DatabaseCommand.Parameters"/> property.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to be added. Null value will be replaced by System.DBNull.Value.</param>
        /// <returns></returns>
        public virtual DatabaseCommand AddParameter(string name, object value)
        {
            return AddParameter(name, value, null, null);
        }

        /// <summary>
        /// Adds a value to the end of the <see cref="DbCommand.Parameters"/> property.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to be added. Null value will be replaced by System.DBNull.Value.</param>
        /// <param name="type">Type of parameter.</param>
        /// <returns></returns>
        public virtual DatabaseCommand AddParameter(string name, object value, DbType type)
        {
            return AddParameter(name, value, type, null);
        }

        /// <summary>
        /// Add all properties / values to the end of the <see cref="DbCommand.Parameters"/> property.
        /// If a property is already exist in Parameters collection, the parameter is removed and new added with new value.
        /// </summary>
        /// <param name="values">Object or anonymous object to convert all properties to parameters</param>
        public virtual DatabaseCommand AddParameter<T>(T values)
        {
            Schema.DataParameter.AddValues<T>(this.Command, values);
            return this;
        }

        /// <summary>
        /// Raises the ExceptionOccured event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnExceptionOccured(ExceptionOccuredEventArgs e)
        {
            if (this.ExceptionOccured != null)
            {
                ExceptionOccured(this, e);
            }
        }

        /// <summary>
        /// Dispose the object and free ressources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose the object and free ressources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            if (this.Command != null)
                this.Command.Dispose();
        }

#if !SQL_CLR

        /// <summary>
        /// Dispose the object and free ressources
        /// </summary>
        ~DatabaseCommand()
        {
            Dispose(false);
        }

#endif

        #endregion

        #region PRIVATE

        /// <summary>
        /// Execute the query and return an internal DataTable with all data.
        /// </summary>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        internal virtual IEnumerable<Schema.DataTable> ExecuteInternalDataSet(bool firstRowOnly)
        {
            ResetException();

            try
            {
                Update_CommandDotCommandText_If_CommandText_IsNew();

                // Action Before Execution
                if (this.ActionBeforeExecution != null)
                {
                    this.ActionBeforeExecution.Invoke(this);
                    Update_CommandDotCommandText_If_CommandText_IsNew();
                }

                // Log
                if (this.Log != null)
                    this.Log.Invoke(this.Command.CommandText);

                var tables = new List<Schema.DataTable>();

                // Send the request to the Database server
                if (this.Command.CommandText.Length > 0)
                {
                    Retry.ExecuteCommandOrRetryIfErrorOccured<bool>(() =>
                    {
                        using (DbDataReader dr = this.Command.ExecuteReader())
                        {
                            do
                            {
                                tables.Add(new Schema.DataTable(dr, firstRowOnly));
                            }
                            while (!firstRowOnly && dr.NextResult());
                        }
                        return true;
                    });
                }
                else
                {
                    tables.Add(new Schema.DataTable());
                }

                // Action After Execution
                if (this.ActionAfterExecution != null)
                {
                    this.ActionAfterExecution.Invoke(this, tables);
                }

                return tables.ToArray();
            }
            catch (DbException ex)
            {
                return ThrowSqlExceptionOrDefaultValue<IEnumerable<Schema.DataTable>>(ex);
            }

        }

        /// <summary>
        /// Execute the query and return an internal DataTable with all data.
        /// </summary>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        internal virtual Schema.DataTable ExecuteInternalDataTable(bool firstRowOnly)
        {
            return this.ExecuteInternalDataSet(firstRowOnly)?.FirstOrDefault();
        }

        /// <summary>
        /// Set the last raised exception to null
        /// </summary>
        protected virtual void ResetException()
        {
            this.Exception = null;
        }

        /// <summary>
        /// Raise the Exception if the ThrowException property is set to TRUE
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="ex">Exception</param>
        /// <returns></returns>
        protected virtual T ThrowSqlExceptionOrDefaultValue<T>(DbException ex)
        {
            this.Exception = ex;

            OnExceptionOccured(new ExceptionOccuredEventArgs() { Exception = this.Exception });

            if (ex != null)
            {
                if (this.ThrowException) throw ex;
            }

            return default(T);
        }

        /// <summary>
        /// Check if the this.CommandText is different of Command.CommandText and updated it.
        /// </summary>
        /// <returns></returns>
        private string Update_CommandDotCommandText_If_CommandText_IsNew()
        {
            string sql = GetCommandTextWithTags();

            if (String.CompareOrdinal(sql, this.Command.CommandText) != 0)
                this.Command.CommandText = sql;

            return this.Command.CommandText;
        }

        /// <summary>
        /// Returns the complete CommandText, including Tags in comments.
        /// </summary>
        /// <returns></returns>
        internal string GetCommandTextWithTags()
        {
            return $"{GetTagsAsSqlComments()}{this.CommandText.Value}";
        }

        /// <summary>
        /// Returns the tags formatted in SQL comments
        /// </summary>
        /// <returns></returns>
        internal string GetTagsAsSqlComments()
        {
            var sql = new StringBuilder();

            // Add tags
            foreach (var tag in this.Tags)
            {
                string[] splittedTag = tag.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var item in splittedTag)
                {
                    sql.AppendLine($"-- {item}");
                }
            }

            return sql.ToString();
        }

        /// <summary/>
        internal DbCommand GetInternalCommand()
        {
            return this.Command;
        }

        #endregion

        #region Interface

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IDatabaseCommand IDatabaseCommand.Clear()
        {
            return Clear();
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IDatabaseCommand IDatabaseCommand.Prepare()
        {
            return Prepare();
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Action<IDatabaseCommand> IDatabaseCommand.ActionBeforeExecution
        {
            get
            {
                return (Action<IDatabaseCommand>)ActionBeforeExecution;
            }
            set
            {
                ActionBeforeExecution = value;
            }
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        Action<IDatabaseCommand, IEnumerable<Schema.DataTable>> IDatabaseCommand.ActionAfterExecution
        {
            get
            {
                return (Action<IDatabaseCommand, IEnumerable<Schema.DataTable>>)ActionAfterExecution;
            }
            set
            {
                ActionAfterExecution = value;
            }
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IDatabaseCommand IDatabaseCommand.AddParameter(string name, object value)
        {
            return AddParameter(name, value);
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IDatabaseCommand IDatabaseCommand.AddParameter(string name, object value, DbType type)
        {
            return AddParameter(name, value, type);
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IDatabaseCommand IDatabaseCommand.AddParameter(string name, object value, DbType? type, int? size)
        {
            return AddParameter(name, value, type, size);
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IDatabaseCommand IDatabaseCommand.AddParameter<T>(T values)
        {
            return AddParameter<T>(values);
        }

        /// <summary />
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        IDatabaseCommand IDatabaseCommand.WithTag(string name)
        {
            return WithTag(name);
        }

        #endregion
    }
}

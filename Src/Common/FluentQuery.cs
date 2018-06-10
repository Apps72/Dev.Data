using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Common class to manage Fluent queries
    /// </summary>
    public partial class FluentQuery
    {
        private DatabaseCommandBase _databaseCommand;

        /// <summary>
        /// Creates a new instance of FluentQuery
        /// </summary>
        /// <param name="databaseCommand"></param>
        protected internal FluentQuery(DatabaseCommandBase databaseCommand)
        {
            _databaseCommand = databaseCommand;
        }

        /// <summary>
        /// Includes an active transaction to the current query.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public virtual FluentQuery WithTransaction(DbTransaction transaction)
        {
            _databaseCommand.Transaction = transaction;
            return this;
        }

        /// <summary>
        /// Add a SQL Query to the current command.
        /// </summary>
        /// <param name="sqlQuery">SQL query command</param>
        /// <returns></returns>
        public virtual FluentQuery ForSql(string sqlQuery)
        {
            _databaseCommand.CommandText = new StringBuilder(sqlQuery);
            return this;
        }

        /// <summary>
        /// Add a new parameter to the current query.
        /// </summary>
        /// <typeparam name="T">Type of this parameter</typeparam>
        /// <param name="name">Name of this parameter</param>
        /// <param name="value">Value of this parameter</param>
        /// <returns></returns>
        public virtual FluentQuery AddParameter<T>(string name, T value)
        {
            _databaseCommand.AddParameter(name, value);
            return this;
        }

        /// <summary>
        /// Add a new parameter to the current query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of this parameter</param>
        /// <param name="value">Value of this paramater</param>
        /// <param name="type">Type of this parameter</param>
        /// <returns></returns>
        public virtual FluentQuery AddParameter<T>(string name, T value, DbType type)
        {
            _databaseCommand.AddParameter(name, value, type);
            return this;
        }

        /// <summary>
        /// Add a new parameter to the current query.
        /// </summary>
        /// <typeparam name="T">Type of this parameter</typeparam>
        /// <param name="values">Object contains properties to define parameter names and values</param>
        /// <returns></returns>
        public virtual FluentQuery AddParameter<T>(T values)
        {
            _databaseCommand.AddParameter(values);
            return this;
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
            return _databaseCommand.ExecuteRow<T>();
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
            return _databaseCommand.ExecuteRow<T>(itemOftype);
        }

        /// <summary>
        /// Execute the query and return the first column of the first row of results
        /// </summary>
        /// <returns>Object - Result</returns>
        public virtual object ExecuteScalar()
        {
            return _databaseCommand.ExecuteScalar();
        }

        /// <summary>
        /// Execute the query and return the first column of the first row of results
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <returns>Result</returns>
        public virtual T ExecuteScalar<T>()
        {
            return _databaseCommand.ExecuteScalar<T>();
        }
    }
}

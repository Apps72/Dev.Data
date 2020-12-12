using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Apps72.Dev.Data
{
    public partial class DatabaseCommand
    {
        /// <summary>
        /// Execute the query and return a <see cref="System.Data.DataSet"/> object filled with data table results.
        /// </summary>
        /// <returns>Classic <see cref="System.Data.DataSet"/> object.</returns>
        public async virtual Task<System.Data.DataSet> ExecuteDataSetAsync()
        {
            throw new NotImplementedException();
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>>> ExecuteDataSetAsync<T, U>()
        {
            throw new NotImplementedException();
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>>> ExecuteDataSetAsync<T, U>(T typeOfItem1, U typeOfItem2)
        {
            throw new NotImplementedException();
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>>> ExecuteDataSetAsync<T, U, V>()
        {
            throw new NotImplementedException();
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>>> ExecuteDataSetAsync<T, U, V>(T typeOfItem1, U typeOfItem2, V typeOfItem3)
        {
            throw new NotImplementedException();
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>>> ExecuteDataSetAsync<T, U, V, W>()
        {
            throw new NotImplementedException();
        }

        // <summary>
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>>> ExecuteDataSetAsync<T, U, V, W>(T typeOfItem1, U typeOfItem2, V typeOfItem3, W typeOfItem4)
        {
            throw new NotImplementedException();
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>>> ExecuteDataSetAsync<T, U, V, W, X>()
        {
            throw new NotImplementedException();
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
        public async virtual Task<Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>>> ExecuteDataSetAsync<T, U, V, W, X>(T typeOfItem1, U typeOfItem2, V typeOfItem3, W typeOfItem4, X typeOfItem5)
        {
            throw new NotImplementedException();
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
        public async virtual Task<IEnumerable<T>> ExecuteTableAsync<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute the query and return an array of new instances of typed results filled with data table result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns>Array of typed results</returns>
        public async virtual Task<IEnumerable<T>> ExecuteTableAsync<T>(Func<Schema.DataRow, T> converter)
        {
            throw new NotImplementedException();
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
        public async virtual Task<IEnumerable<T>> ExecuteTableAsync<T>(T itemOftype)
        {
            throw new NotImplementedException();
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
        public async virtual Task<T> ExecuteRowAsync<T>()
        {
            throw new NotImplementedException();
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
        public async virtual Task<T> ExecuteRowAsync<T>(T itemOftype)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute the query and fill the specified T object with the first row of results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="converter"></param>
        /// <returns>First row of results</returns>
        public async virtual Task<T> ExecuteRowAsync<T>(Func<Schema.DataRow, T> converter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute the query and return the count of modified rows
        /// </summary>
        /// <returns>Count of modified rows</returns>
        public async virtual Task<int> ExecuteNonQueryAsync()
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

                // Replace null parameters by DBNull value.
                this.Replace_ParametersNull_By_DBNull();

                // Log
                if (this.Log != null)
                    this.Log.Invoke(this.Command.CommandText);

                // Send the request to the Database server
                int rowsAffected = 0;

                if (this.Command.CommandText.Length > 0)
                {
                    if (Retry.IsActivated())
                        rowsAffected = await Retry.ExecuteCommandOrRetryIfErrorOccuredAsync(async () => await this.Command.ExecuteNonQueryAsync());
                    else
                        rowsAffected = await this.Command.ExecuteNonQueryAsync();
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
        public async virtual Task<object> ExecuteScalarAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute the query and return the first column of the first row of results
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <returns>Result</returns>
        public async virtual Task<T> ExecuteScalarAsync<T>()
        {
            object scalar = await this.ExecuteScalarAsync();

            if (scalar == null || scalar == DBNull.Value)
                return default(T);
            else
                return (T)scalar;
        }
    }
}

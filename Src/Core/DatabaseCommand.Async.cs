using Apps72.Dev.Data.Convertor;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
            return await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync(System.Data.CommandBehavior.KeyInfo))
                {
                    return DataReaderConvertor.ToSystemDataSet(dr);
                }
            });
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, NoType, NoType, NoType>(dr);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>>
            (
                datasets.Item1,
                datasets.Item2
            );
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, NoType, NoType, NoType>(dr, forAnonymousTypes: true);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>>
            (
                datasets.Item1,
                datasets.Item2
            );
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, V, NoType, NoType>(dr);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>>
            (
                datasets.Item1,
                datasets.Item2,
                datasets.Item3
            );
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, V, NoType, NoType>(dr, forAnonymousTypes: true);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>>
            (
                datasets.Item1,
                datasets.Item2,
                datasets.Item3
            );
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, V, W, NoType>(dr);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>>
            (
                datasets.Item1,
                datasets.Item2,
                datasets.Item3,
                datasets.Item4
            );
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, V, W, NoType>(dr, forAnonymousTypes: true);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>>
            (
                datasets.Item1,
                datasets.Item2,
                datasets.Item3,
                datasets.Item4
            );
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, V, W, X>(dr);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>>
            (
                datasets.Item1,
                datasets.Item2,
                datasets.Item3,
                datasets.Item4,
                datasets.Item5
            );
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
            var datasets = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToMultipleTypesAsync<T, U, V, W, X>(dr, forAnonymousTypes: true);
                }
            });

            return new Tuple<IEnumerable<T>, IEnumerable<U>, IEnumerable<V>, IEnumerable<W>, IEnumerable<X>>
            (
                datasets.Item1,
                datasets.Item2,
                datasets.Item3,
                datasets.Item4,
                datasets.Item5
            );
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
            return await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    // Primitive type: Executable<string>()
                    if (TypeExtension.IsPrimitive(typeof(T)))
                        return await DataReaderConvertor.ToPrimitivesAsync<T>(dr);

                    // Dynamic type: Executable<dynamic>()
                    else if (DynamicConvertor.IsDynamic(typeof(T)))
                        return await  DataReaderConvertor.ToDynamicAsync<T>(dr);

                    // Object type: Executable<Employee>()
                    else
                        return (await DataReaderConvertor.ToTypeAsync<T>(dr)).Rows;
                }
            });
        }

        /// <summary>
        /// Execute the query and return an array of new instances of typed results filled with data table result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns>Array of typed results</returns>
        public async virtual Task<IEnumerable<T>> ExecuteTableAsync<T>(Func<Schema.DataRow, T> converter)
        {
            var table = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToDataTableAsync(dr);
                }
            });

            if (table != null && table.Rows != null)
                return table.Rows.Select(row => converter.Invoke(row));
            else
                return new T[] { };
        }

        /// <summary>
        /// Execute the query and return an array of new instances of typed results filled with data table result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns>Array of typed results</returns>
        public async virtual Task<IEnumerable<T>> ExecuteTableAsync<T>(Func<Schema.DataRow, Task<T>> converter)
        {
            var table = await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    return await DataReaderConvertor.ToDataTableAsync(dr);
                }
            });

            if (table != null && table.Rows != null)
                return table.Rows.Select(async row => await converter.Invoke(row)).Select(i => i.GetAwaiter().GetResult());
            else
                return new T[] { };
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
            return await ExecuteInternalCommand(async () =>
            {
                using (DbDataReader dr = await this.Command.ExecuteReaderAsync())
                {
                    if (TypeExtension.IsPrimitive(typeof(T)))
                        return await DataReaderConvertor.ToPrimitivesAsync<T>(dr);
                    else
                        return await DataReaderConvertor.ToAnonymousAsync<T>(dr);
                }
            });
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
            // Primitive type: Executable<string>()
            if (TypeExtension.IsPrimitive(typeof(T)))
            {
                return await this.ExecuteScalarAsync<T>();
            }
            else
            {
                // Get DataTable
                var rows = await ExecuteInternalCommand(async () =>
                {
                    using (DbDataReader dr = await this.Command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow))
                    {
                        // Dynamic type: Executable<dynamic>()
                        if (DynamicConvertor.IsDynamic(typeof(T)))
                            return await DataReaderConvertor.ToDynamicAsync<T>(dr);

                        // Object type: Executable<Employee>()
                        else
                            return (await DataReaderConvertor.ToTypeAsync<T>(dr)).Rows;
                    }
                });

                // Return
                return rows?.Any() == true ? rows.First() : default(T);
            }
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
            if (TypeExtension.IsPrimitive(typeof(T)))
            {
                return await this.ExecuteScalarAsync<T>();
            }
            else
            {
                // Get DataTable
                var rows = await ExecuteInternalCommand(async () =>
                {
                    using (DbDataReader dr = await this.Command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow))
                    {
                        return await DataReaderConvertor.ToAnonymousAsync<T>(dr);
                    }
                });

                // Return
                return rows?.Any() == true ? rows.First() : default(T);
            }
        }

        /// <summary>
        /// Execute the query and fill the specified T object with the first row of results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="converter"></param>
        /// <returns>First row of results</returns>
        public async virtual Task<T> ExecuteRowAsync<T>(Func<Schema.DataRow, T> converter)
        {
            if (TypeExtension.IsPrimitive(typeof(T)))
            {
                return await this.ExecuteScalarAsync<T>();
            }
            else
            {
                // Get DataRow
                var table = await ExecuteInternalCommand(async () =>
                {
                    using (DbDataReader dr = await this.Command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow))
                    {
                        return await DataReaderConvertor.ToDataTableAsync(dr);
                    }
                });
                var row = table?.Rows?.FirstOrDefault();

                // Return
                return row != null ? converter.Invoke(row) : default(T);
            }
        }

        /// <summary>
        /// Execute the query and fill the specified T object with the first row of results
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="converter"></param>
        /// <returns>First row of results</returns>
        public async virtual Task<T> ExecuteRowAsync<T>(Func<Schema.DataRow, Task<T>> converter)
        {
            if (TypeExtension.IsPrimitive(typeof(T)))
            {
                return await this.ExecuteScalarAsync<T>();
            }
            else
            {
                // Get DataRow
                var table = await ExecuteInternalCommand(async () =>
                {
                    using (DbDataReader dr = await this.Command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow))
                    {
                        return await DataReaderConvertor.ToDataTableAsync(dr);
                    }
                });
                var row = table?.Rows?.FirstOrDefault();

                // Return
                return row != null ? await converter.Invoke(row) : default(T);
            }
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
                // Commom operations before execution
                this.OperationsBeforeExecution();

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
            ResetException();

            try
            {
                // Commom operations before execution
                this.OperationsBeforeExecution();

                // Send the request to the Database server
                object result = null;

                if (this.Command.CommandText.Length > 0)
                {
                    if (Retry.IsActivated())
                        result = Retry.ExecuteCommandOrRetryIfErrorOccured(async () => await this.Command.ExecuteScalarAsync());
                    else
                        result = await this.Command.ExecuteScalarAsync();
                }

                // Action After Execution
                if (this.ActionAfterExecution != null)
                {
                    var tables = new Schema.DataTable[]
                    {
                        new Schema.DataTable("ExecuteScalar", "Result", result)
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

﻿using Apps72.Dev.Data.Convertor;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Apps72.Dev.Data
{
    public partial class DatabaseCommand
    {
        internal static readonly System.Data.CommandBehavior QUERY_COMMAND_BEHAVIOR = System.Data.CommandBehavior.Default;

        /// <summary />
        private T ExecuteInternalCommand<T>(Func<T> action)
        {
            ResetException();

            try
            {
                // Commom operations before execution
                this.OperationsBeforeExecution();

                // Send the request to the Database server
                T result = action.Invoke();

                // Action After Execution
                if (HasActionAfterExecutionToRaise<T>())
                {
                    var tables = DataTableConvertor.ToDataTable(result);
                    this.ActionAfterExecution.Invoke(this, tables);
                }

                return result;
            }
            catch (DbException ex)
            {
                return ThrowSqlExceptionOrDefaultValue<T>(ex);
            }
        }

        /// <summary />
        private async Task<T> ExecuteInternalCommandAsync<T>(Func<Task<T>> action)
        {
            ResetException();

            try
            {
                // Commom operations before execution
                this.OperationsBeforeExecution();

                // Send the request to the Database server
                T result = await action.Invoke();

                // Action After Execution (not yet for Dataset)
                if (HasActionAfterExecutionToRaise<T>())
                {
                    var tables = DataTableConvertor.ToDataTable(result);
                    this.ActionAfterExecution.Invoke(this, tables);
                }

                return result;
            }
            catch (DbException ex)
            {
                return ThrowSqlExceptionOrDefaultValue<T>(ex);
            }
        }

        /// <summary />
        private bool HasActionAfterExecutionToRaise<T>() => this.ActionAfterExecution != null &&
                                                            typeof(T) != typeof(System.Data.DataSet);

        /// <summary />
        private void OperationsBeforeExecution()
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
        }

        /// <summary>
        /// Check if the this.CommandText is different of Command.CommandText and updated it.
        /// </summary>
        /// <returns></returns>
        private string Update_CommandDotCommandText_If_CommandText_IsNew()
        {
            string sql = GetCommandTextWithTags();

            if (String.CompareOrdinal(sql, this.Command.CommandText) != 0)
            {
                this.Command.CommandText = sql;
            }

            return this.Command.CommandText;
        }

        /// <summary>
        /// Check if the this.CommandText is different of Command.CommandText and updated it.
        /// </summary>
        /// <returns></returns>
        private void Replace_ParametersNull_By_DBNull()
        {
            foreach (DbParameter parameter in this.Command.Parameters)
            {
                if (parameter.Value == null)
                    parameter.Value = DBNull.Value;
            }
        }

    }
}

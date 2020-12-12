using System;
using System.Data.Common;

namespace Apps72.Dev.Data
{
    public partial class DatabaseCommand
    {
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

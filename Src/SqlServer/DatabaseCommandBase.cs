using System;
using System.Collections.Generic;
using System.Linq;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Base class with common methods to retrieve or manage data.
    /// </summary>
    public partial class DatabaseCommand
    {
        /// <summary>
        /// Execute query and return results by using a Datatable
        /// </summary>
        /// <returns>DataTable of results</returns>
        public virtual System.Data.DataTable ExecuteTable()
        {
            ResetException();

            try
            {
                System.Data.DataTable data = new System.Data.DataTable();

                string sql = this.CommandText.ToString();
                if (String.CompareOrdinal(sql, this.Command.CommandText) != 0)
                    this.Command.CommandText = sql;

                if (this.Log != null)
                    this.Log.Invoke(this.Command.CommandText);

                // Send the request to the Database server
                if (this.Command.CommandText.Length > 0)
                {
                    using (System.Data.Common.DbDataReader dr = this.Command.ExecuteReader())
                    {
                        data.Load(dr);
                    }
                }

                return data;
            }
            catch (System.Data.Common.DbException ex)
            {
                return ThrowSqlExceptionOrDefaultValue<System.Data.DataTable>(ex);
            }

        }

        /// <summary>
        /// Execute the query and return the first row of results    
        /// </summary>
        /// <returns>First row of results</returns>
        public virtual System.Data.DataRow ExecuteRow()
        {
            System.Data.DataTable result = this.ExecuteTable();

            if (result == null)
                return null;

            if (result.Rows.Count > 0)
                return result.Rows[0];
            else
                return null;
        }
    }

}

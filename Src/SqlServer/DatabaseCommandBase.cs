using System;
using System.Collections.Generic;
using System.Linq;

namespace Apps72.Dev.Data
{
    public partial class DatabaseCommandBase
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

                // If UnitTest activated, invoke the "Get Method" to retrieve custom data
                if (this.Connection.ContainsDataInjectionDataTable())
                {
                    DataInjectionDbCommand command = this.Connection.InvokeAndReturnData(this);
                    return command.GetSystemDataTable();
                }
                else
                {
                    // Send the request to the Database server
                    using (System.Data.Common.DbDataReader dr = this.Command.ExecuteReader())
                    {
                        data.Load(dr);
                        return data;
                    }
                }
            }
            catch (System.Data.Common.DbException ex)
            {
                return ThrowSqlExceptionOrDefaultValue<System.Data.DataTable>(ex);
            }

        }

        /// <summary>
        /// Execute the query and return an array of new instances of typed results filled with data table result.
        /// </summary>
        /// <typeparam name="TReturn">Object type</typeparam>
        /// <param name="converter">Conveter method to return a typed object from DataRow</param>
        /// <returns>Array of typed results</returns>
        public virtual IEnumerable<TReturn> ExecuteTable<TReturn>(Func<System.Data.DataRow, TReturn> converter)
        {
            System.Data.DataTable table = this.ExecuteTable();

            TReturn[] results = new TReturn[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                results[i] = converter.Invoke(table.Rows[i]);
            }

            return results;
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

        /// <summary>
        /// Execute the query and fill the specified TReturn object with the first row of results
        /// </summary>
        /// <typeparam name="TReturn">Object type</typeparam>
        /// <param name="converter">Conveter method to return a typed object from DataRow</param>
        /// <returns>First row of results</returns>
        public virtual TReturn ExecuteRow<TReturn>(Func<System.Data.DataRow, TReturn> converter)
        {
            System.Data.DataRow row = this.ExecuteRow();
            return converter.Invoke(row);
        }

    }

}

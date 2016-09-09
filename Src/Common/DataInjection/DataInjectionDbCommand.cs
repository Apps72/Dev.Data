using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Main properties used with current DatabaseCommand request.
    /// </summary>
    public class DataInjectionDbCommand
    {
        private IEnumerable<object> _values = null;

        /// <summary>
        /// Initializes a new instance of DataInjection Database Command
        /// </summary>
        /// <param name="command"></param>
        internal DataInjectionDbCommand(DatabaseCommandBase command)
        {
            this.CommandText = command.CommandText.ToString();
            this.Parameters = command.Parameters.Cast<DbParameter>();
        }

        ///// <summary>
        ///// Get the CommandText used with this request.
        ///// </summary>
        public string CommandText { get; private set; }

        /// <summary>
        /// Get all parameters used with this request.
        /// </summary>
        public IEnumerable<DbParameter> Parameters { get; private set; }

        /// <summary>
        /// Inject a table of values to simulate the Command Results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public void Inject<T>(IEnumerable<T> values)
        {
            _values = values.Cast<object>();
        }

        /// <summary>
        /// Inject a simple type value or a single complex type to simulate the Command Results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public void Inject(object value)
        {
            _values = new object[] { value };
        }

        /// <summary>
        /// Returns the array of values converted as a DataTable
        /// </summary>
        /// <returns></returns>
        internal Schema.DataTable GetDataTable()
        {
            var table = new Schema.DataTable();
            table.Load(_values, firstRowOnly: false);
            return table;
        }

        /// <summary>
        /// Returns the array of values converted as a DataRow
        /// </summary>
        /// <returns></returns>
        internal Schema.DataRow GetDataRow()
        {
            var table = new Schema.DataTable();
            table.Load(_values, firstRowOnly: true);
            return table.Rows[0];
        }

        /// <summary>
        /// Returns the first row / column value
        /// </summary>
        /// <returns></returns>
        internal object GetScalar()
        {
            var table = new Schema.DataTable();
            table.Load(_values, firstRowOnly: true);
            return table.Rows[0][0];
        }
    }
}

using Apps72.Dev.Data.Convertor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apps72.Dev.Data.Schema
{
    public class DataTable<T>
    {
        internal DataTable(Func<ColumnsAndRows<T>> actionToGetRows)
        {
            var colProps = actionToGetRows.Invoke();

            this.Columns = colProps.Columns;
            this.Rows = colProps.Rows;
        }

        public IEnumerable<DataColumn> Columns { get; }

        public IEnumerable<T> Rows { get; }
    }
}

using Apps72.Dev.Data.Schema;
using System;
using System.Linq;

namespace Apps72.Dev.Data
{
    internal static class DataTableExtensions
    {
        internal static System.Data.DataTable ConvertToSystemDataTable(this DataTable internalTable)
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.TableName = "DataTable";

            // Columns
            table.Columns.AddRange(internalTable.Columns.Select(c =>
                                            new System.Data.DataColumn()
                                            {
                                                ColumnName = c.ColumnName,
                                                AllowDBNull = c.IsNullable,
                                                DataType = c.DataType
                                            }).ToArray());
            // Rows
            foreach (DataRow row in internalTable.Rows)
            {
                table.Rows.Add(row.ItemArray);
            }

            return table;
        }

    }
}

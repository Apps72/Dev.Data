using Apps72.Dev.Data.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Convertor
{
    public static class DataTableConvertor
    {
        public static IEnumerable<DataTable> ToDataTable(object value)
        {
            switch (value)
            {
                case DataTable table:
                    return new[] { table };

                case IEnumerable<object> rows:
                    return new[] { ToDataTable(rows) };

                case Array rows:
                    return new[] { ToDataTable(rows.Cast<object>()) };

                default:
                    return new[] { new DataTable(null, "Column", value) };
            }
        }

        private static DataTable ToDataTable(IEnumerable<object> rows)
        {
            var table = new DataTable();
            var firstRow = rows.FirstOrDefault();
            var rowType = firstRow?.GetType();

            // No row, so Empty table
            if (firstRow == null) return table;

            // *** Primitive type
            if (TypeExtension.IsPrimitive(rowType))
            {
                table.Columns = new[]
                {
                    new DataColumn(0, "Column", null, rowType, TypeExtension.IsNullable(rowType))
                };

                table.Rows = rows.Select(i => new DataRow(table, new object[] { i })).ToArray();
            }

            // *** Class
            else
            {
                // Class Properties
                var properties = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Columns
                table.Columns = Enumerable.Range(0, properties.Count())
                                           .Select(i =>
                                           {
                                               var prop = properties.ElementAt(i);
                                               return new DataColumn
                                               (
                                                   ordinal: i,
                                                   columnName: prop.Name,
                                                   dataType: prop.PropertyType,
                                                   sqlType: null,
                                                   isNullable: TypeExtension.IsNullable(prop.PropertyType)
                                               );
                                           })
                                           .ToArray();

                // Rows
                table.Rows = rows.Select(row =>
                                    {
                                        var values = properties.Select(prop => prop.GetValue(row, null)).ToArray();
                                        return new DataRow(table, values);
                                    })
                                 .ToArray();

            }

            return table;
        }
    }
}

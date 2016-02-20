using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Apps72.Dev.Data.Internal
{
    /// <summary>
    /// Internal DataTable filled with all data from the Server.
    /// </summary>
    internal class DataTable
    {
        /// <summary>
        /// Initialize a new instance of DataTable
        /// </summary>
        public DataTable()
        {
            this.Columns = null;
            this.Rows = null;
        }

        /// <summary>
        /// Load and fill all data (Rows and Columns) from the DbDataReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="firstRowOnly"></param>
        public void Load(DbDataReader reader, bool firstRowOnly)
        {
            List<DataRow> data = new List<DataRow>();
            int fieldCount = this.FillColumnsProperties(reader);

            while (reader.Read())
            {
                object[] row = new object[fieldCount];
                int result = reader.GetValues(row);
                data.Add(new DataRow(this, row));

                if (firstRowOnly)
                    continue;
            }

            this.Rows = data.AsEnumerable();
        }

        /// <summary>
        /// Fill all columns properties
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private int FillColumnsProperties(DbDataReader reader)
        {
            int fieldCount = reader.FieldCount;

            var columns = new DataColumn[fieldCount];

            for (int i = 0; i < fieldCount; i++)
            {
                columns[i] = new DataColumn()
                {
                    Ordinal = i,
                    ColumnName = reader.GetName(i),
                    ColumnType = reader.GetFieldType(i),
                    IsNullable = reader.IsDBNull(i)
                };
            }

            this.Columns = columns;

            return fieldCount;
        }

        /// <summary>
        /// Gets the Columns properties
        /// </summary>
        public IEnumerable<DataColumn> Columns { get; private set; }

        /// <summary>
        /// Gets all Rows values
        /// </summary>
        public IEnumerable<DataRow> Rows { get; private set; }

        /// <summary>
        /// Creates a new instance of T type and sets all row values to the new T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public T[] DataTableTo<T>()
        {
            T[] results = new T[this.Rows.Count()];

            // If is Primitive type (string, int, ...)
            if (Convertor.TypeExtension.IsPrimitive(typeof(T)))
            {
                int i = 0;
                foreach (DataRow row in this.Rows)
                {
                    object scalar = row[0];
                    if (scalar == null || scalar == DBNull.Value)
                        results[i] = default(T);
                    else
                        results[i] = (T)scalar;
                    i++;
                }
            }

            // If is Complex type (class)
            else
            {
                int i = 0;
                foreach (DataRow row in this.Rows)
                {
                    results[i] = row.ConvertTo<T>();
                    i++;
                }
            }

            return results;
        }
    }

}

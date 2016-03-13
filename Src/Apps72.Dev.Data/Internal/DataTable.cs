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
            int fieldCount = 0;

            // Read Columns definition
            if (reader.Read())
            {
                fieldCount = this.FillColumnsProperties(reader);
            }

            // Read data
            if (fieldCount > 0)
            {
                // Only first row
                if (firstRowOnly)
                {
                    object[] row = new object[fieldCount];
                    int result = reader.GetValues(row);
                    data.Add(new DataRow(this, row));
                }

                // All rows
                else
                {
                    do
                    {
                        object[] row = new object[fieldCount];
                        int result = reader.GetValues(row);
                        data.Add(new DataRow(this, row));

                        if (firstRowOnly)
                            continue;
                    }
                    while (reader.Read());
                }
            }

            this.Rows = data.ToArray();
        }

        /// <summary>
        /// Load and fill all data (Rows and Columns) from the array of object[].
        /// </summary>
        /// <param name="values"></param>
        /// <param name="firstRowOnly"></param>
        public void Load(IEnumerable<object[]> arrayOfvalues, bool firstRowOnly)
        {
            this.FillColumnsProperties(arrayOfvalues.First());
            this.Rows = arrayOfvalues.Select(v => new DataRow(this, v)).ToArray();
        }

        /// <summary>
        /// Load and fill all data (Rows and Columns) from the array of typed object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayOfvalues"></param>
        /// <param name="firstRowOnly"></param>
        public void Load<T>(IEnumerable<T> arrayOfvalues, bool firstRowOnly)
        {
            this.FillColumnsProperties(arrayOfvalues.First());
            this.Rows = arrayOfvalues.Select(v => new DataRow(this, v)).ToArray();
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
        /// Fill all columns properties
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private int FillColumnsProperties(object data)
        {
            if (data != null)
            {
                // Simple type
                if (Convertor.TypeExtension.IsPrimitive(data.GetType()))
                {
                    Type columnType = data.GetType();
                    var column = new DataColumn()
                    {
                        Ordinal = 0,
                        ColumnName = "NoName",
                        ColumnType = Convertor.TypeExtension.GetNullableSubType(columnType),
                        IsNullable = Convertor.TypeExtension.IsNullable(columnType)
                    };

                    this.Columns = new DataColumn[] { column };
                    return 1;
                }

                // Complex type
                else
                {
                    PropertyInfo[] properties = data.GetType().GetProperties();
                    var columns = new DataColumn[properties.Length];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        Type columnType = properties[i].PropertyType;
                        columns[i] = new DataColumn()
                        {
                            Ordinal = i,
                            ColumnName = properties[i].Name,
                            ColumnType = Convertor.TypeExtension.GetNullableSubType(columnType),
                            IsNullable = Convertor.TypeExtension.IsNullable(columnType)
                        };
                    }

                    this.Columns = columns;
                    return properties.Length;
                }
            }
            else
            {
                this.Columns = null;
                return 0;
            }
        }

        /// <summary>
        /// Gets the Columns properties
        /// </summary>
        public DataColumn[] Columns { get; private set; }

        /// <summary>
        /// Gets all Rows values
        /// </summary>
        public DataRow[] Rows { get; private set; }

        /// <summary>
        /// Creates a new instance of T type and sets all row values to the new T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public T[] ConvertTo<T>()
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

#if NET451
        public System.Data.DataTable ConvertToSystemDataTable()
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.TableName = "DataTable";

            // Columns
            table.Columns.AddRange(this.Columns.Select(c => 
                                            new System.Data.DataColumn()
                                            {
                                                ColumnName = c.ColumnName,
                                                AllowDBNull = c.IsNullable,
                                                DataType = c.ColumnType
                                            }).ToArray());
            // Rows
            foreach (DataRow row in this.Rows)
            {
                table.Rows.Add(row.ItemArray);
            }
            
            return table;
        }
#endif
    }

}

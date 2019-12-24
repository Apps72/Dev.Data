using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Schema
{
    /// <summary>
    /// Represents one table of in-memory data.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{SchemaAndName}")]
    public partial class DataTable
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initialize a new instance of DataTable
        /// </summary>
        public DataTable()
        {
            this.Columns = null;
            this.Rows = null;
        }

        /// <summary>
        /// Initialize a new instance of DataTable,
        /// based on a single Row/Col value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="firstColRowValue"></param>
        internal DataTable(string tableName, string columnName, object firstColRowValue)
        {
            this.Name = tableName;
            this.Columns = new DataColumn[] { new DataColumn()
            {
                ColumnName = columnName,
                IsNullable = true,
                DataType = firstColRowValue != null ? firstColRowValue.GetType() : typeof(object)
            } };

            this.Rows = new DataRow[] { new DataRow(this, new object[] { firstColRowValue }) };
        }

        /// <summary>
        /// Initialize a new instance of DataTable, 
        /// load and fill all data (Rows and Columns) from the DbDataReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="firstRowOnly"></param>
        internal DataTable(DbDataReader reader, bool firstRowOnly) : this()
        {
            this.Load(reader, firstRowOnly);
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the name of this Table
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the Schema of this table
        /// </summary>
        public string Schema { get; internal set; }

        /// <summary>
        /// Gets the Schema and the Name of this table, separated by an underscore.
        /// </summary>
        public string SchemaAndName
        {
            get
            {
                return $"{Schema}_{Name}";
            }
        }

        /// <summary>
        /// Gets True if this 'Table' is a View.
        /// Not developed (always False)
        /// </summary>
        public bool IsView { get; internal set; }

        /// <summary>
        /// Gets the Columns properties
        /// </summary>
        public DataColumn[] Columns { get; internal set; }

        /// <summary>
        /// Gets all Rows values
        /// </summary>
        public DataRow[] Rows { get; internal set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Creates a new instance of T type and sets all row values to the new T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T[] ConvertTo<T>()
        {
            if (this.Rows == null || this.Rows.Length <= 0)
                return new T[0];

            var results = new T[this.Rows.Count()];

            // If is Primitive type (string, int, ...)
            if (Apps72.Dev.Data.Convertor.TypeExtension.IsPrimitive(typeof(T)))
            {
                int i = 0;
                foreach (var row in this.Rows)
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
                foreach (var row in this.Rows)
                {
                    results[i] = row.ConvertTo<T>();
                    i++;
                }
            }

            return results;
        }

        #endregion

        #region PRIVATES

        /// <summary>
        /// Load and fill all data (Rows and Columns) from the DbDataReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="firstRowOnly"></param>
        internal void Load(DbDataReader reader, bool firstRowOnly)
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
        /// <param name="arrayOfvalues"></param>
        /// <param name="firstRowOnly"></param>
        internal void Load(IEnumerable<object[]> arrayOfvalues, bool firstRowOnly)
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
        internal void Load<T>(IEnumerable<T> arrayOfvalues, bool firstRowOnly)
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

            this.Columns = Enumerable.Range(0, fieldCount)
                                     .Select(i => new DataColumn()
                                     {
                                         ColumnName = reader.GetName(i),
                                         DataType = reader.GetFieldType(i),
                                         Ordinal = i,
                                         IsNullable = reader.IsDBNull(i)
                                     })
                                     .ToArray();

            return fieldCount;
        }

        /// <summary>
        /// Fill all columns properties
        /// </summary>
        /// <param name="data"></param>
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
                        DataType = Convertor.TypeExtension.GetNullableSubType(columnType),
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
                            DataType = Apps72.Dev.Data.Convertor.TypeExtension.GetNullableSubType(columnType),
                            IsNullable = Apps72.Dev.Data.Convertor.TypeExtension.IsNullable(columnType)
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

        #endregion
    }
}

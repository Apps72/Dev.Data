using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// Extractor of all tables and columns properties from Database specified in the ConnectionString.
    /// </summary>
    public partial class SqlEntitiesGenerator
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connection">Connection to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(DbConnection connection)
        {
            SearchAndFill(connection);
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the DbConnection connected to the database
        /// </summary>
        protected virtual DbConnection Connection { get; set; }

        /// <summary>
        /// Gets the Database server name
        /// </summary>
        public string ProductName { get; private set; }

        /// <summary>
        /// Gets the Database server version
        /// </summary>
        public string ProductVersion { get; private set; }

        /// <summary>
        /// Gets all tables and views founds
        /// </summary>
        public virtual IEnumerable<Schema.DataTable> TablesAndViews { get; protected set; }

        /// <summary>
        /// Gets all tables founds
        /// </summary>
        public virtual IEnumerable<Schema.DataTable> Tables => this.TablesAndViews.Where(t => t.IsView == false);

        /// <summary>
        /// Gets all views founds
        /// Not developed (always empty)
        /// </summary>
        public virtual IEnumerable<Schema.DataTable> Views => this.TablesAndViews.Where(t => t.IsView == true);

        #endregion

        #region METHODS

        /// <summary>
        /// Search all columns definitions
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void SearchAndFill(DbConnection connection)
        {
            // Initializes
            this.Connection = connection ?? throw new ArgumentException("The connection must be defined.");
            Convertor.DbTypeMap.Initialize(connection);

            // Gets Database Server information
            DataRow information = Connection.GetSchema("DataSourceInformation").Rows[0];

            this.ProductName = Convert.ToString(information["DataSourceProductName"]);
            this.ProductVersion = Convert.ToString(information["DataSourceProductVersion"]);

            // Gets all columns and convert to DataTables
            IEnumerable<TableAndColumn> allColumns = GetTablesDescription();
            this.TablesAndViews = ConvertDescriptionsToTables(allColumns);
        }

        /// <summary>
        /// Search all table names and columns names in SQL Server
        /// </summary>
        protected virtual IEnumerable<TableAndColumn> GetTablesDescription()
        {
            List<TableAndColumn> tableAndColumns = new List<TableAndColumn>();
            var fields = new SchemaColumnsFields(ProductName);

            // Columns
            DataTable allColumns = Connection.GetSchema(fields.NAME);

            // Tables et columns
            foreach (DataRow row in allColumns.Rows)
            {
                tableAndColumns.Add(new TableAndColumn()
                {
                    SequenceNumber = Convert.ToInt32(row[fields.SequenceNumber]),
                    SchemaName = Convert.ToString(row[fields.SchemaName]),
                    TableName = Convert.ToString(row[fields.TableName]),
                    ColumnName = Convert.ToString(row[fields.ColumnName]),
                    ColumnType = Convert.ToString(row[fields.ColumnType]),
                    ColumnSize = row[fields.ColumnSize] != DBNull.Value ? Convert.ToInt32(row[fields.ColumnSize]) : 0,
                    IsColumnNullable = ToBoolean(Convert.ToString(row[fields.IsColumnNullable])),
                    IsView = false  // TODO
                });
            }

            return tableAndColumns;
        }

        /// <summary>
        /// Tranform the list of tables and columns (description of database tables) to a list of DataTable.
        /// </summary>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Schema.DataTable> ConvertDescriptionsToTables(IEnumerable<TableAndColumn> descriptions)
        {
            // Select all tables
            var tables = descriptions.GroupBy(i => new { i.TableName, i.SchemaName, i.IsView })
                                        .Select(i => new Schema.DataTable()
                                        {
                                            Schema = i.Key.SchemaName,
                                            Name = i.Key.TableName,
                                            IsView = i.Key.IsView
                                        })
                                        .ToArray();

            Convertor.DbTypeMap.Initialize(Connection);

            // Assign all columns
            for (int i = 0; i < tables.Length; i++)
            {
                var table = tables[i];
                table.Columns = descriptions.Where(c => c.SchemaName == table.Schema && 
                                                        c.TableName == table.Name)
                                               .Select(c => new Schema.DataColumn(table)
                                               {
                                                   ColumnName = RemoveExtraChars(c.ColumnName),
                                                   SqlType = c.ColumnType,
                                                   DataType = Convertor.DbTypeMap.FirstType(c.ColumnType),
                                                   IsNullable = c.IsColumnNullable,
                                                   Ordinal = c.SequenceNumber
                                               })
                                               .ToArray();
            }

            // Remove extra chars
            foreach (var table in tables)
            {
                table.Name = RemoveExtraChars(table.Name);
                table.Schema = RemoveExtraChars(table.Schema);
            }

            return tables;
        }

        /// <summary>
        /// Remove invalid chars for CSharp class and property names.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/gg615485.aspx </remarks>
        protected virtual string RemoveExtraChars(string name)
        {
            StringBuilder newName = new StringBuilder();
            int ascii = 0;

            // Keep only digits, letters or underscore
            foreach (char c in name)
            {
                // Ascii code of the current Char
                ascii = (int)c;

                // 0 .. 9, A .. Z, a .. z, _
                if (ascii >= 48 && ascii <= 57 ||
                    ascii >= 65 && ascii <= 90 ||
                    ascii >= 97 && ascii <= 122 ||
                    ascii == 95)
                {
                    newName.Append(c);
                }
            }

            // First char must be a letter or underscore
            if (newName.Length > 0)
            {
                ascii = (int)newName[0];
                if (ascii >= 65 && ascii <= 90 ||
                    ascii >= 97 && ascii <= 122 ||
                    ascii == 95)
                {
                    return newName.ToString();
                }
                else
                {
                    return $"_{newName.ToString()}";
                }
            }
            else
            {
                return $"__{Guid.NewGuid().ToString().Replace('-', '_')}";
            }

        }

        /// <summary>
        /// Convert the string to a boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ToBoolean(string value)
        {
            switch (value.ToUpper())
            {
                case "YES":
                case "Y":
                case "TRUE":
                    return true;

                case "NO":
                case "N":
                case "FALSE":
                    return false;

                default:
                    return false;
            }
        }

        #endregion
    }
}

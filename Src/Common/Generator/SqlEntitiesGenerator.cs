using Apps72.Dev.Data.Convertor;
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
        /// Search all table names and columns names in Database
        /// </summary>
        /// <remarks>
        /// See https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/getschema-and-schema-collections
        /// </remarks>
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
                    DatabaseFamily = fields.DatabaseFamily,
                    SequenceNumber = Convert.ToInt32(row[fields.SequenceNumber]),
                    SchemaName = Convert.ToString(row[fields.SchemaName]),
                    TableName = Convert.ToString(row[fields.TableName]),
                    ColumnName = Convert.ToString(row[fields.ColumnName]),
                    ColumnType = ExtractTypeNameOnly(Convert.ToString(row[fields.ColumnType])),
                    ColumnSize = row[fields.ColumnSize] != DBNull.Value ? Convert.ToInt32(row[fields.ColumnSize]) : 0,
                    NumericPrecision = row[fields.NumericPrecision] != DBNull.Value ? Convert.ToInt32(row[fields.NumericPrecision]) : 0,
                    NumericScale = row[fields.NumericScale] != DBNull.Value ? Convert.ToInt32(row[fields.NumericScale]) : 0,
                    IsColumnNullable = Convert.ToString(row[fields.IsColumnNullable]).ToBoolean(),
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
                                                   ColumnName = c.ColumnName,
                                                   SqlType = c.ColumnType,
                                                   DataType = c.GetDataType(),
                                                   IsNullable = c.IsColumnNullable,
                                                   Ordinal = c.SequenceNumber
                                               })
                                               .ToArray();
            }

            // Remove extra chars
            foreach (var table in tables)
            {
                table.Name = table.Name.RemoveExtraChars();
                table.Schema = table.Schema.RemoveExtraChars();
            }

            return tables;
        }

        private string ExtractTypeNameOnly(string columnType)
        {
            if (columnType.Contains('(') && columnType.Contains(')'))
            {                
                return ReplaceBetween(columnType, '(', ')', String.Empty);
            }
            else
                return columnType;
        }

        private string ReplaceBetween(string text, char from, char to, string newValue)
        {
            int indexFrom = text.IndexOf(from);
            int indexTo = text.IndexOf(to);
            
            if (indexTo > indexFrom)
            {                
                return String.Format("{0}{1}{2}", text.Substring(0, indexFrom), newValue, text.Substring(indexTo + 1));
            }
            else
                return text;
        }

        #endregion
    }
}

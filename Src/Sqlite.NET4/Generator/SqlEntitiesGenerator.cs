using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Data;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// Extractor of all tables and columns properties from Database specified in the ConnectionString.
    /// </summary>
    public class SqlEntitiesGenerator : SqlEntitiesGeneratorBase
    {
        private Dictionary<string, string> _mapping = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(string connectionString) : this(connectionString, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionStringName">ConnectionString or App.Config Name of the ConnectionString to retrieve all tables and columns</param>
        /// <param name="appConfig">Path and name of the App.Config file where the connection string is written</param>
        public SqlEntitiesGenerator(string connectionStringName, string appConfig) : base()
        {
            if (!String.IsNullOrEmpty(appConfig))
            {
                ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
                configFile.ExeConfigFilename = appConfig;
                var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
                this.ConnectionString = configuration.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;
            }
            else
            {
                this.ConnectionString = connectionStringName;
            }

            // Search and fill columns
            using (var conn = new SQLiteConnection(this.ConnectionString))
            {
                conn.Open();
                base.SearchAndFill(conn);
                conn.Close();
            }
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connection">Connection to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(SQLiteConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Gets the connection string
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Search all table names and columns names in SQL Server
        /// </summary>
        protected override IEnumerable<TableAndColumn> GetTablesDescription()
        {
            List<TableAndColumn> tableAndColumns = new List<TableAndColumn>();
            
            // Columns
            DataTable allColumns = base.Connection.GetSchema("Columns");
            DataTable allTypes = base.Connection.GetSchema("DataTypes");

            // Datatypes
            foreach (DataRow row in allTypes.Rows)
            {
                _mapping.Add(row.Field<string>("TypeName"), row.Field<string>("DataType"));
            }

            // Tables et columns
            foreach (DataRow row in allColumns.Rows)
            {
                tableAndColumns.Add(new TableAndColumn()
                {
                    ColumnName = row.Field<string>("COLUMN_NAME"),
                    TableName = row.Field<string>("TABLE_NAME"),
                    SchemaName = row.Field<string>("TABLE_SCHEMA"),
                    ColumnType = row.Field<string>("DATA_TYPE"),
                    IsColumnNullable = row.Field<bool>("IS_NULLABLE"),
                    IsView = false                    
                });
            }

            return tableAndColumns;
        }

        /// <summary>
        /// Tranform the list of tables and columns (description of database tables) to a list of DataTable.
        /// </summary>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        protected override IEnumerable<Schema.DataTable> ConvertDescriptionsToTables(IEnumerable<TableAndColumn> descriptions)
        {
            IEnumerable<Schema.DataTable> tables = base.ConvertDescriptionsToTables(descriptions);

            foreach (var table in tables)
            {
                foreach (var column in table.Columns)
                {
                    if (_mapping.ContainsKey(column.SqlType))
                        column.CSharpType = _mapping[column.SqlType];
                    else
                        column.CSharpType = "Object";
                }
            }

            return tables;
        }
    }
}

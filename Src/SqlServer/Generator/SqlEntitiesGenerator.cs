using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// Extractor of all tables and columns properties from Database specified in the ConnectionString.
    /// </summary>
    public class SqlEntitiesGenerator : SqlEntitiesGeneratorBase
    {
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
            // Gets the ConnectionString 
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
            using (var conn = new SqlConnection(this.ConnectionString))
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
        public SqlEntitiesGenerator(SqlConnection connection) : base(connection)
        {

        }

        /// <summary>
        /// Gets the connection string
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Returns the SQL query will be executed to retrieve description of tables and columns
        /// </summary>
        /// <returns></returns>
        protected override string GetSqlQueryToDescribeTables()
        {
            return @"SELECT sys.schemas.name AS SchemaName,
                            sys.tables.name AS TableName, 
                            sys.columns.name AS ColumnName, 
                            sys.types.name AS ColumnType, 
                            sys.columns.max_length AS ColumnSize, 
                            sys.columns.is_nullable AS IsColumnNullable, 
                            CAST(0 AS BIT) AS IsView 
                     FROM sys.tables 
                            INNER JOIN sys.schemas ON sys.tables.schema_id = sys.schemas.schema_id 
                            INNER JOIN sys.columns ON sys.tables.object_id = sys.columns.object_id 
                            INNER JOIN sys.types ON sys.types.user_type_id = sys.columns.user_type_id 
                     ORDER BY SchemaName, TableName, column_id ";
        }
    }
}

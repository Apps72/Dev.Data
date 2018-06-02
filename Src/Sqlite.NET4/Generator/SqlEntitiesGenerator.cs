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
    public partial class SqlEntitiesGenerator
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
            if (String.IsNullOrEmpty(connectionStringName) && String.IsNullOrEmpty(appConfig)) return;

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
            using (var conn = new SQLiteConnection(this.ConnectionString))
            {
                conn.Open();
                this.SearchAndFill(conn);
                conn.Close();
            }

        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connection">Connection to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(SQLiteConnection connection)
        {
            SearchAndFill(connection);
        }

        /// <summary>
        /// Gets the connection string
        /// </summary>
        public string ConnectionString { get; private set; }
        
    }
}

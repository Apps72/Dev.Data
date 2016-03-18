using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// Extractor of all tables and columns properties from Database specified in the ConnectionString.
    /// </summary>
    public class SqlEntitiesGenerator
    {
        private string _connectionString = string.Empty;
        private SqliteConnection _connection = null;

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(string connectionString)
        {
            _connectionString = connectionString;
            this.FillAllTablesAndColumns();
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summaryconnection
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(SqliteConnection connection)
        {
            _connection = connection;
            this.FillAllTablesAndColumns();
        }

        /// <summary>
        /// Gets all tables founds
        /// </summary>
        public virtual IEnumerable<Table> Tables { get; private set; }

        /// <summary>
        /// Search all table names and columns names in SQL Server
        /// </summary>
        protected virtual void FillAllTablesAndColumns()
        {
            // TODO: To complete
        }
    }
}

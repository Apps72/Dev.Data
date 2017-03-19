using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SqlEntitiesGeneratorBase
    {
        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        protected SqlEntitiesGeneratorBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGeneratorBase(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.FillAllTablesAndColumns();
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connection">Connection to retrieve all tables and columns</param>
        public SqlEntitiesGeneratorBase(DbConnection connection)
        {
            this.Connection = connection;
            this.FillAllTablesAndColumns();
        }

        /// <summary>
        /// Gets the connection string to the database
        /// </summary>
        public virtual string ConnectionString { get; protected set; }

        /// <summary>
        /// Gets the DbConnection connected to the database
        /// </summary>
        protected virtual DbConnection Connection { get; set; }

        /// <summary>
        /// Gets all tables founds
        /// </summary>
        public virtual IEnumerable<Schema.DataTable> Tables { get; protected set; }

        /// <summary>
        /// Search all table names and columns names in SQL Server
        /// </summary>
        protected virtual void FillAllTablesAndColumns()
        {
            throw new NotImplementedException("You must override this method.");
        }
    }
}

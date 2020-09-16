using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Generator
    {
        public Generator(Arguments args)
        {
            this.Arguments = args;

            using (DbConnection conn = GetConnection())
            {
                conn.ConnectionString = Arguments.ConnectionString;
                conn.Open();

                this.AllEntities = new EntityGenerator(conn, this.Arguments.OnlySchema);

                var generator = new GeneratorCSharp(this.AllEntities, Arguments);
                this.Code = generator.Code;
                this.EntitiesGenerated = generator.Entities;

                conn.Close();
            }
        }

        public Arguments Arguments { get; private set; }

        public EntityGenerator AllEntities { get; set; }

        public string Code { get; private set; }

        public IEnumerable<Schema.DataTable> EntitiesGenerated { get; set; }

        private DbConnection GetConnection()
        {
            // Oracle
            if (Arguments.Provider.IsEqualTo("Oracle"))
                return new Oracle.ManagedDataAccess.Client.OracleConnection();

            // SQLite
            if (Arguments.Provider.IsEqualTo("SQLite"))
                return new Microsoft.Data.Sqlite.SqliteConnection();

            // SQLServer or Default
            return new System.Data.SqlClient.SqlConnection();
        }
    }
}

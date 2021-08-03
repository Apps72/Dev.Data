using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Generator
    {
        public Generator(Arguments args, Action<GeneratorOptions> options = null)
        {
            this.Arguments = args;

            var config = new GeneratorOptions();
            options?.Invoke(config);

            using (DbConnection conn = GetConnection())
            {
                conn.ConnectionString = Arguments.ConnectionString;
                conn.Open();

                config.PreCommand?.Invoke(conn);

                this.AllEntities = new EntityGenerator(conn, this.Arguments.OnlySchema);

                var generator = new GeneratorCSharp(this.AllEntities, Arguments);
                this.Code = generator.Code;
                this.EntitiesGenerated = generator.Entities;

                config.PostCommand?.Invoke(conn);

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

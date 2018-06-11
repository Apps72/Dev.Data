using System;
using System.Data.Common;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Generator
    {
        public Generator(string[] args)
        {
            this.Arguments = new Arguments(args);

            using (DbConnection conn = GetConnection())
            {
                conn.ConnectionString = Arguments.ConnectionString;
                conn.Open();

                var generator = new GeneratorCSharp(new SqlEntitiesGenerator(conn), Arguments);
                this.Code = generator.GenerateCodeForEntities();

                conn.Close();
            }
        }

        public Arguments Arguments { get; private set; }

        public string Code { get; set; }

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

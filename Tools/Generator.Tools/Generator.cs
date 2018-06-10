using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

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
            if (Arguments.Provider.IsEqualTo("SqlServer"))
                return new SqlConnection();

            return null;
        }
    }
}

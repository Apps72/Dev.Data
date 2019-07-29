using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Runner
    {
        public Runner(Arguments args)
        {
            this.Files = args.GetFilesForSource();
            this.Separator = args.Separator;
            this.Provider = args.Provider;
            this.ConnectionString = args.ConnectionString;
        }

        public Runner Start()
        {
            using (var connection = GetConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();

                string[] separatorWithCR = new[]
                {
                    $"{Environment.NewLine}{this.Separator}{Environment.NewLine}"
                };

                foreach (var file in this.Files)
                {
                    string sql = File.ReadAllText(file.FullName);
                    string[] scripts;

                    // Splits
                    if (!String.IsNullOrEmpty(this.Separator))
                    {
                        scripts = sql.Split(separatorWithCR, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        scripts = new[] { sql };
                    }

                    // Execute
                    foreach (var script in scripts)
                    {
                        Console.WriteLine($" ... Execution of script \"{file.Name}\"");
                        Console.WriteLine($"       {script.Replace(Environment.NewLine, " ").Left(80)}");
                        Console.WriteLine($"       ...");
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = script;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                connection.Close();
            }
            return this;
        }

        public IEnumerable<FileInfo> Files { get; private set; }

        public string ConnectionString { get; private set; }

        public string Separator { get; private set; }

        public string Provider { get; private set; }

        private DbConnection GetConnection()
        {
            // Oracle
            if (Provider.IsEqualTo("Oracle"))
                return new Oracle.ManagedDataAccess.Client.OracleConnection();

            // SQLite
            if (Provider.IsEqualTo("SQLite"))
                return new Microsoft.Data.Sqlite.SqliteConnection();

            // SQLServer or Default
            return new System.Data.SqlClient.SqlConnection();
        }
    }
}

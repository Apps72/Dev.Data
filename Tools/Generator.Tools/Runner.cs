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
            this.DbConfigAfter = args.DbConfigAfter;
            this.DbConfigUpdate = args.DbConfigUpdate;
        }

        public Runner Start()
        {
            string lastScriptName = string.Empty;

            using (var connection = GetConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();

                string[] separatorWithCR = new[]
                {
                    $"{Environment.NewLine}{this.Separator}{Environment.NewLine}"
                };

                // Filter files list
                if (!string.IsNullOrEmpty(this.DbConfigAfter))
                {
                    this.Files = FilterFiles(connection, this.DbConfigAfter);
                }

                // Read and execute all files
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
                        lastScriptName = file.NameWithoutExtension();
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

                // Update the database version
                if (!string.IsNullOrEmpty(this.DbConfigUpdate) && !string.IsNullOrEmpty(lastScriptName))
                {
                    UpdateConfigDb(connection, this.DbConfigUpdate, lastScriptName);
                }

                connection.Close();
            }
            return this;
        }

        public IEnumerable<FileInfo> Files { get; private set; }

        public string ConnectionString { get; private set; }

        public string Separator { get; private set; }

        public string Provider { get; private set; }

        public string DbConfigAfter { get; private set; }

        public string DbConfigUpdate { get; private set; }

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

        private IEnumerable<FileInfo> FilterFiles(DbConnection connection, string queryConfigAfter)
        {
            string lastConfigValue;

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = queryConfigAfter;
                lastConfigValue = Convert.ToString(cmd.ExecuteScalar());

                Console.WriteLine($" ... Read the value of \"DbConfigAfter\" script = \"{lastConfigValue}\".");
                Console.WriteLine($"       {queryConfigAfter.Replace(Environment.NewLine, " ").Left(80)}");
                Console.WriteLine($"       ...");
            }

            return this.Files.Where(i => String.Compare(i.NameWithoutExtension(), lastConfigValue, ignoreCase: true) > 0);
        }

        private void UpdateConfigDb(DbConnection connection, string queryUpdateConfig, string lastScriptName)
        {
            Console.WriteLine($" ... Update the database via \"DbConfigUpdate\" script, with \"{lastScriptName}\".");
            Console.WriteLine($"       {queryUpdateConfig.Replace(Environment.NewLine, " ").Left(80)}");
            Console.WriteLine($"       ...");
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = queryUpdateConfig.Replace("@Filename",        // For SQL Server
                                                            $"'{lastScriptName.Replace("'", "''")}'",
                                                            StringComparison.InvariantCultureIgnoreCase)
                                                   .Replace(":Filename",        // For Oracle
                                                            $"'{lastScriptName.Replace("'", "''")}'",
                                                            StringComparison.InvariantCultureIgnoreCase);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Apps72.Dev.Data.Performances
{
    public class SqlDatabaseCommandPerformances
    {
        private SqlConnection _connection;

        public SqlDatabaseCommandPerformances(string connectionString, int numberOfExecutions)
        {
            this.NumberOfExecutions = numberOfExecutions;
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public int NumberOfExecutions { get; set; }

        public void SelectCount()
        {
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < this.NumberOfExecutions; i++)
            {
                using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
                {
                    cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    var count = cmd.ExecuteScalar();
                }
            }
            var elapsed = watch.ElapsedMilliseconds;

            Console.WriteLine($"SqlDatabaseCommand - SelectCount: {elapsed} ms for {this.NumberOfExecutions} executions... {(double)elapsed / (double)this.NumberOfExecutions} ms for one execution.");
        }

    }
}


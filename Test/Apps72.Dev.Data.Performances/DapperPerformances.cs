using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Dapper;

namespace Apps72.Dev.Data.Performances
{
    public class DapperPerformances
    {
        private SqlConnection _connection;

        public DapperPerformances(string connectionString, int numberOfExecutions)
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
                var count = _connection.Query("SELECT COUNT(*) FROM EMP");
            }
            var elapsed = watch.ElapsedMilliseconds;
                                
            Console.WriteLine($"Dapper             - SelectCount: {elapsed} ms for {this.NumberOfExecutions} executions... {(double)elapsed / (double)this.NumberOfExecutions} ms for one execution.");
        }
    }
}

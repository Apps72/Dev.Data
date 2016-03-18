using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Dapper;
using System.Linq;

namespace Apps72.Dev.Data.Performances
{
    public class DapperPerformances : IPerformance
    {
        public DapperPerformances()
        {
            this.Watcher = new Stopwatch();
        }

        public SqlConnection Connection { get; set; }

        public int NumberOfExecutions { get; set; }

        public Stopwatch Watcher { get; }

        public void ExecuteScalar()
        {
            this.Watcher.Restart();

            for (int i = 0; i < this.NumberOfExecutions; i++)
            {
                var count = this.Connection.ExecuteScalar("SELECT COUNT(*) FROM EMP");
            }

            Program.DisplayResult(this);
        }

        public void ExecuteScalarTyped()
        {
            this.Watcher.Restart();

            for (int i = 0; i < this.NumberOfExecutions; i++)
            {
                int count = this.Connection.ExecuteScalar<int>("SELECT COUNT(*) FROM EMP");
            }

            Program.DisplayResult(this);
        }

        public void ExecuteRowTyped()
        {
            this.Watcher.Restart();

            for (int i = 0; i < this.NumberOfExecutions; i++)
            {
                EMP emp = this.Connection.Query<EMP>(" SELECT * FROM EMP WHERE EMPNO = 7369 ").First();
            }

            Program.DisplayResult(this);
        }

    }
}

using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Apps72.Dev.Data.Performances
{
    public class SqlDatabaseCommandPerformances : IPerformance
    {
        public SqlDatabaseCommandPerformances()
        {
            this.Watcher = new Stopwatch();
        }

        public SqlConnection Connection { get; set; }

        public int NumberOfExecutions { get; set; }

        public Stopwatch Watcher { get; set; }

        public void ExecuteScalar()
        {
            this.Watcher.Restart();
            
            for (int i = 0; i < this.NumberOfExecutions; i++)
            {
                using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(this.Connection))
                {
                    cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    var count = cmd.ExecuteScalar();
                }
            }

            Program.DisplayResult(this);
        }

        public void ExecuteScalarTyped()
        {
            this.Watcher.Restart();

            for (int i = 0; i < this.NumberOfExecutions; i++)
            {
                using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(this.Connection))
                {
                    cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    int count = cmd.ExecuteScalar<int>();
                }
            }

            Program.DisplayResult(this);
        }

        public void ExecuteRowTyped()
        {
            this.Watcher.Restart();

            for (int i = 0; i < this.NumberOfExecutions; i++)
            {
                using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(this.Connection))
                {
                    cmd.CommandText.AppendLine(" SELECT * FROM EMP WHERE EMPNO = 7369");
                    EMP emp = cmd.ExecuteRow<EMP>();
                }
            }

            Program.DisplayResult(this);
        }
    }
}


using Apps72.Dev.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests
{
    [TestClass]
    public class SqlBestPracticeTests
    {
        [TestMethod]
        public void BestPractice1_TwoCommandWithTransactions_Test()
        {
            BestPractice11.DataService service = new BestPractice11.DataService();

            using (SqlDatabaseCommand cmd = service.GetDatabaseCommand())
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");
                cmd.TransactionBegin();
                cmd.ExecuteNonQuery();

                using (SqlDatabaseCommand cmd2 = service.GetDatabaseCommand(cmd.Transaction))
                {
                    cmd2.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    int count = cmd2.ExecuteScalar<int>();
                }

                cmd.TransactionRollback();
            }
        }

        [TestMethod]
        public void BestPractice2_TwoCommandWithTransactions_Test()
        {
            BestPractice2.DataService service = new BestPractice2.DataService();

            using (SqlDatabaseCommand cmd = service.GetDatabaseCommand())
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");
                cmd.TransactionBegin();
                cmd.ExecuteNonQuery();

                using (SqlDatabaseCommand cmd2 = service.GetDatabaseCommand(cmd.Transaction))
                {
                    cmd2.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    int count = cmd2.ExecuteScalar<int>();
                }

                cmd.TransactionRollback();
            }
        }

    }

    public interface IDataService
    {
        SqlDatabaseCommand GetDatabaseCommand();
        SqlDatabaseCommand GetDatabaseCommand(SqlTransaction transaction);
    }

    namespace BestPractice11
    {

        public class DataService : IDataService, IDisposable
        {
            public string CONNECTION_STRING = SqlDatabaseCommandTests.CONNECTION_STRING;
            private SqlConnection _connection = null;

            public DataService()
            {
                _connection = new SqlConnection(CONNECTION_STRING);
                _connection.Open();
            }

            public SqlDatabaseCommand GetDatabaseCommand()
            {
                return new SqlDatabaseCommand(_connection);
            }

            public SqlDatabaseCommand GetDatabaseCommand(SqlTransaction transaction)
            {
                return new SqlDatabaseCommand(_connection, transaction);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                        _connection.Dispose();
                        _connection = null;
                    }
                }
            }

            ~DataService()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }

    namespace BestPractice2
    {

        public class DataService : IDataService
        {
            public string CONNECTION_STRING = SqlDatabaseCommandTests.CONNECTION_STRING;

            public SqlDatabaseCommand GetDatabaseCommand()
            {
                return new SqlDatabaseCommand(CONNECTION_STRING);
            }

            public SqlDatabaseCommand GetDatabaseCommand(SqlTransaction transaction)
            {
                return new SqlDatabaseCommand(transaction.Connection, transaction);
            }
        }

    }
}

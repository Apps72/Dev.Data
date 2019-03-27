using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;

namespace Data.Core.Tests
{
    /*
        To run these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class RetryTests
    {
        #region INITIALIZATION

        private SqlConnection _connection;

        [TestInitialize]
        public void Initialization()
        {
            _connection = new SqlConnection(Configuration.CONNECTION_STRING);
            _connection.Open();
        }

        [TestCleanup]
        public void Finalization()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        #endregion

        [TestMethod()]
        public void RaiseSQLDeadLock_Test()
        {
            DbException ex = this.RaiseSqlDeadLock(false);

            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("deadlock"));
        }

        [TestMethod()]
        public void RetryWhenDeadLockOccured_Test()
        {
            DbException ex = this.RaiseSqlDeadLock(true);

            Assert.IsNull(ex);
        }

        private DbException RaiseSqlDeadLock(bool withRetry)
        {
            // See: http://stackoverflow.com/questions/22825147/how-to-simulate-deadlock-on-sql-server

            SqlConnection connection2 = new SqlConnection(Configuration.CONNECTION_STRING);
            connection2.Open();
            DbException exToReturn = null;

            try
            {
                using (var cmd = new DatabaseCommand(_connection))
                {
                    cmd.Log = Console.WriteLine;

                    cmd.CommandText.AppendLine(" CREATE TABLE ##Employees ( ");
                    cmd.CommandText.AppendLine("     EmpId INT IDENTITY, ");
                    cmd.CommandText.AppendLine("     EmpName VARCHAR(16), ");
                    cmd.CommandText.AppendLine("     Phone VARCHAR(16) ");
                    cmd.CommandText.AppendLine(" ) ");

                    cmd.CommandText.AppendLine(" INSERT INTO ##Employees (EmpName, Phone) ");
                    cmd.CommandText.AppendLine(" VALUES('Martha', '800-555-1212'), ('Jimmy', '619-555-8080') ");

                    cmd.CommandText.AppendLine(" CREATE TABLE ##Suppliers( ");
                    cmd.CommandText.AppendLine("     SupplierId INT IDENTITY, ");
                    cmd.CommandText.AppendLine("     SupplierName VARCHAR(64), ");
                    cmd.CommandText.AppendLine("     Fax VARCHAR(16) ");
                    cmd.CommandText.AppendLine(" ) ");

                    cmd.CommandText.AppendLine(" INSERT INTO ##Suppliers (SupplierName, Fax) ");
                    cmd.CommandText.AppendLine(" VALUES ('Acme', '877-555-6060'), ('Rockwell', '800-257-1234') ");

                    cmd.ExecuteNonQuery();

                }

                using (var cmd1 = new DatabaseCommand(_connection))
                {
                    using (var cmd2 = new DatabaseCommand(connection2))
                    {
                        cmd1.Log = Console.WriteLine;
                        cmd2.Log = Console.WriteLine;

                        cmd1.TransactionBegin();
                        cmd2.TransactionBegin();

                        cmd1.Clear();
                        cmd1.CommandText.AppendLine(" UPDATE ##Employees SET EmpName = 'Mary'    WHERE empid = 1 ");
                        cmd1.ExecuteNonQuery();

                        cmd2.Clear();
                        cmd2.CommandText.AppendLine(" UPDATE ##Suppliers SET Fax = N'555-1212'   WHERE supplierid = 1 ");
                        cmd2.ExecuteNonQuery();

                        // Start and when cmd2.ExecuteNonQuery command will be executed, an DeadLock exception will be raised.
                        Task task1 = Task.Factory.StartNew(() =>
                        {
                            cmd1.Clear();
                            cmd1.ThrowException = false;
                            if (withRetry)
                            {
                                cmd1.Retry.SetDefaultExceptionsToRetry(DatabaseRetry.DefaultExceptionsToRetry.SqlServer_DeadLock);
                            }
                            cmd1.CommandText.AppendLine(" UPDATE ##Suppliers SET Fax = N'555-1212'   WHERE supplierid = 1 ");
                            cmd1.ExecuteNonQuery();
                        });

                        System.Threading.Thread.Sleep(1000);

                        cmd2.Clear();
                        cmd2.CommandText.AppendLine(" UPDATE ##Employees SET phone = N'555-9999' WHERE empid = 1 ");
                        cmd2.ExecuteNonQuery();

                        cmd2.Dispose();
                        connection2.Close();

                        // Wait cmd1 finished (and raised an Exception)
                        task1.Wait();

                        exToReturn = cmd1.Exception;
                    }
                }

                using (var cmd = new DatabaseCommand(_connection))
                {
                    cmd.Log = Console.WriteLine;

                    cmd.CommandText.AppendLine(" DROP TABLE ##Employees ");
                    cmd.CommandText.AppendLine(" DROP TABLE ##Suppliers ");
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                connection2.Close();
                connection2.Dispose();
                connection2 = null;
            }

            return exToReturn;
        }
    }
}

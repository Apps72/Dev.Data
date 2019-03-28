using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;
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
        public void RaiseSQLWithDeadLock_Test()
        {
            DbException ex = this.RaiseSqlDeadLock(withRetry: false, throwException: false);

            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("deadlock"));
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void RaiseSQLWithDeadLock_Exception_Test()
        {
            this.RaiseSqlDeadLock(withRetry: false, throwException: true);
        }

        [TestMethod()]
        public void RetryWhenNoDeadLock_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Retry.SetDefaultCriteriaToRetry(RetryDefaultCriteria.SqlServer_DeadLock);

                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                int count = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod()]
        public void RetryUsingOptionsWhenNoDeadLock_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Retry.Activate(options =>
                {
                    options.SetDefaultCriteriaToRetry(RetryDefaultCriteria.SqlServer_DeadLock);
                    options.MillisecondsBetweenTwoRetries = 1000;
                    options.NumberOfRetriesBeforeFailed = 3;
                });

                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                int count = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod()]
        public void RetryWhenDeadLockOccured_Test()
        {
            DbException ex = this.RaiseSqlDeadLock(withRetry: true, throwException: false);

            Assert.IsNull(ex);
        }

        private DbException RaiseSqlDeadLock(bool withRetry, bool throwException)
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

                    cmd.CommandText = @" CREATE TABLE ##Employees ( 
                                             EmpId INT IDENTITY, 
                                             EmpName VARCHAR(16), 
                                             Phone VARCHAR(16) 
                                         ) 

                                         INSERT INTO ##Employees (EmpName, Phone) 
                                         VALUES('Martha', '800-555-1212'), ('Jimmy', '619-555-8080') 

                                         CREATE TABLE ##Suppliers ( 
                                             SupplierId INT IDENTITY, 
                                             SupplierName VARCHAR(64), 
                                             Fax VARCHAR(16) 
                                         ) 

                                         INSERT INTO ##Suppliers (SupplierName, Fax) 
                                         VALUES ('Acme', '877-555-6060'), ('Rockwell', '800-257-1234') ";

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

                        cmd1.CommandText = " UPDATE ##Employees SET EmpName = 'Mary'    WHERE empid = 1 ";
                        cmd1.ExecuteNonQuery();

                        cmd2.CommandText = " UPDATE ##Suppliers SET Fax = N'555-1212'   WHERE supplierid = 1 ";
                        cmd2.ExecuteNonQuery();

                        // Start and when cmd2.ExecuteNonQuery command will be executed, an DeadLock exception will be raised.
                        Task task1 = Task.Factory.StartNew(() =>
                        {
                            cmd1.ThrowException = throwException;
                            if (withRetry)
                            {
                                cmd1.Retry.SetDefaultCriteriaToRetry(RetryDefaultCriteria.SqlServer_DeadLock);
                            }
                            cmd1.CommandText = " UPDATE ##Suppliers SET Fax = N'555-1212'   WHERE supplierid = 1 ";
                            cmd1.ExecuteNonQuery();
                        });

                        System.Threading.Thread.Sleep(500);

                        cmd2.CommandText = " UPDATE ##Employees SET phone = N'555-9999' WHERE empid = 1 ";
                        cmd2.ExecuteNonQuery();

                        cmd2.Dispose();
                        connection2.Close();

                        // Wait cmd1 finished (and raised an Exception)
                        task1.Wait();

                        exToReturn = cmd1.Exception;
                    }
                }

                DropTemporaryTable();
            }
            finally
            {
                connection2.Close();
                connection2.Dispose();
                connection2 = null;
            }

            return exToReturn;
        }

        private void DropTemporaryTable()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;

                cmd.CommandText = @" DROP TABLE ##Employees
                                     DROP TABLE ##Suppliers ";
                cmd.ExecuteNonQuery();
            }
        }
    }
}

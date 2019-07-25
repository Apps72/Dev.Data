using Apps72.Dev.Data;
using Data.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Transactions;

namespace Data.Core.Tests
{
    /*
    To run these tests, you must have the SCOTT database (scott.sql)
    and you need to configure your connection string (configuration.cs)
    */
    [TestClass]
    public class TransactionTests
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

        [TestMethod]
        public void Transaction_GetInternalTransaction_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                var trans1 = DataExtensions.GetTransaction(_connection);

                var trans2A = cmd.TransactionBegin();
                var trans2B = DataExtensions.GetTransaction(_connection);

                cmd.ExecuteNonQuery();
                cmd.TransactionRollback();

                var trans3 = DataExtensions.GetTransaction(_connection);

                Assert.AreEqual(EMP.GetEmployeesCount(_connection), 14);
                Assert.AreEqual(null, trans1);
                Assert.AreEqual(trans2A, trans2B);
                Assert.AreEqual(null, trans3);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Transaction_TwoBeginTransaction_Test()
        {
            // Main Transaction
            using (var cmd1 = new DatabaseCommand(_connection))
            {
                cmd1.Log = Console.WriteLine;
                var trans1 = cmd1.TransactionBegin();

                // Second Transaction
                using (var cmd2 = new DatabaseCommand(trans1))
                {
                    // Raise an exception
                    var trans2 = cmd2.TransactionBegin();
                }

                cmd1.TransactionRollback();
            }
        }

        //[TestMethod]
        //public void Transaction_TransactionScope_Test()
        //{
        //    var trans1 = _connection.BeginTransaction("T1");
        //    using (var cmd1 = _connection.CreateCommand())
        //    {
        //        cmd1.Transaction = trans1;
        //        cmd1.CommandText = "INSERT INTO DEPT (DEPTNO, DNAME) VALUES (91, 'Test1')";
        //        cmd1.ExecuteNonQuery();
        //    }

        //    var trans2 = _connection.BeginTransaction("T2");
        //    using (var cmd2 = _connection.CreateCommand())
        //    {
        //        cmd2.Transaction = trans2;
        //        cmd2.CommandText = "INSERT INTO DEPT (DEPTNO, DNAME) VALUES (92, 'Test2')";
        //        cmd2.ExecuteNonQuery();
        //    }
        //}


        //[TestMethod]
        //public void Transaction_TransactionScope_Test()
        //{
        //    // https://docs.microsoft.com/en-us/dotnet/api/system.transactions.transactionscope

        //    using (var scope = new TransactionScope())
        //    {
        //        using (var cmd1 = new DatabaseCommand(_connection))
        //        {
        //            cmd1.Log = Console.WriteLine;
        //            cmd1.CommandText = "INSERT INTO DEPT (DEPTNO, DNAME) VALUES (90, 'Test1')";
        //            cmd1.ExecuteNonQuery();
        //        }
        //    }

        //    var deptExisting = new DatabaseCommand(_connection)
        //    {
        //        CommandText = "SELECT COUNT(*) FROM DEPT WHERE DEPTNO = 90"
        //    }.ExecuteScalar<int>();

        //    Assert.AreEqual(0, deptExisting);
        //}
    }
}

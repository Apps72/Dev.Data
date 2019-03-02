using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace Data.Core.Tests
{
    /*
        To run these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class ExecuteNonQueryTests
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
        public void ExecuteNonQuery_Transaction_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                cmd.TransactionBegin();
                cmd.ExecuteNonQuery();
                cmd.TransactionRollback();

                Assert.AreEqual(EMP.GetEmployeesCount(_connection), 14);
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_DefineTransactionBefore_Test()
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var cmd = new DatabaseCommand(transaction))
                {
                    cmd.Log = Console.WriteLine;
                    cmd.CommandText.AppendLine(" INSERT INTO EMP (EMPNO, ENAME) VALUES (1234, 'ABC') ");
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new DatabaseCommand(transaction))
                {
                    cmd.Log = Console.WriteLine;
                    cmd.CommandText.AppendLine(" INSERT INTO EMP (EMPNO, ENAME) VALUES (9876, 'XYZ') ");
                    cmd.ExecuteNonQuery();
                }

                transaction.Rollback();

                Assert.AreEqual(EMP.GetEmployeesCount(_connection), 14);
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoCommands_Test()
        {
            DbTransaction currentTransaction;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                currentTransaction = cmd.TransactionBegin();
                cmd.ExecuteNonQuery();

                Assert.AreEqual(EMP.GetEmployeesCount(currentTransaction), 0);     // Inside the transaction

                cmd.TransactionRollback();

                Assert.AreEqual(EMP.GetEmployeesCount(_connection), 14);           // Ouside the transaction
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoIncludedCommands_Test()
        {
            using (var cmd1 = new DatabaseCommand(_connection))
            {
                cmd1.Log = Console.WriteLine;
                cmd1.CommandText.AppendLine(" DELETE FROM EMP ");
                cmd1.TransactionBegin();
                cmd1.ExecuteNonQuery();

                using (var cmd2 = new DatabaseCommand(cmd1.Transaction))
                {
                    cmd2.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    int count = cmd2.ExecuteScalar<int>();
                }

                cmd1.TransactionRollback();
            }
        }

    }
}

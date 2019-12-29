using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Core.Tests
{
    /*
        To run these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class ActionsBeforeAfterTests
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
        public void ExecuteNonQuery_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    command.CommandText.Clear();
                    command.CommandText.Append("SELECT 1+1 FROM EMP");
                    isPassed = true;
                };

                cmd.ExecuteNonQuery();

                Assert.IsTrue(isPassed);
                Assert.AreEqual("SELECT 1+1 FROM EMP", cmd.CommandText.ToString());
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionAfterExecution = (command, tables) =>
                {
                    Assert.AreEqual(-1, tables.First().Rows[0][0]);
                    isPassed = true;
                };

                int rowsAffected = cmd.ExecuteNonQuery();

                Assert.AreEqual(-1, rowsAffected);
                Assert.IsTrue(isPassed);
            }
        }

        [TestMethod]
        public void ExecuteScalar_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    command.CommandText.Clear();
                    command.CommandText.Append("SELECT 1+1 FROM EMP");      // New Count
                    isPassed = true;
                };

                int count = cmd.ExecuteScalar<int>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(2, count);                                  // Check new Count
            }
        }

        [TestMethod]
        public void ExecuteScalar_ActionBefore_ChangeParameter_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT SAL FROM EMP WHERE EMPNO = @EmployeeID");
                cmd.AddParameter("@EmployeeID", 1234);

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.Parameters["@EmployeeID"].Value = 7369;
                    isPassed = true;
                };

                var salary = cmd.ExecuteScalar<decimal>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(800, salary);                                  // Check Salary for 7369 (and not 1234)
            }
        }

        [TestMethod]
        public void ExecuteScalar_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionAfterExecution = (command, tables) =>
                {
                    isPassed = true;
                };

                int count = cmd.ExecuteScalar<int>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(14, count);                                  // Check new Count
            }
        }

        [TestMethod]
        public void ExecuteRow_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.CommandText.AppendLine(" WHERE EMPNO = 7369 ");
                    isPassed = true;
                };

                var row = cmd.ExecuteRow<EMP>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual("SMITH", row.EName);
            }
        }

        [TestMethod]
        public void ExecuteRow_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionAfterExecution = (command, tables) =>
                {
                    isPassed = true;
                };

                var row = cmd.ExecuteRow<EMP>();

                Assert.IsTrue(isPassed);
            }
        }

        [TestMethod]
        public void ExecuteTable_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.CommandText.AppendLine(" WHERE EMPNO > 7369 ");
                    isPassed = true;
                };

                var data = cmd.ExecuteTable<EMP>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(13, data.Count());
                Assert.AreEqual("ALLEN", data.First().EName);
            }
        }

        [TestMethod]
        public void ExecuteTable_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionAfterExecution = (command, tables) =>
                {
                    isPassed = true;
                };

                var data = cmd.ExecuteTable<EMP>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(14, data.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP; ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT; ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.CommandText.Replace("FROM EMP;", "FROM EMP WHERE EMPNO > 7369; ");
                    isPassed = true;
                };

                var data = cmd.ExecuteDataSet<EMP, DEPT>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(13, data.Item1.Count());
                Assert.AreEqual(4, data.Item2.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP; ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT; ");

                cmd.ActionAfterExecution = (command, tables) =>
                {
                    isPassed = true;
                };

                var data = cmd.ExecuteDataSet<EMP, DEPT>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(4, data.Item2.Count());
            }
        }

    }
}

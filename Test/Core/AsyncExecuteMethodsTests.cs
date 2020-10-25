using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Data.Core.Tests
{
    /*
        To run these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class AsyncExecuteMethodsTests
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
        public async Task ExecuteNonQueryAsync_Transaction_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                cmd.TransactionBegin();

                int count = await cmd.ExecuteNonQueryAsync();
                cmd.TransactionRollback();

                Assert.AreEqual(14, count);
                Assert.AreEqual(14, EMP.GetEmployeesCount(_connection));
            }
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsync_NoData_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP WHERE EMPNO = 99999 ");

                var count = await cmd.ExecuteNonQueryAsync();

                Assert.AreEqual(0, count);
            }
        }
    }
}

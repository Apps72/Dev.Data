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

        [TestMethod]
        public async Task ExecuteScalarAsyncObject_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                object data = await cmd.ExecuteScalarAsync();

                Assert.AreEqual(14, data);
            }
        }

        [TestMethod]
        public async Task ExecuteScalarAsyncTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                int data = await cmd.ExecuteScalarAsync<int>();

                Assert.AreEqual(14, data);
            }
        }

        [TestMethod]
        public async Task ExecuteScalarAsyncDefault_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT NULL FROM EMP";
                int data = await cmd.ExecuteScalarAsync<int>();

                Assert.AreEqual(0, data);
            }
        }

        [TestMethod]
        public async Task ExecuteRowTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT * FROM EMP WHERE EMPNO = 7369";
                EMP emp = await cmd.ExecuteRowAsync<EMP>();

                Assert.AreEqual(7369, emp.EmpNo);
            }
        }

        [TestMethod]
        public async Task ExecuteRowPrimitive_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT EMPNO FROM EMP WHERE EMPNO = 7369";
                int emp = await cmd.ExecuteRowAsync<int>();

                Assert.AreEqual(7369, emp);
            }
        }

        [TestMethod]
        public async Task ExecuteRowAnonymous_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT EName, Empno FROM EMP WHERE EMPNO = 7369";
                var emp = await cmd.ExecuteRowAsync(new 
                {                    
                    EName = default(string),
                    Empno = default(int)
                });

                Assert.AreEqual(7369, emp.Empno);
            }
        }

        [TestMethod]
        public async Task ExecuteRowFunction_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT EName, Empno FROM EMP WHERE EMPNO = 7369";
                var emp = await cmd.ExecuteRowAsync(row => 
                {
                    return new
                    {
                        Empno = row.Field<int>("Empno")
                    };
                });

                Assert.AreEqual(7369, emp.Empno);
            }
        }

        [TestMethod]
        public async Task ExecuteRowFunctionAsync_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT EName, Empno FROM EMP WHERE EMPNO = 7369";
                var emp = await cmd.ExecuteRowAsync(async row =>
                {
                    return await Task.Run(() => 
                    {
                        return new
                        {
                            Empno = row.Field<int>("Empno")
                        };
                    });
                    
                });

                Assert.AreEqual(7369, emp.Empno);
            }
        }
    }
}

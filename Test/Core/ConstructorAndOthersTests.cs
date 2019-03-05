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
    public class ConstructorAndOthersTests
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
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullConnection_Test()
        {
            var cmd = new DatabaseCommand(connection: null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_NullTransaction_Test()
        {
            var cmd = new DatabaseCommand(transaction: null);
        }

        [TestMethod]
        public void Constructor_Connection_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";

                Assert.AreEqual(14, cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        [Obsolete]
        public void Constructor_Connection_Timeout_Test()
        {
            using (var cmd = new DatabaseCommand(_connection, 33))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                Assert.AreEqual(14, cmd.ExecuteScalar());
                Assert.AreEqual(33, cmd.CommandTimeout);
            }
        }

        [TestMethod]
        [Obsolete]
        public void Constructor_Connection_CommandText_Test()
        {
            using (var cmd = new DatabaseCommand(_connection, "SELECT COUNT(*) FROM EMP"))
            {
                Assert.AreEqual("SELECT COUNT(*) FROM EMP", cmd.CommandText);
                Assert.AreEqual(14, cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        public void Constructor_Transaction_Test()
        {
            var transaction = _connection.BeginTransaction();
            using (var cmd = new DatabaseCommand(transaction))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";

                Assert.AreEqual(14, cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        [Obsolete]
        public void Constructor_Transaction_CommandText_Test()
        {
            var transaction = _connection.BeginTransaction();
            using (var cmd = new DatabaseCommand(transaction, "SELECT COUNT(*) FROM EMP"))
            {
                Assert.AreEqual("SELECT COUNT(*) FROM EMP", cmd.CommandText);
                Assert.AreEqual(14, cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        [Obsolete]
        public void Constructor_Transaction_Timeout_Test()
        {
            var transaction = _connection.BeginTransaction();
            using (var cmd = new DatabaseCommand(transaction, 33))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                Assert.AreEqual(14, cmd.ExecuteScalar());
                Assert.AreEqual(33, cmd.CommandTimeout);
            }
        }

        [TestMethod]
        [Obsolete]
        public void Constructor_Transaction_CommandText_Timeout_Test()
        {
            var transaction = _connection.BeginTransaction();
            using (var cmd = new DatabaseCommand(transaction, "SELECT COUNT(*) FROM EMP", 33))
            {
                Assert.AreEqual(14, cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        public void Command_Timeout_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                Assert.AreEqual(30, cmd.CommandTimeout);        // Default Value

                cmd.CommandTimeout = 33;
                Assert.AreEqual(33, cmd.CommandTimeout);
            }
        }

        [TestMethod]
        public void Command_ChangeCommandType_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                Assert.AreEqual(System.Data.CommandType.Text, cmd.CommandType);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Assert.AreEqual(System.Data.CommandType.StoredProcedure, cmd.CommandType);
            }
        }
        
        [TestMethod]
        public void Extension_ConvertToDBNull_Test()
        {
            var parameter = new SqlParameter("MyParam", null);
            parameter.ConvertToDBNull();

            Assert.AreEqual(DBNull.Value, parameter.Value);
        }
    }
}

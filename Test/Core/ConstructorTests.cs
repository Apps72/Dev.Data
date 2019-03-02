using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;

namespace Data.Core.Tests
{
    /*
        To run these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class ConstructorTests
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
        public void NullConnection_Test()
        {
            var cmd = new DatabaseCommand(connection: null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullTransaction_Test()
        {
            var cmd = new DatabaseCommand(transaction: null);
        }

       
    }
}

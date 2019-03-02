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
    public class SqlStringTests
    {
        [TestMethod]
        public void ExecuteRowTyped_Test()
        {
            var empty = new SqlString();
            Assert.AreEqual(String.Empty, empty);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;
using System.Text;

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
        public void Empty_Test()
        {
            var query = new SqlString();
            Assert.AreEqual(String.Empty, query);
        }

        [TestMethod]
        public void Value_Test()
        {
            var query = new SqlString("Text");
            Assert.AreEqual("Text", query.Value);
            Assert.AreEqual("Text", query);
        }

        [TestMethod]
        public void Equal_Test()
        {
            var query1 = new SqlString("Text");
            var query2 = new SqlString("Text");
            Assert.IsTrue(query1.Equals(query2));
        }

        [TestMethod]
        public void StringBuilder_Test()
        {
            var query = new SqlString(new StringBuilder("Text"));
            Assert.AreEqual("Text", query);
        }

        [TestMethod]
        public void Append_Test()
        {
            var query = new SqlString();
            query.Append("Hello");
            query.Append("World");
            Assert.AreEqual("HelloWorld", query);
        }

        [TestMethod]
        public void AppendLine_Test()
        {
            var query = new SqlString();
            query.AppendLine("Hello");
            query.AppendLine("World");
            Assert.AreEqual($"Hello{Environment.NewLine}World{Environment.NewLine}", query);
        }

        [TestMethod]
        public void AppendFormat_Test()
        {
            var query = new SqlString();
            query.AppendFormat("Hello{0}", 123);
            query.AppendFormat("World{0}", 456);
            Assert.AreEqual("Hello123World456", query);
        }

        [TestMethod]
        public void AppendLineFormat_Test()
        {
            var query = new SqlString();
            query.AppendLineFormat("Hello{0}", 123);
            query.AppendLineFormat("World{0}", 456);
            Assert.AreEqual($"Hello123{Environment.NewLine}World456{Environment.NewLine}", query);
        }

        [TestMethod]
        public void Clear_Test()
        {
            var query = new SqlString("Hello");
            query.Clear();
            Assert.AreEqual(String.Empty, query);
        }

        [TestMethod]
        public void ToString_Test()
        {
            var query = new SqlString("Hello");
            Assert.AreEqual("Hello", query.ToString());
        }
    }
}

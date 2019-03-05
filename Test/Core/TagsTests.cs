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
    public class TagsTests
    {
        private static readonly string NEW_LINE = Environment.NewLine;

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
        public void SimpleTag_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.WithTag("My command");
                cmd.CommandText = "SELECT * FROM EMP";

                cmd.ActionBeforeExecution = (query) =>
                {
                    Assert.AreEqual($"My command", query.Tags.First());
                    Assert.AreEqual($"SELECT * FROM EMP", query.CommandText);
                    Assert.AreEqual($"-- My command{NEW_LINE}SELECT * FROM EMP", query.Formatted.CommandAsText);
                };

                cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void MultipleTags_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.WithTag("Tag1");
                cmd.WithTag("Tag2");
                cmd.CommandText = "SELECT * FROM EMP";

                cmd.ActionBeforeExecution = (query) =>
                {
                    Assert.AreEqual($"Tag1", query.Tags.ElementAt(0));
                    Assert.AreEqual($"Tag2", query.Tags.ElementAt(1));
                    Assert.AreEqual($"SELECT * FROM EMP", query.CommandText);
                    Assert.AreEqual($"-- Tag1{NEW_LINE}-- Tag2{NEW_LINE}SELECT * FROM EMP", query.Formatted.CommandAsText);
                };

                cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void TagMultilineTags_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.WithTag($"Tag1{NEW_LINE}Tag2");
                cmd.CommandText = "SELECT * FROM EMP";

                cmd.ActionBeforeExecution = (query) =>
                {
                    Assert.AreEqual($"Tag1", query.Tags.ElementAt(0));
                    Assert.AreEqual($"Tag2", query.Tags.ElementAt(1));
                    Assert.AreEqual($"SELECT * FROM EMP", query.CommandText);
                    Assert.AreEqual($"-- Tag1{NEW_LINE}-- Tag2{NEW_LINE}SELECT * FROM EMP", query.Formatted.CommandAsText);
                };

                cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void UpdateTagAfterExecution_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;

                // Tag 1
                cmd.WithTag("Tag1");
                cmd.CommandText = "SELECT * FROM EMP";

                cmd.ActionBeforeExecution = (query) =>
                {
                    Assert.AreEqual($"Tag1", query.Tags.First());
                    Assert.AreEqual($"SELECT * FROM EMP", query.CommandText);
                    Assert.AreEqual($"-- Tag1{NEW_LINE}SELECT * FROM EMP", query.Formatted.CommandAsText);
                };

                cmd.ExecuteNonQuery();

                // Tag 2
                cmd.WithTag("Tag2");
                cmd.CommandText = "SELECT * FROM EMP";

                cmd.ActionBeforeExecution = (query) =>
                {
                    Assert.AreEqual($"Tag1", query.Tags.First());
                    Assert.AreEqual($"SELECT * FROM EMP", query.CommandText);
                    Assert.AreEqual($"-- Tag1{NEW_LINE}-- Tag2{NEW_LINE}SELECT * FROM EMP", query.Formatted.CommandAsText);
                };

                cmd.ExecuteNonQuery();
            }
        }
    }
}

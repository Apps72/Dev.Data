using System;
using System.Data.SqlClient;
using Apps72.Dev.Data;
using Data.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Annotations
{
    /*
        To run most of these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class IgnoreAttributeTests
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

        #endregion{

        [TestMethod]
        public void ToParameters_WhenIgnoreAttribute_DontAddParameter()
        {
            // Arrange
            var data = new DEPTWithIgnoredProperty() {
                DeptNo = 70,
                Loc = "BRUSSELS"
            };
            var cmd = new DatabaseCommand(_connection);

            // Act
            cmd.AddParameter(data);

            // Assert
            Assert.AreEqual(2, cmd.Parameters.Count);
            Assert.AreEqual("@DeptNo", cmd.Parameters[0].ParameterName);
            Assert.AreEqual("@Loc", cmd.Parameters[1].ParameterName);
        }

        [TestMethod]
        public void DatabaseRead_WhenIgnoreAttribute_IgnoreProperty()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT * FROM DEPT WHERE DEPTNO = 10";
                var result = cmd.ExecuteRow<DEPTWithIgnoredProperty>();

                Assert.AreEqual(10, result.DeptNo);
                Assert.AreEqual("IgnoredValue", result.DName);
                Assert.AreEqual("NEW YORK", result.Loc);
            }
        }
    }

    internal class DEPTWithIgnoredProperty
    {
        public virtual int DeptNo { get; set; }

        [Apps72.Dev.Data.Annotations.Ignore]
        public virtual string DName { get; set; } = "IgnoredValue";

        public virtual string Loc { get; set; }
    }
}
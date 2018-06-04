﻿using Apps72.Dev.Data;
using Data.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Core.Tests
{
    [TestClass]
    public class FluentQueryTests
    {
        #region INITIALIZATION

        private SqlConnection _connection;

        [TestInitialize]
        public void Initialization()
        {
            _connection = new SqlConnection(Configuration.CONNECTION_STRING);
            _connection.Open();
        }

        #endregion

        [TestMethod]
        public void ForSql_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query()
                               .ForSql("SELECT COUNT(*) FROM EMP")
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void CompactFluent_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP")
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void CompactFluent_WithParameters_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID", 
                                      new { ID = 10 })
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void Parameters_NameValue_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID")
                               .AddParameter("ID", 10)
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void Parameters_NameValueDbtype_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID")
                               .AddParameter("ID", 10, System.Data.DbType.Int32)
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void Parameters_Dynamic_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID")
                               .AddParameter(new { ID = 10 })
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ExecuteScalar_Simple_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                int count = cmd.Query("SELECT COUNT(*) FROM EMP")
                               .ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ExecuteScalar_Object_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                object count = cmd.Query("SELECT COUNT(*) FROM EMP")
                                  .ExecuteScalar();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ExecuteRow_Simple_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                var emp = cmd.Query("SELECT * FROM EMP WHERE EMPNO = 7369")
                             .ExecuteRow<EMP>();

                Assert.AreEqual("SMITH", emp.EName);
            }
        }

        [TestMethod]
        public void ExecuteRow_Lambda_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                var emp = cmd.Query("SELECT EName FROM EMP WHERE EMPNO = 7369")
                             .ExecuteRow(new { EName = String.Empty});

                Assert.AreEqual("SMITH", emp.EName);
            }
        }
    }
}

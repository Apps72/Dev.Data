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
    public class FormattedTextTests
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
        public void GetFormattedAsText_Simple_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = " SELECT * FROM EMP ";
                Assert.AreEqual(" SELECT * FROM EMP ", cmd.Formatted.CommandAsText);
            }
        }

        [TestMethod]
        public void GetFormattedAsText_WithTags_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.TagWith("MyTag");
                cmd.CommandText = " SELECT * FROM EMP ";
                Assert.AreEqual($"-- MyTag{Environment.NewLine} SELECT * FROM EMP ", cmd.Formatted.CommandAsText);
            }
        }

        [TestMethod]
        public void GetFormattedAsText_Parameters_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = " SELECT *, @MyGuid FROM EMP WHERE EMPNO = @EmpNo AND ENAME LIKE @Ename AND HIREDATE > @Hire AND COMM = @Comm ";

                cmd.AddParameter("@EmpNo", 7369);                                                  // Parameter normal
                cmd.AddParameter("@ENAME", "%SM%");                                                // Parameter in Upper Case
                cmd.AddParameter("Hire", new DateTime(1970, 05, 04, 14, 15, 16));                  // Parameter without @
                cmd.AddParameter("@Comm", null);                                                   // Parameter NULL
                cmd.AddParameter("@MyGuid", new Guid("2fff1b89-b5f9-4a33-ac5b-a3ffee3e8b82"));     // Parameter GUID

                string formatted = cmd.Formatted.CommandAsText;

                Assert.AreEqual(" SELECT *, '2fff1b89-b5f9-4a33-ac5b-a3ffee3e8b82' FROM EMP WHERE EMPNO = 7369 AND ENAME LIKE '%SM%' AND HIREDATE > '1970-05-04 14:15:16' AND COMM = NULL ", formatted);
            }
        }

        [TestMethod]
        public void GetFormattedAsHtml_Parameters_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
                cmd.CommandText.AppendLine("  WHERE EMPNO = @EmpNo AND ENAME LIKE @Ename AND HIREDATE > @Hire AND COMM = @Comm ");

                cmd.AddParameter("@EmpNo", 7369);                                    // Parameter normal
                cmd.AddParameter("@ENAME", "%SM%");                                  // Parameter in Upper Case
                cmd.AddParameter("Hire", new DateTime(1970, 05, 04, 14, 15, 16));    // Parameter without @
                cmd.AddParameter("@Comm", null);                                     // Parameter NULL

                string formatted = cmd.Formatted.CommandAsHtml;

                Assert.AreEqual(formatted, @" <span style=""color: #33f; font-weight: bold;"">SELECT</span> * <span style=""color: #33f; font-weight: bold;"">FROM</span> EMP <br/>  <span style=""color: #33f; font-weight: bold;"">WHERE</span> EMPNO = <span style=""color: #FF3F00;"">7369</span> <span style=""color: #33f; font-weight: bold;"">AND</span> ENAME <span style=""color: #33f; font-weight: bold;"">LIKE</span> <span style=""color: #FF3F00;"">'%SM%'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> HIREDATE &gt; <span style=""color: #FF3F00;"">'<span style=""color: #FF3F00;"">1970</span><span style=""color: #FF3F00;"">-05</span><span style=""color: #FF3F00;"">-04</span> <span style=""color: #FF3F00;"">14</span>:<span style=""color: #FF3F00;"">15</span>:<span style=""color: #FF3F00;"">16</span>'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> COMM = <span style=""color: #33f; font-weight: bold;"">NULL</span> <br/>");
            }
        }

        [TestMethod]
        public void GetFormattedAsVariables_Parameters_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.TagWith("Sample");

                cmd.CommandText.AppendLine(" SET ANSI_NULLS OFF");
                cmd.CommandText.AppendLine(" SELECT * FROM EMP");
                cmd.CommandText.AppendLine("  WHERE EMPNO = @EmpNo");
                cmd.CommandText.AppendLine("    AND ENAME LIKE @Ename");
                cmd.CommandText.AppendLine("    AND HIREDATE > @Hire");
                cmd.CommandText.AppendLine("    AND COMM = @Comm");

                cmd.AddParameter("@EmpNo", 7369);                                    // INT
                cmd.AddParameter("@ENAME", "%SM%");                                  // VARCHAR(6)
                cmd.AddParameter("Hire", new DateTime(1970, 05, 04, 14, 15, 16));    // DATETIME
                cmd.AddParameter("@Comm", null, System.Data.DbType.Currency);        // VARCHAR
                cmd.AddParameter("@MyBool", true);                                   // BIT

                string formatted = cmd.Formatted.CommandAsVariables;

                Assert.AreEqual(formatted,
@"-- Sample
DECLARE @Hire AS DATETIME = '1970-05-04 14:15:16'
DECLARE @MyBool AS BIT = 1
DECLARE @ENAME AS VARCHAR(4) = '%SM%'
DECLARE @EmpNo AS INT = 7369
DECLARE @Comm AS VARCHAR(4000) = NULL

 SET ANSI_NULLS OFF
 SELECT * FROM EMP
  WHERE EMPNO = @EmpNo
    AND ENAME LIKE @Ename
    AND HIREDATE > @Hire
    AND COMM = @Comm
");
            }
        }

        [TestMethod]
        public void GetFormatted_Unknown_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText.AppendLine(" SELECT * FROM EMP");

                string formatted = cmd.Formatted.GetSqlFormatted((QueryFormat)999);

                Assert.AreEqual(String.Empty, formatted);
            }
        }

        [TestMethod]
        public void GetFormattedAsText_TimeSpan_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = " SELECT * FROM EMP WHERE MyCol > @MyTime ";
                cmd.AddParameter("@MyTime", new TimeSpan(2, 30, 00));    // 2:30:00

                Assert.AreEqual(" SELECT * FROM EMP WHERE MyCol > '02:30:00' ", cmd.Formatted.CommandAsText);
            }
        }
    }
}

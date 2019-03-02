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
        public void GetFormattedAsText_Simple_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText= " SELECT * FROM EMP ";
                string formatted = cmd.GetCommandTextFormatted();

                Assert.AreEqual(formatted, " SELECT * FROM EMP ");
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

                string formatted = cmd.GetCommandTextFormatted();

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

                string formatted = (cmd as DatabaseCommand).GetCommandTextFormatted(QueryFormat.Html);

                Assert.AreEqual(formatted, @" <span style=""color: #33f; font-weight: bold;"">SELECT</span> * <span style=""color: #33f; font-weight: bold;"">FROM</span> EMP <br/>  <span style=""color: #33f; font-weight: bold;"">WHERE</span> EMPNO = <span style=""color: #FF3F00;"">7369</span> <span style=""color: #33f; font-weight: bold;"">AND</span> ENAME <span style=""color: #33f; font-weight: bold;"">LIKE</span> <span style=""color: #FF3F00;"">'%SM%'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> HIREDATE &gt; <span style=""color: #FF3F00;"">'<span style=""color: #FF3F00;"">1970</span><span style=""color: #FF3F00;"">-05</span><span style=""color: #FF3F00;"">-04</span> <span style=""color: #FF3F00;"">14</span>:<span style=""color: #FF3F00;"">15</span>:<span style=""color: #FF3F00;"">16</span>'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> COMM = <span style=""color: #33f; font-weight: bold;"">NULL</span> <br/>");
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

using Apps72.Dev.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Data.Core.Tests
{
    /*
        To run these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class ExecuteScalarTests
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
        public void ExecuteScalar_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                object data = cmd.ExecuteScalar();

                Assert.AreEqual(14, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithParameter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @"SELECT ENAME
                                      FROM EMP
                                     WHERE EMPNO = @EmpNo
                                       AND HIREDATE = @HireDate
                                       AND JOB = @Job";

                cmd.AddParameter("@EMPNO", 7369);                            // Parameter in Upper Case
                cmd.AddParameter("HireDate", new DateTime(1980, 12, 17));    // Parameter without @
                cmd.AddParameter("@Job", "CLERK");                           // Parameter in normal mode

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithPrepare_Test()
        {
            var sample = new Dictionary<int, string>()
            {
                { 7369, "SMITH"},
                { 7499, "ALLEN"}
            };

            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @"SELECT ENAME
                                      FROM EMP
                                     WHERE EMPNO = @EmpNo ";

                cmd.AddParameter("@EMPNO", 0, System.Data.DbType.Int32);
                cmd.Prepare();

                foreach (var item in sample)
                {
                    cmd.Parameters["@EMPNO"].Value = item.Key;
                    var ename = cmd.ExecuteScalar<string>();

                    Assert.AreEqual(item.Value, ename);
                }
            }
        }

        [TestMethod]
        public void ExecuteScalarWithNullParameter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @"SET ANSI_NULLS OFF 
                                     SELECT COUNT(*) 
                                      FROM EMP 
                                     WHERE COMM = @Comm 
                                    SET ANSI_NULLS ON";

                cmd.AddParameter("@Comm", null);

                int count = cmd.ExecuteScalar<int>();

                Assert.AreEqual(10, count);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithAnonymousParameters_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @" SELECT ENAME 
                                       FROM EMP 
                                      WHERE EMPNO = @EmpNo 
                                        AND HIREDATE = @HireDate 
                                        AND JOB = @Job 
                                        AND 1 = @NotDeleted";

                cmd.AddParameter("@EMPNO", 1234);                            // Parameter in Upper Case
                cmd.AddParameter("HireDate", new DateTime(1980, 1, 1));      // Parameter without @
                cmd.AddParameter("@Job", "FAKE");                            // Parameter in normal mode
                cmd.AddParameter("@NotDeleted", true);                       // Parameter not replaced 

                // Replace previous values wiht these new propery values
                cmd.AddParameter(new
                {
                    EmpNo = 7369,
                    HireDate = new DateTime(1980, 12, 17),
                    Job = "CLERK"
                });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithAnonymousOnlyParameters_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @"SELECT ENAME 
                                      FROM EMP 
                                     WHERE EMPNO = @EmpNo 
                                       AND HIREDATE = @HireDate 
                                       AND JOB = @Job";

                // Replace previous values wiht these new propery values
                cmd.AddParameter(new
                {
                    EmpNo = 7369,
                    HireDate = new DateTime(1980, 12, 17),
                    Job = "CLERK"
                });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteScalarWithSimpleParameters_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ");


                // Simple value are not autorized
                cmd.AddParameter(123);

                object data = cmd.ExecuteScalar();

                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecuteScalarWithObjectParameter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @" SELECT ENAME 
                                       FROM EMP 
                                      WHERE EMPNO = @EmpNo ";

                // Add manual parameter
                cmd.AddParameter(new { EmpNo = 7369 });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithDbParameter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @" SELECT ENAME
                                       FROM EMP
                                      WHERE EMPNO = @EmpNo";

                // Add manual parameter
                cmd.Parameters.Add(new SqlParameter("@EmpNo", 7369));

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithParameterTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @" SELECT ENAME
                                       FROM EMP
                                      WHERE EMPNO = @EmpNo";

                // Add manual parameter
                cmd.AddParameter("@EmpNo", 7369, System.Data.DbType.Int32, 4);

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT COUNT(*) FROM EMP";
                int data = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarTypedNull_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT COMM FROM EMP WHERE EMPNO = 7369 ";
                int? data = cmd.ExecuteScalar<int?>();

                Assert.AreEqual(null, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWhereNoDataFound_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT COMM FROM EMP WHERE EMPNO = 99999 ";
                int? data = cmd.ExecuteScalar<int?>();

                Assert.AreEqual(null, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarDynamic_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT COUNT(*) FROM EMP ";
                var count = cmd.ExecuteScalar<dynamic>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ExecuteScalar_NoData_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT EMPNO FROM EMP WHERE EMPNO = 99999";
                object empno = cmd.ExecuteScalar();

                Assert.AreEqual(null, empno);
            }
        }

        [TestMethod]
        public void ExecuteScalar_NoDataTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT EMPNO FROM EMP WHERE EMPNO = 99999";
                int empno = cmd.ExecuteScalar<int>();

                Assert.AreEqual(0, empno);
            }
        }

        [TestMethod]
        public void ExecuteScalar_NoDataNullableTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT EMPNO FROM EMP WHERE EMPNO = 99999";
                int? empno = cmd.ExecuteScalar<int?>();

                Assert.AreEqual(null, empno);
            }
        }
    }
}

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
    public class ExecuteRowTests
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
        public void ExecuteRowTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = "SELECT * FROM EMP WHERE EMPNO = 7369";
                EMP emp = cmd.ExecuteRow<EMP>();

                Assert.AreEqual(7369, emp.EmpNo);
            }
        }

        [TestMethod]
        public void ExecuteRowWithConverter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT * FROM EMP WHERE EMPNO = 7369";
                EMP emp = cmd.ExecuteRow<EMP>((row) =>
                {
                    return new EMP()
                    {
                        EmpNo = Convert.ToInt32(row["EMPNO"]),
                        EName = Convert.ToString(row["ENAME"])
                    };
                });

                Assert.AreEqual(7369, emp.EmpNo);
            }
        }

        [TestMethod]
        public void ExecuteRowWithAnonymousConverter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME FROM EMP WHERE EMPNO = 7369";
                var emp = cmd.ExecuteRow((row) =>
                {
                    return new
                    {
                        Id = Convert.ToInt32(row["EMPNO"]),
                        Name = Convert.ToString(row["ENAME"])
                    };
                });

                Assert.AreEqual(7369, emp.Id);
            }
        }

        [TestMethod]
        public void ExecuteRowAnonymousTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE FROM EMP WHERE EMPNO = 7369";
                var emp = cmd.ExecuteRow(new
                {
                    EmpNo = 0,
                    Ename = string.Empty,
                    HireDate = (DateTime?)null
                });

                Assert.AreEqual(7369, emp.EmpNo);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException), "Properties of your anonymous class must be in the same type and same order of your SQL Query.")]
        public void ExecuteRowAnonymousTypedWithExtraProperty_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE FROM EMP WHERE EMPNO = 7369";
                var emp = cmd.ExecuteRow(new
                {
                    EmpNo = 0,
                    Ename = string.Empty,
                    HireDate = (DateTime?)null,
                    UnknowProperty = 0
                });

                Assert.AreEqual(7369, emp.EmpNo);
            }
        }

        [TestMethod]
        public void ExecuteRowPrimitive_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO FROM EMP WHERE EMPNO = 7369";
                int empno = cmd.ExecuteRow<int>();

                Assert.AreEqual(7369, empno);
            }
        }

        [TestMethod]
        public void ExecuteRowPrimitiveWithFunction_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO FROM EMP WHERE EMPNO = 7369";
                int empno = cmd.ExecuteRow<int>((row) => Convert.ToInt32(row[0]));

                Assert.AreEqual(7369, empno);
            }
        }

        [TestMethod]
        public void ExecuteRowPrimitiveNullable_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT COMM FROM EMP WHERE EMPNO = 7369";
                int? comm = cmd.ExecuteRow<int?>();

                Assert.AreEqual(null, comm);
            }
        }

        [TestMethod]
        public void ExecuteRowDynamic_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP WHERE EMPNO = 7369";
                var emp = cmd.ExecuteRow<dynamic>();

                Assert.AreEqual(7369, emp.EMPNO);
                Assert.AreEqual("SMITH", emp.ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp.HIREDATE);
                Assert.AreEqual(null, emp.COMM);
            }
        }

        [TestMethod]
        public void ExecuteStarRowDynamic_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT * FROM EMP WHERE EMPNO = 7369";
                var emp = cmd.ExecuteRow<dynamic>();

                Assert.AreEqual(7369, emp.EMPNO);
                Assert.AreEqual("SMITH", emp.ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp.HIREDATE);
                Assert.AreEqual(null, emp.COMM);
            }
        }

        [TestMethod]
        public void ExecuteTwoRowsDynamic_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP WHERE EMPNO = 7369";
                var emp1 = cmd.ExecuteRow<dynamic>();

                cmd.Clear();
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP WHERE EMPNO = 7499";
                var emp2 = cmd.ExecuteRow<dynamic>();

                Assert.AreEqual(7369, emp1.EMPNO);
                Assert.AreEqual("SMITH", emp1.ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp1.HIREDATE);
                Assert.AreEqual(null, emp1.COMM);

                Assert.AreEqual(7499, emp2.EMPNO);
                Assert.AreEqual("ALLEN", emp2.ENAME);
                Assert.AreEqual(new DateTime(1981, 02, 20), emp2.HIREDATE);
                Assert.AreEqual(300, emp2.COMM);
            }
        }

    }
}

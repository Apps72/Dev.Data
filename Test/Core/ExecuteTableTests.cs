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
    public class ExecuteTableTests
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
        public void ExecuteTableTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP ";
                EMP[] data = cmd.ExecuteTable<EMP>().ToArray();
                EMP smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
                Assert.AreEqual(EMP.Smith.Comm, smith.Comm);
            }
        }

        [TestMethod]
        public void ExecuteTablePrimitive_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT ENAME FROM EMP ";
                string[] data = cmd.ExecuteTable<string>().ToArray();
                string smith = data.FirstOrDefault();

                Assert.AreEqual(EMP.Smith.EName, smith);
            }
        }

        [TestMethod]
        public void ExecuteTablePrimitiveAnonymous_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT ENAME FROM EMP ";
                string[] data = cmd.ExecuteTable(string.Empty).ToArray();
                string smith = data.FirstOrDefault();

                Assert.AreEqual(EMP.Smith.EName, smith);
            }
        }

        [TestMethod]
        public void ExecuteTableNullableProperties_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP ";
                var data = cmd.ExecuteTable(new
                {
                    EmpNo = default(int),
                    EName = default(string),
                    HireDate = default(DateTime?),
                    Comm = (int?)null,
                    Mgr = (int?)4
                });

                var smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
                Assert.AreEqual(EMP.Smith.Comm, smith.Comm);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException), "Properties of your anonymous class must be in the same type and same order of your SQL Query.")]
        public void ExecuteTableCustomedAnonymousTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE FROM EMP ";
                var data = cmd.ExecuteTable(new
                {
                    EmpNo = 0,
                    EName = String.Empty,
                    HireDate = DateTime.Today,
                    MyVar = ""
                });
                var smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
            }
        }

        [TestMethod]
        public void ExecuteTableTypedWithColumnAttribute_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, SAL, HIREDATE, COMM, MGR FROM EMP ";
                var data = cmd.ExecuteTable<EMP>();
                EMP smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(smith.EmpNo, EMP.Smith.EmpNo);
                Assert.AreEqual(smith.EName, EMP.Smith.EName);
                Assert.AreEqual(smith.HireDate, EMP.Smith.HireDate);
                Assert.AreEqual(smith.Comm, EMP.Smith.Comm);
                Assert.AreEqual(smith.Salary, EMP.Smith.Salary);
            }
        }

        [TestMethod]
        public void ExecuteTableWithAnonymousConverter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, SAL, HIREDATE, COMM, MGR  FROM EMP";
                var employees = cmd.ExecuteTable((row) =>
                {
                    return new
                    {
                        Id = row.Field<int>("EMPNO"),
                        Name = row.Field<string>("ENAME"),
                        Salary = row.Field<Decimal>("SAL"),
                        HireDate = row.Field<DateTime>("HIREDATE"),
                        Comm = row.Field<int?>("COMM"),
                        Manager = row.Field<int?>("MGR"),
                    };
                });

                var smith = employees.First();

                Assert.AreEqual(14, employees.Count());
                Assert.AreEqual(EMP.Smith.EmpNo, smith.Id);
                Assert.AreEqual(EMP.Smith.Salary, smith.Salary);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
                Assert.AreEqual(EMP.Smith.Comm, smith.Comm);
                Assert.AreEqual(EMP.Smith.Manager, smith.Manager);

            }
        }

        [TestMethod]
        public void ExecuteTablePrimitiveWithAnonymousConverter_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT ENAME FROM EMP";
                var employees = cmd.ExecuteTable((row) => row.Field<string>("ENAME"));
                var smith = employees.First();

                Assert.AreEqual(14, employees.Count());
                Assert.AreEqual(EMP.Smith.EName, smith);
            }
        }

        [TestMethod]
        public void ExecuteTableDynamic_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = " SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP ORDER BY EMPNO";
                var emp = cmd.ExecuteTable<dynamic>();

                Assert.AreEqual(14, emp.Count());
                Assert.AreEqual("SMITH", emp.First().ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp.First().HIREDATE);
                Assert.AreEqual(null, emp.First().COMM);
            }
        }

    }
}

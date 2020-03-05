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
    public class ExecuteDatasetTests
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
        public void ExecuteSystemDataSet_TwoTables_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT ");

                var dataset = cmd.ExecuteDataSet();
                var smith = dataset.Tables[0].Rows[0];
                var accounting = dataset.Tables[1].Rows[0];

                Assert.AreEqual(EMP.Smith.EmpNo, smith["EmpNo"]);
                Assert.AreEqual(EMP.Smith.EName, smith["EName"]);
                Assert.AreEqual(DEPT.Accounting.DName, accounting["DName"]);
                Assert.AreEqual(DEPT.Accounting.Loc, accounting["Loc"]);
            }
        }

        [TestMethod]
        public void ExecuteSystemDataSet_TableNames_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO  FROM EMP ");                     // With rows
                cmd.CommandText.AppendLine(" SELECT DEPTNO FROM DEPT WHERE DEPTNO = 0 ");   // No rows

                var dataset = cmd.ExecuteDataSet();

                Assert.AreEqual("EMP", dataset.Tables[0].TableName);
                Assert.AreEqual("DEPT", dataset.Tables[1].TableName);
            }
        }

        [TestMethod]
        public void ExecuteSystemDataSet_OneTable_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");

                var dataset = cmd.ExecuteDataSet();
                var smith = dataset.Tables[0].Rows[0];

                Assert.AreEqual(EMP.Smith.EmpNo, smith["EmpNo"]);
                Assert.AreEqual(EMP.Smith.EName, smith["EName"]);
            }
        }

        [TestMethod]
        public void ExecuteSystemDataSet_JoinForTableName_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @" SELECT EMP.ENAME, 
                                            DEPT.DNAME
                                       FROM EMP 
                                      INNER JOIN DEPT ON DEPT.DEPTNO = EMP.DEPTNO ";

                var dataset = cmd.ExecuteDataSet();

                Assert.AreEqual("Table1", dataset.Tables[0].TableName);
            }
        }

        [TestMethod]
        public void ExecuteSystemDataSet_CountForTableName_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText = @" SELECT DEPTNO, COUNT(*) FROM EMP GROUP BY DEPTNO ";

                var dataset = cmd.ExecuteDataSet();

                Assert.AreEqual("EMP", dataset.Tables[0].TableName);
            }
        }

        [TestMethod]
        public void ExecuteDataSetTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT ");

                var data = cmd.ExecuteDataSet<EMP, DEPT>();
                var smith = data.Item1.FirstOrDefault(i => i.EmpNo == 7369);
                var accounting = data.Item2.FirstOrDefault(i => i.DeptNo == 10);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(DEPT.Accounting.DName, accounting.DName);
                Assert.AreEqual(DEPT.Accounting.Loc, accounting.Loc);
            }
        }

        [TestMethod]
        public void ExecuteDataSetCustomedTyped_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");
                cmd.CommandText.AppendLine(" SELECT DEPTNO, DNAME FROM DEPT ");

                var data = cmd.ExecuteDataSet
                (
                    new { EmpNo = 0, EName = "" },
                    new { DeptNo = 0, DName = "" }
                );
                var smith = data.Item1.FirstOrDefault(i => i.EmpNo == 7369);
                var accounting = data.Item2.FirstOrDefault(i => i.DeptNo == 10);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(DEPT.Accounting.DeptNo, accounting.DeptNo);
                Assert.AreEqual(DEPT.Accounting.DName, accounting.DName);
            }
        }

        [TestMethod]
        public void ExecuteDataSetTyped_WithSimpleType_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT ");
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                var data = cmd.ExecuteDataSet<EMP, DEPT, int>();
                var smith = data.Item1.FirstOrDefault(i => i.EmpNo == 7369);
                var accounting = data.Item2.FirstOrDefault(i => i.DeptNo == 10);
                var NbOfEmp = data.Item3;

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(DEPT.Accounting.DName, accounting.DName);
                Assert.AreEqual(DEPT.Accounting.Loc, accounting.Loc);
                Assert.AreEqual(14, NbOfEmp.First());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_2Types_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet<EMP, EMP>();

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_3Types_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet<EMP, EMP, EMP>();

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
                Assert.AreEqual(14, data.Item3.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_4Types_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet<EMP, EMP, EMP, EMP>();

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
                Assert.AreEqual(14, data.Item3.Count());
                Assert.AreEqual(14, data.Item4.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_5Types_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet<EMP, EMP, EMP, EMP, EMP>();

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
                Assert.AreEqual(14, data.Item3.Count());
                Assert.AreEqual(14, data.Item4.Count());
                Assert.AreEqual(14, data.Item5.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_2AnonymousTypes_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";
                var emp = new { Empno = 0, Ename = "" };

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet(emp, emp);

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_3AnonymousTypes_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";
                var emp = new { Empno = 0, Ename = "" };

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet(emp, emp, emp);

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
                Assert.AreEqual(14, data.Item3.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_4AnonymousTypes_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";
                var emp = new { Empno = 0, Ename = "" };

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet(emp, emp, emp, emp);

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
                Assert.AreEqual(14, data.Item3.Count());
                Assert.AreEqual(14, data.Item4.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_5AnonymousTypes_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                string sql = " SELECT EMPNO, ENAME FROM EMP; ";
                var emp = new { Empno = 0, Ename = "" };

                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);
                cmd.CommandText.AppendLine(sql);

                var data = cmd.ExecuteDataSet(emp, emp, emp, emp, emp);

                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(14, data.Item2.Count());
                Assert.AreEqual(14, data.Item3.Count());
                Assert.AreEqual(14, data.Item4.Count());
                Assert.AreEqual(14, data.Item5.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_NoData_Test()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP  WHERE EMPNO = 99999");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT WHERE DEPTNO = 99999");

                var data = cmd.ExecuteDataSet<EMP, DEPT>();

                Assert.AreEqual(0, data.Item1.Count());
                Assert.AreEqual(0, data.Item2.Count());
            }
        }
    }
}

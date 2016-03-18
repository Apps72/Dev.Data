using Apps72.Dev.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tests
{
    [TestClass]
    public class SqlDataInjectionTests
    {
        [TestMethod]
        public void DataInjection_WithEmp_Test()
        {
            SqlConnection conn = new SqlConnection();

            conn.DefineDataInjection((cmd) =>
            {
                List<EMPBase> employees = new List<EMPBase>();
                employees.Add(new EMPBase() { EName = "", EmpNo = 1 });
                employees.Add(new EMPBase() { EName = "", EmpNo = 2 });

                cmd.Inject(employees);
            });

            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                DataTable table = cmd.ExecuteTable();

                Assert.AreEqual(table.Rows.Count, 2);
            }
        }

        [TestMethod]
        public void DataInjection_WithEmp_SingleRow_Test()
        {
            SqlConnection conn = new SqlConnection();

            conn.DefineDataInjection((cmd) =>
            {
                cmd.Inject(new EMPBase() { EName = "SMITH", EmpNo = 1 });
            });

            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP WHERE EMPNO = 7369 ");

                DataTable table = cmd.ExecuteTable();

                Assert.AreEqual(table.Rows[0]["ENAME"], "SMITH");
            }
        }

        [TestMethod]
        public void DataInjection_Typed_Test()
        {
            SqlConnection conn = new SqlConnection();
            
            conn.DefineDataInjection((cmd) =>
            {
                List<EMPBase> employees = new List<EMPBase>();
                employees.Add(new EMPBase() { EName = "", EmpNo = 1 });
                employees.Add(new EMPBase() { EName = "", EmpNo = 2 });

                cmd.Inject(employees);
            });

            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                var data = cmd.ExecuteTable<EMPBase>();

                Assert.AreEqual(data.Count(), 2);
            }
        }

        [TestMethod]
        public void DataInjection_WithPrimitiveArray_Test()
        {
            SqlConnection conn = new SqlConnection();

            conn.DefineDataInjection((cmd) =>
            {
                cmd.Inject(new int[] { 2 });
            });

            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                Assert.AreEqual(cmd.ExecuteScalar<int>(), 2);
            }
        }

        [TestMethod]
        public void DataInjection_WithPrimitiveValue_Test()
        {
            SqlConnection conn = new SqlConnection();

            conn.DefineDataInjection((cmd) =>
            {
                cmd.Inject(2);
            });

            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                Assert.AreEqual(cmd.ExecuteScalar<int>(), 2);
            }
        }

    }
}

using Apps72.Dev.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tests
{
    [TestClass]
    public class SqlDataInjectionTests
    {
        //[TestMethod]
        //public void DataInjection_WithDataTable_Test()
        //{
        //    SqlConnection conn = new SqlConnection();

        //    conn.DefineDataInjection((cmd) =>
        //    {
        //        var dt = new DataTable();
        //        dt.Columns.Add("Col1", typeof(int));
        //        dt.Rows.Add(2);
        //        return dt;
        //    });

        //    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
        //    {
        //        cmd.Log = Console.WriteLine;
        //        cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
        //        cmd.Parameters.AddWithValueOrDBNull("@Comm", null);

        //        Assert.AreEqual(cmd.ExecuteScalar<int>(), 2);
        //    }
        //}

        //[TestMethod]
        //public void DataInjection_WithEmp_Test()
        //{
        //    SqlConnection conn = new SqlConnection();

        //    conn.DefineDataInjection((cmd) =>
        //    {
        //        List<EMPBase> employees = new List<EMPBase>();
        //        employees.Add(new EMPBase() { EmpNo = 1 });
        //        employees.Add(new EMPBase() { EmpNo = 2 });
        //        return DataTypedConvertor.ToDataTable(employees);
        //    });

        //    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
        //    {
        //        cmd.Log = Console.WriteLine;
        //        cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

        //        Assert.AreEqual(cmd.ExecuteTable().Rows.Count, 2);
        //    }
        //}

        //[TestMethod]
        //public void DataInjection_WithPrimitive_Test()
        //{
        //    SqlConnection conn = new SqlConnection();

        //    conn.DefineDataInjection((cmd) =>
        //    {
        //        return DataTypedConvertor.ToDataTable(new int[] { 2 });
        //    });

        //    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
        //    {
        //        cmd.Log = Console.WriteLine;
        //        cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

        //        Assert.AreEqual(cmd.ExecuteScalar<int>(), 2);
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data.Generator;
using Apps72.Dev.Data.Schema;
using System.Data.SqlClient;

namespace Data.Tests
{
    [TestClass]
    public class SqlEntitiesGeneratorTests
    {
        private static readonly string CONNECTION_STRING = SqlDatabaseCommandTests.CONNECTION_STRING;

        #region RemoveExtraChars

        [TestMethod]
        public void EntitiesGenerator_RemoveExtraChars_WithSpecialChars_Test()
        {
            var entitiesGenerator = new SqlEntitiesGenerator(String.Empty);

            PrivateObject obj = new PrivateObject(entitiesGenerator);
            var retVal = obj.Invoke("RemoveExtraChars", "Abc@123#xyZ-,_;è|789");

            Assert.AreEqual("Abc123xyZ_789", retVal);
        }

        [TestMethod]
        public void EntitiesGenerator_RemoveExtraChars_FirstCharMustBeALetter_Test()
        {
            var entitiesGenerator = new SqlEntitiesGenerator(String.Empty);

            PrivateObject obj = new PrivateObject(entitiesGenerator);
            var retVal = obj.Invoke("RemoveExtraChars", "1A2B3C");

            Assert.AreEqual("_1A2B3C", retVal);
        }

        [TestMethod]
        public void EntitiesGenerator_RemoveExtraChars_OnlyInvalidChars_Test()
        {
            var entitiesGenerator = new SqlEntitiesGenerator(String.Empty);

            PrivateObject obj = new PrivateObject(entitiesGenerator);
            var retVal = obj.Invoke("RemoveExtraChars", "à{}@#|").ToString();

            Assert.AreEqual('_', retVal[0]);
            Assert.AreEqual(38, retVal.Length);
        }

        #endregion

        #region EntitiesGenerator

        [TestMethod]
        public void EntitiesGenerator_ConstructorWithConnectionStringNameOnly_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator("Scott", "../../App.Config");

            Assert.AreEqual("Server=(localdb)\\ProjectsV12;Database=Scott;Integrated Security=true;", entitiesGenerator.ConnectionString);
        }

        [TestMethod]
        public void EntitiesGenerator_EmployeeTable_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator(CONNECTION_STRING);
            DataTable table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual("EMP", table.Name, "Name=EMP");
            Assert.AreEqual("dbo", table.Schema, "Schema=dbo");
            Assert.AreEqual(false, table.IsView, "IsView=false");
        }

        [TestMethod]
        public void EntitiesGenerator_EmployeeColumns_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator(CONNECTION_STRING);
            DataTable table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual("EMP", table.Name);
            Assert.AreEqual("dbo", table.Schema);
            Assert.AreEqual("dbo_EMP", table.SchemaAndName);
            Assert.AreEqual(false, table.IsView);

            Assert.AreEqual(8, table.Columns.Count());
            Assert.AreEqual(false, table.Columns.First(c => c.ColumnName == "EMPNO").IsNullable);
            Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "ENAME").IsNullable);

            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "EMPNO").CSharpType, "EMPNO");
            Assert.AreEqual("string", table.Columns.First(c => c.ColumnName == "ENAME").CSharpType, "ENAME");
            Assert.AreEqual("string", table.Columns.First(c => c.ColumnName == "JOB").CSharpType, "JOB");
            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "MGR").CSharpType, "MGR");
            Assert.AreEqual("DateTime", table.Columns.First(c => c.ColumnName == "HIREDATE").CSharpType, "HIREDATE");
            Assert.AreEqual("decimal", table.Columns.First(c => c.ColumnName == "SAL").CSharpType, "SAL");
            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "COMM").CSharpType, "COMM");
            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "DEPTNO").CSharpType, "DEPTNO");
            Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "HIREDATE").IsNullable, "HIREDATE");
            Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "SAL").IsNullable, "SAL");

            Assert.AreEqual("Int32", table.Columns.First(c => c.ColumnName == "EMPNO").DotNetType, "EMPNO");
            Assert.AreEqual("String", table.Columns.First(c => c.ColumnName == "ENAME").DotNetType, "ENAME");
            Assert.AreEqual("String", table.Columns.First(c => c.ColumnName == "JOB").DotNetType, "JOB");
            Assert.AreEqual("Int32", table.Columns.First(c => c.ColumnName == "MGR").DotNetType, "MGR");

            Assert.AreEqual("int?", table.Columns.First(c => c.ColumnName == "MGR").CSharpTypeNullable, "MGR");
            Assert.AreEqual("Int32?", table.Columns.First(c => c.ColumnName == "MGR").DotNetTypeNullable, "MGR");
        }

        //[TestMethod]
        //public void EntitiesGeneratorBase_Test()
        //{
        //    using (var conn = new SqlConnection(CONNECTION_STRING))
        //    {
        //        conn.Open();
        //        var entitiesGenerator = new SqlEntitiesGenerator(conn);

        //        DataTable table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

        //        Assert.AreEqual(8, table.Columns.Count());
        //        Assert.AreEqual(false, table.Columns.First(c => c.ColumnName == "EMPNO").IsNullable);
        //        Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "ENAME").IsNullable);

        //        conn.Close();
        //    }
        //}

        #endregion
    }
}

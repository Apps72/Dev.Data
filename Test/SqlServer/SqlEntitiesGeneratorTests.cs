using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data.Generator;
using Apps72.Dev.Data.Schema;

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

            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "EMPNO").CSharpType);
            Assert.AreEqual("string", table.Columns.First(c => c.ColumnName == "ENAME").CSharpType);
            Assert.AreEqual("string", table.Columns.First(c => c.ColumnName == "JOB").CSharpType);
            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "MGR").CSharpType);
            Assert.AreEqual("DateTime", table.Columns.First(c => c.ColumnName == "HIREDATE").CSharpType);
            Assert.AreEqual("decimal", table.Columns.First(c => c.ColumnName == "SAL").CSharpType);
            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "COMM").CSharpType);
            Assert.AreEqual("int", table.Columns.First(c => c.ColumnName == "DEPTNO").CSharpType);
            Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "HIREDATE").IsNullable);
            Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "SAL").IsNullable);

            Assert.AreEqual("Int32", table.Columns.First(c => c.ColumnName == "EMPNO").DotNetType);
            Assert.AreEqual("String", table.Columns.First(c => c.ColumnName == "ENAME").DotNetType);
            Assert.AreEqual("String", table.Columns.First(c => c.ColumnName == "JOB").DotNetType);
            Assert.AreEqual("Int32", table.Columns.First(c => c.ColumnName == "MGR").DotNetType);

            Assert.AreEqual("int?", table.Columns.First(c => c.ColumnName == "MGR").CSharpTypeNullable);
            Assert.AreEqual("Int32?", table.Columns.First(c => c.ColumnName == "MGR").DotNetTypeNullable);

            Assert.AreEqual(System.Data.SqlDbType.VarChar, table.Columns.First(c => c.ColumnName == "ENAME").SqlDbType);
            Assert.AreEqual(System.Data.SqlDbType.Int, table.Columns.First(c => c.ColumnName == "MGR").SqlDbType);
        }

        [TestMethod]
        public void EntitiesGeneratorBase_Test()
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                var entitiesGenerator = new SqlEntitiesGenerator(conn);

                DataTable table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

                Assert.AreEqual(8, table.Columns.Count());
                Assert.AreEqual(false, table.Columns.First(c => c.ColumnName == "EMPNO").IsNullable);
                Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "ENAME").IsNullable);

                conn.Close();
            }
        }

        #endregion
    }
}

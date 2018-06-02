using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data.Generator;
using Apps72.Dev.Data.Schema;

namespace Data.Sqlite.Tests
{
    [TestClass]
    public class SqliteEntitiesGeneratorTests
    {
        private static readonly string CONNECTION_STRING = SqliteDatabaseCommandTests.CONNECTION_STRING;

        [TestInitialize]
        public void Initialization()
        {
            // Set Data directory
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path + "\\Data");
        }

        #region EntitiesGenerator

        [TestMethod]
        public void EntitiesGenerator_ConstructorWithConnectionStringNameOnly_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator("Scott", "../../App.Config");

            Assert.AreEqual("Data Source=|DataDirectory|\\scott.db;Version=3;", entitiesGenerator.ConnectionString);
        }

        [TestMethod]
        public void EntitiesGenerator_EmployeeTable_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator(CONNECTION_STRING);
            DataTable table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual("EMP", table.Name);
            Assert.AreEqual("sqlite_default_schema", table.Schema);
            Assert.AreEqual(false, table.IsView);
        }

        [TestMethod]
        public void EntitiesGenerator_EmployeeColumns_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator(CONNECTION_STRING);
            DataTable table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

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
            Assert.AreEqual("DateTime", table.Columns.First(c => c.ColumnName == "HIREDATE").DotNetType, "HIREDATE");
            Assert.AreEqual("int?", table.Columns.First(c => c.ColumnName == "MGR").CSharpTypeNullable, "MGR");
        }

        #endregion
    }
}

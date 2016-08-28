using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data.Generator;

namespace Data.Tests
{
    [TestClass]
    public class SqlEntitiesGeneratorTests
    {
        private static readonly string CONNECTION_STRING_NAME = "Scott";
        private static readonly string CONNECTION_STRING = SqlDatabaseCommandTests.CONNECTION_STRING;

        [TestMethod]
        public void EntitiesGenerator_EmployeeTable_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator(CONNECTION_STRING);
            Table table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual("EMP", table.Name, "Name=EMP");
            Assert.AreEqual("dbo", table.Schema, "Schema=dbo");
            Assert.AreEqual(false, table.IsView, "IsView=false");
        }

        [TestMethod]
        public void EntitiesGenerator_EmployeeColumns_Test()
        {
            SqlEntitiesGenerator entitiesGenerator = new SqlEntitiesGenerator(CONNECTION_STRING);
            Table table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual(8, table.Columns.Count());
            Assert.AreEqual(false, table.Columns.First(c => c.Name == "EMPNO").IsNullable);
            Assert.AreEqual(true, table.Columns.First(c => c.Name == "ENAME").IsNullable);

            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "EMPNO").CSharpType, "EMPNO");
            Assert.AreEqual("String", table.Columns.First(c => c.Name == "ENAME").CSharpType, "ENAME");
            Assert.AreEqual("String", table.Columns.First(c => c.Name == "JOB").CSharpType, "JOB");
            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "MGR").CSharpType, "MGR");
            Assert.AreEqual("DateTime", table.Columns.First(c => c.Name == "HIREDATE").CSharpType, "HIREDATE");
            Assert.AreEqual("Decimal", table.Columns.First(c => c.Name == "SAL").CSharpType, "SAL");
            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "COMM").CSharpType, "COMM");
            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "DEPTNO").CSharpType, "DEPTNO");
        }

    }
}

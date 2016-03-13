using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests
{
    [TestClass]
    public class SqlEntitiesGenerator
    {
        private const string CONNECTION_STRING = SqlDatabaseCommandTests.CONNECTION_STRING;

        [TestMethod]
        public void EntitiesGenerator_EmployeeTable_Test()
        {
            Apps72.Dev.Data.Generator.SqlEntitiesGenerator entitiesGenerator = new Apps72.Dev.Data.Generator.SqlEntitiesGenerator(CONNECTION_STRING);
            Apps72.Dev.Data.Generator.Table table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual("EMP", table.Name);
            Assert.AreEqual("dbo", table.Schema);
            Assert.AreEqual(false, table.IsView);
        }

        [TestMethod]
        public void EntitiesGenerator_EmployeeColumns_Test()
        {
            Apps72.Dev.Data.Generator.SqlEntitiesGenerator entitiesGenerator = new Apps72.Dev.Data.Generator.SqlEntitiesGenerator(CONNECTION_STRING);
            Apps72.Dev.Data.Generator.Table table = entitiesGenerator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual(8, table.Columns.Count());
            Assert.AreEqual(false, table.Columns.First(c => c.Name == "EMPNO").IsNullable);
            Assert.AreEqual(true, table.Columns.First(c => c.Name == "ENAME").IsNullable);

            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "EMPNO").CSharpType);
            Assert.AreEqual("String", table.Columns.First(c => c.Name == "ENAME").CSharpType);
            Assert.AreEqual("String", table.Columns.First(c => c.Name == "JOB").CSharpType);
            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "MGR").CSharpType);
            Assert.AreEqual("DateTime", table.Columns.First(c => c.Name == "HIREDATE").CSharpType);
            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "SAL").CSharpType);
            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "COMM").CSharpType);
            Assert.AreEqual("Int32", table.Columns.First(c => c.Name == "DEPTNO").CSharpType);
        }

    }
}

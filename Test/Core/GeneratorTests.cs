using Apps72.Dev.Data.Generator;
using Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Data.Core.Tests
{
    /*
        To run these tests, you must have the SCOTT database (scott.sql)
        and you need to configure your connection string (configuration.cs)
    */

    [TestClass]
    public class GeneratorTests
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

        #region RemoveExtraChars



        [TestMethod]
        public void generator_RemoveExtraChars_WithSpecialChars_Test()
        {

            var obj = new PrivateType("Apps72.Dev.Data", "Apps72.Dev.Data.Convertor.TypeExtension");
            var retVal = obj.InvokeStatic("RemoveExtraChars", "Abc@123#xyZ-,_;è|789");
            Assert.AreEqual("Abc_123_xyZ______789", retVal);
        }

        [TestMethod]
        public void generator_RemoveExtraChars_FirstCharMustBeALetter_Test()
        {
            var obj = new PrivateType("Apps72.Dev.Data", "Apps72.Dev.Data.Convertor.TypeExtension");
            var retVal = obj.InvokeStatic("RemoveExtraChars", "1A2B3C");

            Assert.AreEqual("_1A2B3C", retVal);
        }

        [TestMethod]
        public void generator_RemoveExtraChars_OnlyInvalidChars_Test()
        {
            var obj = new PrivateType("Apps72.Dev.Data", "Apps72.Dev.Data.Convertor.TypeExtension");
            var retVal = obj.InvokeStatic("RemoveExtraChars", "à{}@#|").ToString();

            Assert.AreEqual('_', retVal[0]);
            Assert.AreEqual(38, retVal.Length);
        }

        #endregion

        #region generator

        [TestMethod]
        public void generator_EmployeeTable_Test()
        {
            var generator = new EntityGenerator(_connection);
            var table = generator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual("EMP", table.Name);
            Assert.AreEqual("dbo", table.Schema);
            Assert.AreEqual(false, table.IsView);
        }

        [TestMethod]
        public void generator_EmployeeColumns_Test()
        {
            var generator = new EntityGenerator(_connection);
            var table = generator.Tables.FirstOrDefault(t => t.Name == "EMP");

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
        }

        [TestMethod]
        public void generatorBase_Test()
        {
            var generator = new EntityGenerator(_connection);

            var table = generator.Tables.FirstOrDefault(t => t.Name == "EMP");

            Assert.AreEqual(8, table.Columns.Count());
            Assert.AreEqual(false, table.Columns.First(c => c.ColumnName == "EMPNO").IsNullable);
            Assert.AreEqual(true, table.Columns.First(c => c.ColumnName == "ENAME").IsNullable);
        }

        [TestMethod]
        public void generator_EmployeeSalary_MustBeDecimal_Test()
        {
            var generator = new EntityGenerator(_connection);
            var table = generator.Tables.FirstOrDefault(t => t.Name == "EMP");
            var column = table.Columns.First(c => c.ColumnName == "SAL");

            Assert.AreEqual("numeric", column.SqlType);
            Assert.AreEqual(typeof(decimal), column.DataType);
        }

        [TestMethod]
        public void generator_SqlDataType_Test()
        {
            var privateTypeObject = new PrivateType("Apps72.Dev.Data", "Apps72.Dev.Data.Convertor.DbTypeMap");
            privateTypeObject.InvokeStatic("Initialize", _connection);

            Type sqlInt = privateTypeObject.InvokeStatic("FirstType", "int") as Type;
            Type sqlVarchar = privateTypeObject.InvokeStatic("FirstType", "varchar") as Type;
            Type sqlTinyInt = privateTypeObject.InvokeStatic("FirstType", "tinyint") as Type;
            Type sqlBit = privateTypeObject.InvokeStatic("FirstType", "bit") as Type;

            Assert.AreEqual(typeof(int), sqlInt);
            Assert.AreEqual(typeof(string), sqlVarchar);
            Assert.AreEqual(typeof(byte), sqlTinyInt);
            Assert.AreEqual(typeof(bool), sqlBit);
        }

        #endregion
    }
}

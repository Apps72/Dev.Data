using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Collections.Generic;

namespace Data.SqlServerClr.Tests
{
    [TestClass]
    public class SqlClrCommandTests
    {
        [TestInitialize]
        public void Initialize()
        {
            SetupDeployment.Run();            
        }

        [TestMethod]
        public void Simple_HelloWorld_Test()
        {
            using (var cmd = new SqlDatabaseCommand(SetupDeployment.CONNECTION_STRING))
            {
                cmd.CommandText.AppendLine(" EXEC HelloWorld ");
                cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Simple_NoArguments_ReturnsScalar_Test()
        {
            using (var cmd = new SqlDatabaseCommand(SetupDeployment.CONNECTION_STRING))
            {
                cmd.CommandText.AppendLine(" SELECT dbo.GetNumberOfEmployees() ");
                var count = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void Simple_OneArgument_ReturnsScalar_Test()
        {
            using (var cmd = new SqlDatabaseCommand(SetupDeployment.CONNECTION_STRING))
            {
                cmd.CommandText.AppendLine(" SELECT dbo.GetNumberOfEmployeesInDepartement(@DeptNo) ");
                cmd.Parameters.AddWithValue("@DeptNo", 20);

                var count = cmd.ExecuteScalar<int>();

                Assert.AreEqual(5, count);
            }
        }
    }
}

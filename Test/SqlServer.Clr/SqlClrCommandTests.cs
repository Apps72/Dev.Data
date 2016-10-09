using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Collections.Generic;

namespace Data.SqlServerClr.Tests
{
    [TestClass]
    public class SqlClrCommandTests
    {
        public static readonly string CONNECTION_STRING = System.Configuration.ConfigurationManager.ConnectionStrings["Scott"].ConnectionString;

        [TestInitialize]
        public void Initialize()
        {

            using (var cmd = new SqlDatabaseCommand(CONNECTION_STRING))
            {
                var procedures = new List<ProcedureDefinition>();
                procedures.Add(new ProcedureDefinition("HelloWorld", ProcedureType.Procedure, string.Empty, string.Empty));

                cmd.CommandText.Append(SetupDeployment.GetInitializeScript(procedures));
                cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Simple_HelloWorld_Test()
        {
            using (var cmd = new SqlDatabaseCommand(CONNECTION_STRING))
            {
                cmd.CommandText.AppendLine(" EXEC HelloWorld ");
                cmd.ExecuteNonQuery();
            }
        }
    }
}

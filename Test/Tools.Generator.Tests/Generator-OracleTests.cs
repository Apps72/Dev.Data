using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tools.Generator.Tests
{
    [TestClass]
    public class Generator_OracleTests
    {
        [Ignore]
        [TestMethod]
        public void Oracle_DefaultParameters_Test()
        {
            var args = new[] 
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.ORACLE_CONNECTION_STRING}\"",
                $"p=Oracle",
                $"cf=SchemaAndName",
                $"a=\"System.ComponentModel.DataAnnotations.Schema.Column\"",   // Include Column Attribute
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(args);
            var code = generator.Code;

            Assert.IsTrue(code.Contains("public virtual int EMPNO { get; set; }"));
        }
    }
}


namespace Test
{
}

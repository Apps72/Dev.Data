using Apps72.Dev.Data.Generator.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tools.Generator.Tests
{
    [TestClass]
    public class Generator_SqlServerTests
    {
        [TestMethod]
        public void SqlServer_DefaultParameters_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\""
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("public virtual int EMPNO { get; set; }"));
        }

        [TestMethod]
        public void SqlServer_EMP_SAL_MustBeDecimal_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\""
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("public virtual decimal? SAL { get; set; }"));
        }

    }
}

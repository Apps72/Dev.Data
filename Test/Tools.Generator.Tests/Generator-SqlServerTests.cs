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
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\""
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(args);
            var code = generator.Code;

            Assert.IsTrue(code.Contains("public virtual int EMPNO { get; set; }"));
        }
    }
}

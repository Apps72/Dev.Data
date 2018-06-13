using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tools.Generator.Tests
{
    [TestClass]
    public class Generator_OracleTests
    {
        [TestMethod]
        public void Oracle_DefaultParameters_Test()
        {
            var args = new[] 
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.ORACLE_CONNECTION_STRING}\"",
                $"ns=\"Actiris.CoreBusiness.Services.Data.Models.Ibis\"",
                $"p=Oracle",
                $"cf=NameOnly",
                $"ca=\"AV1130, AV1507, AV1704, AV1706, AV1710\"",
                $"os=IBIS",
                $"a=\"System.ComponentModel.DataAnnotations.Schema.Column\"",   // Include Column Attribute
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(args);
            var code = generator.Code;

            Assert.IsTrue(code.Contains("public virtual long ID_JOUR_OUVRE { get; set; }"));
        }
    }
}


namespace Test
{
}

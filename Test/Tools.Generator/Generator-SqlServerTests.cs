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

        [TestMethod]
        public void SqlServer_NullableRefTypes_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"NullableRefTypes"
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("public virtual string? ENAME { get; set; }"));
            Assert.IsTrue(code.Contains("public virtual int? MGR { get; set; }"));
        }

        [TestMethod]
        public void SqlServer_SortProperties_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"SortProperties"
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains(
@"
    public partial class DEPT
    {
        /// <summary />
        public virtual int DEPTNO { get; set; }
        /// <summary />
        public virtual string DNAME { get; set; }
        /// <summary />
        public virtual string LOC { get; set; }
    }
    /// <summary />
    public partial class EMP
    {
        /// <summary />
        public virtual int? COMM { get; set; }
        /// <summary />
        public virtual int? DEPTNO { get; set; }
        /// <summary />
        public virtual int EMPNO { get; set; }
        /// <summary />
        public virtual string ENAME { get; set; }
        /// <summary />
        public virtual DateTime? HIREDATE { get; set; }
        /// <summary />
        public virtual string JOB { get; set; }
        /// <summary />
        public virtual int? MGR { get; set; }
        /// <summary />
        public virtual decimal? SAL { get; set; }
    }"));

        }

        [TestMethod]
        public void SqlServer_ValidationRange_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"Validations=Range"
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("Range(-99999999999999999999999999999999999999.0d, 99999999999999999999999999999999999999.0d)]"));
        }
    }
}

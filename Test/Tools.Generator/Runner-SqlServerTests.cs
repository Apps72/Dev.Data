using Apps72.Dev.Data.Generator.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Tools.Generator.Tests
{
    [TestClass]
    public class Runner_SqlServerTests
    {
        private readonly string FILENAME1 = Path.Join(Environment.CurrentDirectory, "script1.sql");
        private readonly string FILENAME2 = Path.Join(Environment.CurrentDirectory, "script2.sql");

        [TestMethod]
        public void RunnerRunner_SqlServer_DefaultParameters_Test()
        {
            // Sample script
            File.WriteAllText(FILENAME1, "SELECT COUNT(*) FROM EMP");
            File.WriteAllText(FILENAME2, "SELECT COUNT(*) FROM DEPT");

            var args = new[]
            {
                $"Run",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
            };
            var runner = new Runner(new Arguments(args)).Start();

            Assert.AreEqual(2, runner.Files.Count());

            // Remove sample files
            File.Delete(FILENAME1);
            File.Delete(FILENAME2);
        }
    }
}

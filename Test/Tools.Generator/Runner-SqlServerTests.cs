using Apps72.Dev.Data.Generator.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Tools.Generator.Tests
{
    [TestClass]
    public class Runner_SqlServerTests
    {
        private readonly string DB_SELECT_CONFIG = "SELECT Job FROM Bonus WHERE ename = 'DbVer'";
        private readonly string DB_UPDATE_CONFIG = "UPDATE Bonus SET Job = @Filename WHERE ename = 'DbVer'";
        private readonly string FILENAME1 = Path.Join(Environment.CurrentDirectory, "script1.sql");
        private readonly string FILENAME2 = Path.Join(Environment.CurrentDirectory, "script2.sql");

        [TestMethod]
        public void RunnerRunner_SqlServer_EmptyScript_Test()
        {
            // Sample script
            File.WriteAllText(FILENAME1, "-- Hello");
            File.WriteAllText(FILENAME2, "");

            var args = new[]
            {
                $"Run",
                $"-cs={Configuration.SQLSERVER_CONNECTION_STRING}",
            };
            var runner = new Runner(new Arguments(args)).Start();

            Assert.AreEqual(2, runner.Files.Count());

            // Remove sample files
            File.Delete(FILENAME1);
            File.Delete(FILENAME2);

        }

        [TestMethod]
        public void RunnerRunner_SqlServer_DefaultParameters_Test()
        {
            // Sample script
            File.WriteAllText(FILENAME1, "SELECT COUNT(*) FROM EMP");
            File.WriteAllText(FILENAME2, "SELECT COUNT(*) FROM DEPT");

            var args = new[]
            {
                $"Run",
                $"-cs={Configuration.SQLSERVER_CONNECTION_STRING}",
            };
            var runner = new Runner(new Arguments(args)).Start();

            Assert.AreEqual(2, runner.Files.Count());

            // Remove sample files
            File.Delete(FILENAME1);
            File.Delete(FILENAME2);

        }

        [TestMethod]
        public void RunnerRunner_SqlServer_OnlyNextConfig_Test()
        {
            // Using BONUS table to simulate the Configuration Table
            ExecuteSqlQuery("DELETE FROM Bonus; INSERT INTO Bonus (ename, job) VALUES('DbVer', 'script1');");

            // Sample script
            File.WriteAllText(FILENAME1, "SELECT COUNT(*) FROM EMP");
            File.WriteAllText(FILENAME2, "SELECT COUNT(*) FROM DEPT");

            var args = new[]
            {
                $"Run",
                $"-cs={Configuration.SQLSERVER_CONNECTION_STRING}",
                $"-ca={DB_SELECT_CONFIG}",
                $"-cu={DB_UPDATE_CONFIG}",
            };
            var runner = new Runner(new Arguments(args)).Start();

            Assert.AreEqual(1, runner.Files.Count());
            Assert.AreEqual("script2", ExecuteSqlQuery("SELECT Job FROM Bonus WHERE ename = 'DbVer'"));

            // Remove sample files
            File.Delete(FILENAME1);
            File.Delete(FILENAME2);
            ExecuteSqlQuery("DELETE FROM Bonus");
        }


        [TestMethod]
        public void RunnerRunner_SqlServer_NoScriptAvailable_Test()
        {
            // Using BONUS table to simulate the Configuration Table
            ExecuteSqlQuery("DELETE FROM Bonus; INSERT INTO Bonus (ename, job) VALUES('DbVer', 'script2');");

            // Sample script
            File.WriteAllText(FILENAME1, "SELECT COUNT(*) FROM EMP");
            File.WriteAllText(FILENAME2, "SELECT COUNT(*) FROM DEPT");

            var args = new[]
            {
                $"Run",
                $"-cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"-ca=\"{DB_SELECT_CONFIG}\"",
                $"-cu=\"{DB_UPDATE_CONFIG}\"",
            };
            var runner = new Runner(new Arguments(args)).Start();

            Assert.AreEqual(0, runner.Files.Count());
            Assert.AreEqual("script2", ExecuteSqlQuery("SELECT Job FROM Bonus WHERE ename = 'DbVer'"));

            // Remove sample files
            File.Delete(FILENAME1);
            File.Delete(FILENAME2);
            ExecuteSqlQuery("DELETE FROM Bonus");
        }

        [TestMethod]
        public void RunnerRunner_SqlServer_Transaction_Test()
        {
            Runner runner = null;

            // Using BONUS table to simulate the Configuration Table
            ExecuteSqlQuery("DELETE FROM Bonus;");

            // Sample script
            File.WriteAllText(FILENAME1, "INSERT INTO Bonus (ename, job) VALUES('Data1', 'Value1');");
            File.WriteAllText(FILENAME2, "BOUM");

            var args = new[]
            {
                $"Run",
                $"-cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"-ca=\"{DB_SELECT_CONFIG}\"",
                $"-cu=\"{DB_UPDATE_CONFIG}\"",
            };

            try
            {
                runner = new Runner(new Arguments(args)).Start();
            }
            catch (SqlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("BOUM"));
            }

            Assert.AreEqual(0, ExecuteSqlQuery("SELECT COUNT(*) FROM Bonus"));

            // Remove sample files
            File.Delete(FILENAME1);
            File.Delete(FILENAME2);
            ExecuteSqlQuery("DELETE FROM Bonus");
        }

        private object ExecuteSqlQuery(string commandText)
        {
            using (var conn = new SqlConnection(Configuration.SQLSERVER_CONNECTION_STRING))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = commandText;
                    return cmd.ExecuteScalar();
                }
                conn.Close();
            }
        }
    }
}

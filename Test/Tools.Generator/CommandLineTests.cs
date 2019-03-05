using Apps72.Dev.Data.Generator.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tools.Generator.Tests
{
    [TestClass]
    public class CommandLineTests
    {
        [TestMethod]
        public void CmdLine_Separators_Test()
        {
            var cmdLine = new CommandLine(new string[] { "-a=1", "B:2", "--c=3", "/D:4" });

            Assert.IsTrue(cmdLine.ContainsKey("a"));
            Assert.AreEqual("1", cmdLine.GetValue("a"));
            Assert.IsTrue(cmdLine.ContainsKey("b"));
            Assert.AreEqual("2", cmdLine.GetValue("b"));
            Assert.IsTrue(cmdLine.ContainsKey("c"));
            Assert.AreEqual("3", cmdLine.GetValue("c"));
            Assert.IsTrue(cmdLine.ContainsKey("d"));
            Assert.AreEqual("4", cmdLine.GetValue("d"));
        }

        [TestMethod]
        public void CmdLine_Guillemets_Test()
        {
            var cmdLine = new CommandLine(new string[] { "-a=\"Hello World\"", "/B:Hello World" });

            Assert.AreEqual("Hello World", cmdLine.GetValue("a"));
            Assert.AreEqual("Hello World", cmdLine.GetValue("b"));
        }

        [TestMethod]
        public void CmdLine_ContainsKey_Test()
        {
            var cmdLine = new CommandLine(new string[] { "-a=1", "B:2" });

            Assert.IsTrue(cmdLine.ContainsKey("a"));
            Assert.IsTrue(cmdLine.ContainsKey("A"));
            Assert.IsTrue(cmdLine.ContainsKey("a", "b"));
            Assert.IsFalse(cmdLine.ContainsKey("c"));
        }

        [TestMethod]
        public void CmdLine_GetValue_Test()
        {
            var cmdLine = new CommandLine(new string[] { "-a=1", "B:2" });

            Assert.AreEqual("1", cmdLine.GetValue("a"));
            Assert.AreEqual("1", cmdLine.GetValue("A"));
            Assert.AreEqual("1", cmdLine.GetValue("a", "B"));
            Assert.AreEqual("1", cmdLine.GetValue("B", "a"));
            Assert.AreEqual(null, cmdLine.GetValue("c"));
        }
    }
}

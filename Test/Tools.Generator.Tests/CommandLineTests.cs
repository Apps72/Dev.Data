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
    }
}

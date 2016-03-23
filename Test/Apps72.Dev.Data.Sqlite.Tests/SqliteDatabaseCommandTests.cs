using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Apps72.Dev.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using Windows.Storage;
using System.Diagnostics;

namespace Apps72.Dev.Data.Sqlite.Tests
{
    [TestClass]
    public class SqliteDatabaseCommandTests
    {
        #region INITIALIZATION

        private SqliteConnection _connection;

        [TestInitialize]
        public void Initialization()
        {
            _connection = Scott.CreateScottDatabase();
        }

        #endregion

        [TestMethod]
        public void ExecuteRowTyped_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT * FROM EMP WHERE EMPNO = 7369");
                EMP emp = cmd.ExecuteRow<EMP>();

                Assert.AreEqual(emp.EmpNo, 7369);
            }
        }
    }
}

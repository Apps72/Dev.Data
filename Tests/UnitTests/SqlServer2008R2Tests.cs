using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apps72.Dev.Data.Tests
{
    [TestClass]
    public class SqlServer2008R2Tests
    {
        private SqlConnection _connection;

        [TestInitialize]
        public void Initialization()
        {
            _connection = new SqlConnection(SqlDatabaseCommandTests.CONNECTION_STRING);
            _connection.Open();

            // Deployment of Assembly
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.CommandText.AppendLine(" IF NOT EXISTS(SELECT value_in_use from sys.configurations where name = 'clr enabled' AND value_in_use = 1) ");
                cmd.CommandText.AppendLine(" BEGIN ");
                cmd.CommandText.AppendLine("   EXEC sp_configure 'clr enabled', 1 ");
                cmd.CommandText.AppendLine("   RECONFIGURE ");
                cmd.CommandText.AppendLine(" END ");

                cmd.CommandText.AppendLine(" IF NOT EXISTS(SELECT is_trustworthy_on FROM sys.databases WHERE name = db_name() AND is_trustworthy_on = 1) ");
                cmd.CommandText.AppendLine(" BEGIN ");
                cmd.CommandText.AppendLine("   DECLARE @sqlAlter NVARCHAR(512) ");
                cmd.CommandText.AppendLine("   SET @sqlAlter = 'ALTER DATABASE ' + db_name() + ' SET TRUSTWORTHY ON' ");
                cmd.CommandText.AppendLine("   EXEC sp_executesql @sqlAlter ");
                cmd.CommandText.AppendLine(" END ");

                // Drop exiting procedures
                cmd.CommandText.AppendLine(" IF EXISTS(SELECT name FROM sysobjects WHERE name = 'sp_DoubleSqlCommand_Test') ");
                cmd.CommandText.AppendLine("   DROP PROCEDURE sp_DoubleSqlCommand_Test ");

                // Drop and register assemblies 
                cmd.CommandText.AppendLine(" IF EXISTS (SELECT [name] FROM sys.assemblies WHERE [name] = 'Apps72.Dev.Data.TestsSqlServer')  ");
                cmd.CommandText.AppendLine("       DROP ASSEMBLY [Apps72.Dev.Data.TestsSqlServer] "); 
                cmd.CommandText.AppendLine(" IF EXISTS (SELECT [name] FROM sys.assemblies WHERE [name] = 'Apps72.Dev.Data')  ");
                cmd.CommandText.AppendLine("       DROP ASSEMBLY [Apps72.Dev.Data] ");
             
                cmd.CommandText.AppendLine(" CREATE ASSEMBLY [Apps72.Dev.Data]  ");
                cmd.CommandText.AppendLine("   FROM '" + this.GetSqlClrAssembly + "'  ");
                cmd.CommandText.AppendLine("   WITH PERMISSION_SET = SAFE ");
                cmd.CommandText.AppendLine(" CREATE ASSEMBLY [Apps72.Dev.Data.TestsSqlServer]  ");
                cmd.CommandText.AppendLine("   FROM '" + this.GetTestClrAssembly + "'  ");
                cmd.CommandText.AppendLine("   WITH PERMISSION_SET = SAFE ");

                // Add PDB
                cmd.CommandText.AppendLine(" ALTER ASSEMBLY [Apps72.Dev.Data] ADD FILE FROM '" + this.GetSqlClrAssembly.Replace(".dll", ".pdb") + "' ");
                cmd.CommandText.AppendLine(" ALTER ASSEMBLY [Apps72.Dev.Data.TestsSqlServer] ADD FILE FROM '" + this.GetTestClrAssembly.Replace(".dll", ".pdb") + "' ");

                // Create procedures
                cmd.CommandText.AppendLine(" EXEC sp_executesql N'CREATE PROCEDURE sp_DoubleSqlCommand_Test AS EXTERNAL NAME [Apps72.Dev.Data.TestsSqlServer].[StoredProcedures].[sp_DoubleSqlCommand_Test]' ");

                cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void DoubleSqlCommand_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.CommandText.AppendLine(" EXEC dbo.sp_DoubleSqlCommand_Test ");
                cmd.ExecuteNonQuery();
                Assert.IsNull(cmd.Exception);
            }
        }

        /// <summary>
        /// Gets the full path to  Apps72.Dev.Data.dll
        /// </summary>
        /// <returns></returns>
        private string GetSqlClrAssembly
        {
            get
            {
                System.IO.DirectoryInfo testDirectory = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                string rootProjectFolder = testDirectory.Parent.Parent.Parent.FullName;
                return String.Format(@"{0}\TestsSqlServer\bin\Debug\Apps72.Dev.Data.SqlServer.dll", rootProjectFolder);
            }
        }

        /// <summary>
        /// Gets the full path to Apps72.Dev.Data.TestsSqlServer.dll
        /// </summary>
        /// <returns></returns>
        private string GetTestClrAssembly
        {
            get
            {
                System.IO.DirectoryInfo testDirectory = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                string rootProjectFolder = testDirectory.Parent.Parent.Parent.FullName;
                return String.Format(@"{0}\TestsSqlServer\bin\Debug\Apps72.Dev.Data.TestsSqlServer.dll", rootProjectFolder);
            }
        }
    }
}

using Apps72.Dev.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.SqlServerClr.Tests
{
    /// <summary>
    /// List of procedure to define the deployment SQL script
    /// </summary>
    public class SetupDeployment
    {
        public static readonly string CONNECTION_STRING = System.Configuration.ConfigurationManager.ConnectionStrings["Scott"].ConnectionString;

        /// <summary>
        /// Generate and start the Deployment script
        /// </summary>
        public static void Run()
        {
            using (var cmd = new SqlDatabaseCommand(CONNECTION_STRING))
            {
                var procedures = new List<ProcedureDefinition>();

                // HelloWorld
                procedures.Add(new ProcedureDefinition(
                    "HelloWorld", 
                    ProcedureType.Procedure));

                // GetNumberOfEmployees
                procedures.Add(new ProcedureDefinition(
                    "GetNumberOfEmployees", 
                    ProcedureType.FunctionScalar, 
                    string.Empty, 
                    "INT"));

                // GetNumberOfEmployeesInDepartement
                procedures.Add(new ProcedureDefinition(
                    "GetNumberOfEmployeesInDepartement",
                    ProcedureType.FunctionScalar, 
                    "@DeptNo INT", 
                    "INT"));

                cmd.CommandText.Append(SetupDeployment.GetInitializeScript(procedures));
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Returns the Setup Script to deploy the SampleStotedProcedures Assembly
        /// </summary>
        /// <param name="procedures"></param>
        /// <returns></returns>
        public static string GetInitializeScript(IEnumerable<ProcedureDefinition> procedures)
        {
            string assemblyFilename = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SampleStoredProcedures.dll";
            string assemblyName = "SampleStoredProcedures";
            string className = "SampleStoredProcedures";

            StringBuilder sql = new StringBuilder();

            // Activation of the CLR flag in SQL Server
            sql.AppendLine($" IF NOT EXISTS(SELECT value_in_use from sys.configurations where name = 'clr enabled' AND value_in_use = 1) ");
            sql.AppendLine($" BEGIN ");
            sql.AppendLine($"   EXEC sp_configure 'clr enabled', 1 ");
            sql.AppendLine($"   RECONFIGURE ");
            sql.AppendLine($" END; ");

            // Verification of authorizations 
            sql.AppendLine($" IF NOT EXISTS(SELECT is_trustworthy_on FROM sys.databases WHERE name = db_name() AND is_trustworthy_on = 1) ");
            sql.AppendLine($" BEGIN ");
            sql.AppendLine($"   DECLARE @sqlAlter NVARCHAR(512) ");
            sql.AppendLine($"   SET @sqlAlter = 'ALTER DATABASE ' + db_name() + ' SET TRUSTWORTHY ON' ");
            sql.AppendLine($"   EXEC sp_executesql @sqlAlter ");
            sql.AppendLine($" END; ");

            // Deleting of existing procedures
            foreach (var item in procedures)
            {
                sql.AppendLine($" IF EXISTS(SELECT name FROM sysobjects WHERE name = '{item.Name}' AND type = '{item.TypeCode}') ");
                sql.AppendLine($"   DROP {item.TypeName} {item.Name}; ");
            }

            // Deleting of Assembly
            sql.AppendLine($" IF EXISTS (SELECT [name] FROM sys.assemblies WHERE [name] = '{assemblyName}')  ");
            sql.AppendLine($"       DROP ASSEMBLY [{assemblyName}]; ");

            // Recording of Assembly
            sql.AppendLine($" CREATE ASSEMBLY [{assemblyName}]  ");
            sql.AppendLine($"   FROM '{assemblyFilename}'  ");
            sql.AppendLine($"   WITH PERMISSION_SET = SAFE; ");

            // Registering CLR Procedures
            foreach (var item in procedures)
            {
                string returns = !String.IsNullOrEmpty(item.Returns) ? $"RETURNS {item.Returns}" : String.Empty;
                string arguments = String.Empty;

                if (item.Type == ProcedureType.Procedure && !String.IsNullOrEmpty(item.Arguments))
                    arguments = $"({item.Arguments})";
                if (item.Type == ProcedureType.FunctionScalar || item.Type == ProcedureType.FunctionTable)
                    arguments = $"({item.Arguments})";

                sql.AppendLine($" EXEC sp_executesql N' CREATE {item.TypeName} {item.Name} {arguments} {returns} AS EXTERNAL NAME [{assemblyName}].[{className}].[{item.Name}] '; ");
            }

            return sql.ToString();
        }
    }
}

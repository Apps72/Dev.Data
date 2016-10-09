using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.SqlServerClr.Tests
{
    public class SetupDeployment
    {
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
            sql.AppendLine($" END ");

            // Verification of authorizations 
            sql.AppendLine($" IF NOT EXISTS(SELECT is_trustworthy_on FROM sys.databases WHERE name = db_name() AND is_trustworthy_on = 1) ");
            sql.AppendLine($" BEGIN ");
            sql.AppendLine($"   DECLARE @sqlAlter NVARCHAR(512) ");
            sql.AppendLine($"   SET @sqlAlter = 'ALTER DATABASE ' + db_name() + ' SET TRUSTWORTHY ON' ");
            sql.AppendLine($"   EXEC sp_executesql @sqlAlter ");
            sql.AppendLine($" END ");

            // Deleting of existing procedures
            foreach (var item in procedures)
            {
                sql.AppendLine($" IF EXISTS(SELECT name FROM sysobjects WHERE name = '{item.Name}' AND type = '{item.TypeCode}') ");
                sql.AppendLine($"   DROP {item.TypeName} {item.Name} ");
            }

            // Deleting of Assembly
            sql.AppendLine($" IF EXISTS (SELECT [name] FROM sys.assemblies WHERE [name] = '{assemblyName}')  ");
            sql.AppendLine($"       DROP ASSEMBLY [{assemblyName}] ");
            
            // Recording of Assembly
            sql.AppendLine($" CREATE ASSEMBLY [{assemblyName}]  ");
            sql.AppendLine($"   FROM '{assemblyFilename}'  ");
            sql.AppendLine($"   WITH PERMISSION_SET = SAFE ");

            // Registering CLR Procedures
            foreach (var item in procedures)
            {
                string returns = String.IsNullOrEmpty(item.Returns) == false ? $"RETURNS {item.Returns}" : "";
                sql.AppendLine($" EXEC sp_executesql N' CREATE {item.TypeName} {item.Name} {item.Arguments} {returns} AS EXTERNAL NAME [{assemblyName}].[{className}].[{item.Name}] '");
            }

            return sql.ToString();
        }

    }

    public class ProcedureDefinition
    {
        public ProcedureDefinition(string name, ProcedureType type, string arguments, string returns)
        {
            this.Name = name;
            switch (type)
            {
                case ProcedureType.Procedure:
                    this.TypeCode = "PC";
                    this.TypeName = "PROCEDURE";
                    break;
                case ProcedureType.FunctionScalar:
                    this.TypeCode = "FS";
                    this.TypeName = "FUNCTION";
                    break;
                case ProcedureType.FunctionTable:
                    this.TypeCode = "FT";
                    this.TypeName = "FUNCTION";
                    break;
            }
            this.Arguments = arguments;
            this.Returns = returns;
        }

        public string TypeCode { get; private set; }
        public string TypeName { get; private set; }
        public string Name { get; private set; }
        public string Arguments { get; private set; }
        public string Returns { get; private set; }
    }

    public enum ProcedureType
    {
        Procedure,
        FunctionScalar,
        FunctionTable
    }
}

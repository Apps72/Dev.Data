using Apps72.Dev.Data;
using Microsoft.SqlServer.Server;
using System;

public class SampleStoredProcedures
{
    [SqlProcedure]
    public static void HelloWorld()
    {
        using (var cmd = new SqlDatabaseCommand(SqlDatabaseCommand.GetContextConnection()))
        {
            cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
            var count = cmd.ExecuteScalar<int>();
        }
    }

    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static int GetNumberOfEmployees()
    {
        using (var cmd = new SqlDatabaseCommand(SqlDatabaseCommand.GetContextConnection()))
        {
            cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
            return cmd.ExecuteScalar<int>();
        }
    }

    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static int GetNumberOfEmployeesInDepartement(int deptno)
    {
        using (var cmd = new SqlDatabaseCommand(SqlDatabaseCommand.GetContextConnection()))
        {
            cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP WHERE DEPTNO = @DeptNo ");
            cmd.Parameters.AddWithValue("@DeptNo", deptno);
            return cmd.ExecuteScalar<int>();
        }
    }
}


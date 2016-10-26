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
    public static int ExecuteWithAnonymousType()
    {
        using (var cmd = new SqlDatabaseCommand(SqlDatabaseCommand.GetContextConnection()))
        {
            cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP WHERE EMPNO = 7369 ");
            var data = cmd.ExecuteRow<Emp>();
            return data.EmpNo;
        }
    }

    public class Emp
    {
        public int EmpNo { get; set; }
        public string EName { get; set; }
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


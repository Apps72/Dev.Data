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
}


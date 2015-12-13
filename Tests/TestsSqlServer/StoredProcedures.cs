using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using Apps72.Dev.Data;

public class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure()]
    public static void sp_DoubleSqlCommand_Test()
    {
        SqlConnection connection = SqlDatabaseCommand.GetContextConnection();
        using (SqlDatabaseCommand cmd1 = new SqlDatabaseCommand(connection))
        {
            cmd1.CommandText.AppendLine(" SELECT GETDATE() ");
            DateTime myDate1 = cmd1.ExecuteScalar<DateTime>();
            SqlContext.Pipe.Send(myDate1.ToLongTimeString());

            using (SqlDatabaseCommand cmd2 = new SqlDatabaseCommand(connection))
            {
                cmd2.CommandText.AppendLine(" SELECT GETDATE() ");
                DateTime myDate2 = cmd2.ExecuteScalar<DateTime>();
                SqlContext.Pipe.Send(myDate2.ToLongTimeString());
            }
        }


    }
}


using System;

namespace Apps72.Dev.Data.Performances
{
    class Program
    {
        const string CONNECTION_STRING = @"Server=(localdb)\ProjectsV12;Database=Scott;Integrated Security=true;";

        static void Main(string[] args)
        {
            SqlDatabaseCommandPerformances sqlDatabaseCommand = new SqlDatabaseCommandPerformances(CONNECTION_STRING, 500);
            DapperPerformances dapper = new DapperPerformances(CONNECTION_STRING, 500);

            sqlDatabaseCommand.SelectCount();
            dapper.SelectCount();

            Console.ReadKey();
        }
    }
}

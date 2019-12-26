using BenchmarkDotNet.Running;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Performances
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<BasicSamples>();
            //return;

            const int COUNT = 300;
            var sample = new BasicSamples();
            var watcher = System.Diagnostics.Stopwatch.StartNew();

            sample.DbCmd_Samples();


            watcher.Restart();
            for (int i = 0; i < COUNT; i++)
            {
                sample.DbCmd_ExecuteTable_5Cols_14Rows();
            }
            double avg_dbcmd = (double)watcher.ElapsedMilliseconds / COUNT;
            Console.WriteLine($"DbCmd_ExecuteTable_5Cols_14Rows  {avg_dbcmd}");

            watcher.Restart();
            for (int i = 0; i < COUNT; i++)
            {
                sample.Dapper_ExecuteTable_5Cols_14Rows();
            }
            double avg_Dapper = (double)watcher.ElapsedMilliseconds / COUNT;
            Console.WriteLine($"Dapper_ExecuteTable_5Cols_14Rows  {avg_Dapper}");

            Console.WriteLine($"{(avg_dbcmd / avg_Dapper - 1) * 100}%");
        }
    }
}

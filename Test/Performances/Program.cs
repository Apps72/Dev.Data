using BenchmarkDotNet.Running;
using System;
using System.Data.SqlClient;

namespace Performances
{
    public class Program
    {
        static void Main(string[] args)
        {
            var scott = new ScottInMemory();

            //var summary = BenchmarkRunner.Run<BasicSamples>();

            const int COUNT = 10000;
            var sample = new BasicSamples(scott.Connection);
            var watcher = System.Diagnostics.Stopwatch.StartNew();

            watcher.Restart();
            for (int i = 0; i < COUNT; i++)
            {
                sample.DbCmd_ExecuteTable_5Cols_14Rows();
            }
            Console.WriteLine($"DbCmd_ExecuteTable_5Cols_14Rows  {(double)watcher.ElapsedMilliseconds / COUNT}");

            watcher.Restart();
            for (int i = 0; i < COUNT; i++)
            {
                sample.Dapper_ExecuteTable_5Cols_14Rows();
            }
            Console.WriteLine($"Dapper_ExecuteTable_5Cols_14Rows  {(double)watcher.ElapsedMilliseconds / COUNT}");

        }
    }
}

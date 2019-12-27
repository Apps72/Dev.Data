using BenchmarkDotNet.Running;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Performances
{
    public class Program
    {
        const int RUN_MODE = 0;         // 0=Benchmark, 1=Light samples, 2=ManualPerformances

        static void Main(string[] args)
        {
            switch (RUN_MODE)
            {
                // *******************************
                // Run Benchmark
                // *******************************
                case 0:

                    var summary = BenchmarkRunner.Run<BasicSamples>();
                    break;

                // *******************************
                // Run light samples
                // *******************************
                case 1:
                    new BasicSamples().DbCmd_Samples();
                    break;

                // *******************************
                // Check Performances Manually
                // *******************************
                case 2:
                    const int COUNT = 300;
                    var sample = new BasicSamples();
                    var watcher = System.Diagnostics.Stopwatch.StartNew();

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

                    watcher.Restart();
                    for (int i = 0; i < COUNT; i++)
                    {
                        sample.EF_ExecuteTable_5Cols_14Rows();
                    }
                    double avg_efcore = (double)watcher.ElapsedMilliseconds / COUNT;
                    Console.WriteLine($"EF_ExecuteTable_5Cols_14Rows  {avg_efcore}");

                    Console.WriteLine($"{(avg_dbcmd / avg_Dapper - 1) * 100}%");
                    break;
            }
        }
    }
}

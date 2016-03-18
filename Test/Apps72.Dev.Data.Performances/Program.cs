using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Apps72.Dev.Data.Performances
{
    class Program
    {
        const string CONNECTION_STRING = @"Server=(localdb)\ProjectsV12;Database=Scott;Integrated Security=true;";
        const int NUMBER_OF_EXECUTIONS = 500;
        static Dictionary<string, long> _previousTimes = new Dictionary<string, long>();

        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            IPerformance[] commands = new IPerformance[] 
            {
                new SqlDatabaseCommandPerformances() { Connection = connection, NumberOfExecutions = NUMBER_OF_EXECUTIONS },
                new DapperPerformances() { Connection = connection, NumberOfExecutions = NUMBER_OF_EXECUTIONS }
            };

            foreach (IPerformance command in commands)
            {
                command.ExecuteScalar();
                command.ExecuteScalarTyped();
                command.ExecuteRowTyped();
            }

            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Display the execution time result
        /// </summary>
        /// <param name="performance"></param>
        /// <param name="method"></param>
        public static void DisplayResult(IPerformance performance, [System.Runtime.CompilerServices.CallerMemberName]string method = "")
        {
            long elapsed = performance.Watcher.ElapsedMilliseconds;

            if (!_previousTimes.ContainsKey(method))
                _previousTimes.Add(method, elapsed);

            int percentWithPrevious = Convert.ToInt32((double)_previousTimes[method] / (double)elapsed * 100) - 100;

            if (performance is DapperPerformances)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Dapper             - {method}: {elapsed} ms for {NUMBER_OF_EXECUTIONS} executions... {(double)elapsed / (double)NUMBER_OF_EXECUTIONS} ms for one execution... {percentWithPrevious}% faster.");
            }
            else if (performance is SqlDatabaseCommandPerformances)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"SqlDatabaseCommand - {method}: {elapsed} ms for {NUMBER_OF_EXECUTIONS} executions... {(double)elapsed / (double)NUMBER_OF_EXECUTIONS} ms for one execution... {percentWithPrevious}% faster.");
            }
        }        
    }
}

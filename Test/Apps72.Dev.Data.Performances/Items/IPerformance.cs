using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Apps72.Dev.Data.Performances
{
    public interface IPerformance
    {
        SqlConnection Connection { get; set; }
        int NumberOfExecutions { get; set; }
        Stopwatch Watcher { get; }

        void ExecuteScalar();
        void ExecuteScalarTyped();
        void ExecuteRowTyped();
    }
}

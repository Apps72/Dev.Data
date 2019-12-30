using Apps72.Dev.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
namespace Core.Tests
{
    public class BestPractices : IDisposable
    {
        private readonly object _dbOpeningLock = new object();
        private readonly string _sqlConnectionStrings = "[value read from the configuration file]";
        private DbConnection _connection;

        public virtual IDatabaseCommand GetDatabaseCommand()
        {
            lock (_dbOpeningLock)
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_sqlConnectionStrings);
                }

                if (_connection.State == ConnectionState.Broken ||
                    _connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }

                return new DatabaseCommand(_connection)
                {
                    Log = (query) => Console.WriteLine($"SQL: {query}")
                };
            }
        }

        public virtual IDatabaseCommand GetDatabaseCommand(DbTransaction transaction)
        {
            if (transaction == null)
                return this.GetDatabaseCommand();

            lock (_dbOpeningLock)
            {
                return new DatabaseCommand(transaction)
                {
                    Log = (query) => Console.WriteLine($"SQL: {query}")
                };
            }
        }

        private bool _disposed;

        public virtual void Dispose()
        {
            Cleanup(fromGC: false);
        }

        protected virtual void Cleanup(bool fromGC)
        {
            if (_disposed) return;

            try
            {
                if (fromGC)
                {
                    // Dispose managed state (managed objects).
                    if (_connection != null)
                    {
                        if (_connection.State != ConnectionState.Closed)
                            _connection.Close();

                        _connection.Dispose();
                    }
                }
            }
            finally
            {
                _disposed = true;
                if (!fromGC) GC.SuppressFinalize(this);
            }
        }
    }
}

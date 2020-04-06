# Best Practice: DatabaseService.cs

In your project, using IoC, create this DatabaseService and add it using `services.AddScoped<DatabaseService>();` in Startup.cs.
So, you can use this snippet to write you SQL queries:

```CSharp
using (var cmd = _factory.Database.GetDatabaseCommand())
{
	cmd.TagWith("MY_TAG_FOR_UNIT_TESTS");
	cmd.CommandText = $@"SELECT MyColumn
						   FROM MyTable ";
	cmd.AddParameter("@MyParam", 0);

	var data = cmd.ExecuteTable();
}
```

## Retry with default values

```CSharp
/// <summary />
public class DatabaseService : IDisposable
{
    #region DECLARATIONS

    private readonly object _dbOpeningLock = new object();
    private readonly string _sqlConnectionStrings = string.Empty;
    private readonly bool _sqlToTraceQueries = false;
    private readonly bool _sqlCloseConnectionWhenDisposing = true;
    private readonly ILogger<DatabaseService> _logger;
    private DbConnection? _connection;

    #endregion

    #region CONSTRUCTORS

    /// <summary>
    /// Initializes a new instance of DatabaseService,
    /// using configuration parameters (appSettings.json)
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration)
    {
        _connection = null;
        _logger = logger;
        _sqlConnectionStrings = configuration.GetSection("ConnectionStrings:SmartTime")?.Value ?? String.Empty;
        _sqlToTraceQueries = bool.Parse(configuration.GetSection("AppSettings:TraceSqlQueries")?.Value ?? "false");
    }

    /// <summary>
    /// Initializes a new instance of DatabaseService,
    /// using an existing SqlConnection... for UnitTests via DbMocker.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    /// <param name="connection"></param>
    public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration, DbConnection connection)
        : this(logger, configuration)
    {
        _connection = connection;
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Returns the SQL Server Date and Time
    /// </summary>
    /// <returns></returns>
    public virtual DateTime GetServerTime()
    {
        using (var cmd = this.GetDatabaseCommand())
        {
            cmd.TagWith("GETDATE");
            cmd.CommandText = "SELECT GETDATE()";
            DateTime date = cmd.ExecuteScalar<DateTime>();
            return date;
        }
    }

    #endregion

    #region DATABASE COMMAND

    /// <summary>
    /// Returns a Database command connected to the project SQL Server.
    /// </summary>
    /// <returns></returns>
    public virtual IDatabaseCommand GetDatabaseCommand()
    {
        return GetDatabaseCommand(0);
    }

    /// <summary>
    /// Returns a Database command connected to the project SQL Server.
    /// </summary>
    /// <param name="commandTimeout">Maimum timeout of the query</param>
    /// <returns></returns>
    public virtual IDatabaseCommand GetDatabaseCommand(int commandTimeout)
    {
        // Active transaction
        var activeTransaction = GetTransaction(_connection);
        if (activeTransaction != null)
        {
            _logger?.LogDebug($"SQL: Use active DbTransaction [HashCode = {activeTransaction.GetHashCode()}].");
            var cmd = GetDatabaseCommand(activeTransaction);
            if (commandTimeout > 0) cmd.CommandTimeout = commandTimeout;
            return cmd;
        }

        // New connection
        lock (_dbOpeningLock)
        {
            if (_connection == null)
                _connection = new SqlConnection(_sqlConnectionStrings);

            if (_connection.State == ConnectionState.Broken ||
                _connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            var cmd = new DatabaseCommand(_connection)
            {
                CommandTimeout = commandTimeout > 0 ? commandTimeout : 0
            };

            if (_sqlToTraceQueries)
            {
                cmd.Log = DatabaseCommand_Logs;
            }

            return cmd;
        }
    }

    /// <summary>
    /// Returns a Database command connected to the project SQL Server.
    /// </summary>
    /// <param name="transaction">Current transaction</param>
    /// <returns></returns>
    public virtual IDatabaseCommand GetDatabaseCommand(DbTransaction transaction)
    {
        if (transaction == null)
            return this.GetDatabaseCommand();

        lock (_dbOpeningLock)
        {

            var cmd = new DatabaseCommand(transaction);

            if (_sqlToTraceQueries)
            {
                cmd.Log = DatabaseCommand_Logs;
            }

            return cmd;
        }
    }

    /// <summary />
    private void DatabaseCommand_Logs(string message)
    {
        _logger?.LogDebug($"SQL: {message}");
    }

    /// <summary>
    /// Returns the internal DbTransaction associated to the <paramref name="connection"/>.
	/// Only if the connection is a SQL Server Connection object (SqlConnection).
    /// </summary>
    /// <param name="connection">Connection to retrieve internal Transaction.</param>
    /// <returns></returns>
    private DbTransaction? GetTransaction(IDbConnection? connection)
    {
        var info = connection?.GetType().GetProperty("InnerConnection", BindingFlags.NonPublic | BindingFlags.Instance);
        var internalConn = info?.GetValue(connection, null);
        var currentTransactionProperty = internalConn?.GetType().GetProperty("CurrentTransaction", BindingFlags.NonPublic | BindingFlags.Instance);
        var currentTransaction = currentTransactionProperty?.GetValue(internalConn, null);
        var realTransactionProperty = currentTransaction?.GetType().GetProperty("Parent", BindingFlags.NonPublic | BindingFlags.Instance);
        var realTransaction = realTransactionProperty?.GetValue(currentTransaction, null);
        return (DbTransaction?)realTransaction;
    }

    #endregion

    #region IDISPOSABLE SUPPORT

    private bool disposedValue = false; // To detect redundant calls

    /// <summary>
    /// This code added to correctly implement the disposable pattern.
    /// </summary>
    /// <param name="isDisposing"></param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (!disposedValue)
        {
            if (isDisposing)
            {
                if (_sqlCloseConnectionWhenDisposing)
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

            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
            disposedValue = true;
        }
    }

    /// <summary>
    /// This code added to correctly implement the disposable pattern.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    #endregion
}
```

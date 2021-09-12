# Best Practice: DatabaseService.cs

In your project, using IoC, create this DatabaseService and add it using `services.AddScoped<DatabaseService>();` in Startup.cs.
So, you can use this snippet to write you SQL queries:

```CSharp
private readonly Data.DatabaseService _databaseService = new Data.DatabaseService();

using (var cmd = _databaseService.GetDatabaseCommand())
{
    cmd.TagWith("MY_TAG_FOR_UNIT_TESTS");
    cmd.CommandText = @"SELECT Name
						  FROM Employee
                         WHERE Id > @MinId ";
    cmd.AddParameter("@MinId", 0);

    var data = await cmd.ExecuteTableAsync<string>();
}
```

## Sample of Database Service

Find here, a **DatabaseService** class to use in your project.
Use IoC Configuration and Logger classes to replace the `CONNECTION_STRING` and to configure `DatabaseCommand_Logs` method.

```CSharp
using Apps72.Dev.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

/// <summary />
public class DatabaseService : IDisposable
{
    private const string CONNECTION_STRING = @"server=(localdb)\MyServer; Database=Scott;";
    private readonly object _dbOpeningLock = new object();
    private DbConnection? _connection;

    /// <summary />
    public virtual IDatabaseCommand GetDatabaseCommand()
    {
        // New connection
        lock (_dbOpeningLock)
        {
            if (_connection == null)
                _connection = new SqlConnection(CONNECTION_STRING);

            if (_connection.State == ConnectionState.Broken ||
                _connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            var cmd = new DatabaseCommand(_connection);
            cmd.Log = DatabaseCommand_Logs;

            return cmd;
        }
    }

    /// <summary />
    private void DatabaseCommand_Logs(string message)
    {
        System.Diagnostics.Trace.WriteLine($"SQL: {message}");
    }

    private bool disposedValue = false; // To detect redundant calls

    /// <summary />
    protected virtual void Dispose(bool isDisposing)
    {
        if (!disposedValue)
        {
            if (isDisposing)
            {
                // Dispose managed state (managed objects).
                if (_connection != null)
                {
                    if (_connection.State != ConnectionState.Closed)
                        _connection.Close();

                    _connection.Dispose();
                }
            }

            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
            disposedValue = true;
        }
    }

    /// <summary />
    public void Dispose()
    {
        Dispose(true);
    }
}
```

## Using a database transaction

See the [GetTransaction](databaseservice.md) method, if you want to use an active transaction.
If there are no transaction (`trans == null`), use the active conection, if not use this current transaction.

```csharp
var trans = DataExtensions.GetTransaction(_connection);

var cmd = trans == null
        ? new DatabaseCommand(_connection)
        : new DatabaseCommand(trans);
```
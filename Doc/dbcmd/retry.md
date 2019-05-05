# Retry queries

Some projects sometimes trigger DeadLock exceptions that are often very difficult to resolve. 
A simple solution is to wait a few milliseconds and restart the request.
The `Retry` property allows you to configure this.

## Retry with default values

In this case, when a Database Exception occured, 
the process wait 1 second and try to execute the same query again.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.Retry.SetDefaultCriteriaToRetry(RetryDefaultCriteria.SqlServer_DeadLock);

    cmd.CommandText = "SELECT COUNT(*) FROM EMP";
    int count = cmd.ExecuteScalar<int>();
}
```

## Retry with specific values

You can configure your retry values with the `Activate` method.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.Retry.Activate(options =>
    {
        options.SetDefaultCriteriaToRetry(RetryDefaultCriteria.SqlServer_DeadLock);
        options.MillisecondsBetweenTwoRetries = 1000;
        options.NumberOfRetriesBeforeFailed = 3;
    });

    cmd.CommandText = "SELECT COUNT(*) FROM EMP";
    int count = cmd.ExecuteScalar<int>();
}
```

## How to dectect a Deadlock?

For **SQL Server**, we check if the exception message contains "deadlock".

For **Oracle Server**, we check if the exception message contains "ORA-04061" or "ORA-04068".

You can configure your criteria using the `CriteriaToRetry` method.
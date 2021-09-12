## ExecuteNonQuery

This method executes the query and returns the count of modified rows

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "DELETE FROM EMP";
    cmd.ExecuteNonQuery();
}
```

You can start a transaction to perform multiple SQL commands as a unit of work.

```CSharp
using (var transaction = mySqlConnection.BeginTransaction())
{
    using (var cmd = new DatabaseCommand(transaction))
    {
        cmd.CommandText = "INSERT INTO EMP (EMPNO, ENAME) VALUES (1234, 'ABC')";
        cmd.ExecuteNonQuery();
    }

    using (var cmd = new DatabaseCommand(transaction))
    {
        cmd.CommandText = "INSERT INTO EMP (EMPNO, ENAME) VALUES (9876, 'XYZ')");
        cmd.ExecuteNonQuery();
    }

    transaction.Rollback();

    // With the Rollback, nothing was saved in the database.
    // Use transaction.Commit() to persist all changes.
}
```
> All execution commands are available in synchronous and asynchronous (Async) mode.
# Tracing and logging

You can easily trace all SQL queries that will be sent to the database server, 
by defining an action at the `Log` property.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    // To trace all queries in the console.
    cmd.Log = Console.WriteLine;    

    // To trace all queries into the file sql.log
    cmd.Log = (query) =>
    {
        System.IO.File.AppendAllText("sql.log", query);
    };
}
```

The easy way is to define a method `GetCommand()` in your `DataService` object 
to centralize all of these configurations.

> If you need more details about your DatabaseCommand, you can use `ActionBeforeExecution` 
> to trace all commands details before executions.
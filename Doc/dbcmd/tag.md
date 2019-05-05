# Tag

This feature helps correlate SQL queries in code with queries captured in logs or in unit tests.
You annotate a `CommandText` query using the new `TagWith()` method.

This feature is similar to the `TagWith` method available in [EF Core 2.2](https://docs.microsoft.com/ef/core/querying/tags).

```CSharp
using (var cmd = new DatabaseCommand(_connection))
{
    cmd.TagWith("List all employees");
    cmd.CommandText = "SELECT * FROM EMP";
}
```

This query is translated to the following SQL statement:

```SQL
-- List all employees
SELECT * FROM EMP
```

It's possible to call `TagWith()` many times on the same query. Query tags are cumulative.

> **Best practice**: We recommend to tag all your requests, in order 
> to find them more easily in the logs or in the unit tests.
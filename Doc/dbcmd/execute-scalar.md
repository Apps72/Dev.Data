## ExecuteScalar

This method executes the query and returns the first column of the first row of results
Other rows and columns are ignored.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT COUNT(*) FROM EMP";
    var nbEmployees = cmd.ExecuteScalar<int>();
}
```

> If no data are found, the result will be `null`.

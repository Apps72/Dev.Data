# Actions _before_ and _after_ query execution

Before and after the execution of a request, DatabaseCommand checks 
for the presence of pre-processing and possible post-processing.
If an ActionBeforeExecution or AfterBeforeExecution action is defined, 
it is executed before or after the request.
This allows you to easily intervene on requests. For example, to perform a security 
or tracking operation before each SQL query.

In this sample, we change the parameter `EmpNo` by the value `9999` (always).

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT COUNT(*) 
                           FROM EMP
                          WHERE EMPNO = @EmpNo ";
    cmd.AddParameter("@EmpNo", 7392);

    cmd.ActionBeforeExecution = (command) =>
    {
        command.Parameters["@EmpNo"].Value = 9999;
    };

    var count = cmd.ExecuteScalar();
}
```

You can also, use this method to trace your request (in addition to [Tracing and logs](log.md)).
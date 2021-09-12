## ExecuteDataSet

Execute the query and return a standard [System.Data.DataSet](https://docs.microsoft.com/en-us/dotnet/api/system.data.dataset) object filled with data table results.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT EMPNO, ENAME FROM EMP;
                         SELECT * FROM DEPT; ");
    var dataset = cmd.ExecuteDataSetAsync();
    
    var smith = dataset.Tables[0].Rows[0];
    var accounting = dataset.Tables[1].Rows[0];
}
```

## ExecuteDataSet<T, U>

This method executes the query and returns a list or array of new instances of typed results filled with data table results.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT EMPNO, ENAME FROM EMP;
                         SELECT * FROM DEPT; ");
    var data = cmd.ExecuteDataSet<EMP, DEPT>();

    var employees = data.Item1;
    var departments = data.Item2;
}
```

You can call this method with 2, 3, 4 or 5 result types (not more than 5).


> All execution commands are available in synchronous and asynchronous (Async) mode.
# Fluent queries

Execute methods ([ExecuteNonQuery](basic-samples.md), [ExecuteScalar](basic-samples.md#ExecuteScalar), 
[ExecuteRow](basic-samples.md#ExecuteRow), [ExecuteTable](basic-samples.md#ExecuteTable)) are available using the [Fluent](https://en.wikipedia.org/wiki/Fluent_interface) syntax. 
To do this, call the `Query()` method.

Sample 1
```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    var count = cmd.Query(@"SELECT COUNT(*) 
                              FROM EMP 
                             WHERE EMPNO > @id")
                   .AddParameter("id", 10)
                   .ExecuteScalar<int>();
}
```

Sample 2
```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    var emps  =  cmd.Query(@"SELECT ENAME, JOB
                              FROM EMP 
                             WHERE EMPNO > @id")
                    .AddParameter(new { id = 10 })
                    .ExecuteTable(new 
                        { 
                            EName = default(string), 
                            Job = default(string) 
                        });
}
```

> All features are not available in Fluent queries. For example, `ExecuteDataset` is not yet implemented.
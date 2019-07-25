## ExecuteTable

This method executes the SQL query to map all data rows to a `IEnumerable<T>`.

### Basic sample

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.Append("SELECT * FROM EMP");
    var emps = cmd.ExecuteTable<Employee>();
    // emps is a IEnumerable<Employee>.
}
```

### ExecuteTable with a converter

Execute the query and return an array of new instances of typed results 
filled with data table result, converted with a function.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.Append("SELECT * FROM EMP");
    var emps = cmd.ExecuteTable<Employee>((row) => 
    {
        return new
        {
            Id = row.Field<int>("EMPNO"),
            Name = row.Field<string>("ENAME"),
            HireDate = row.Field<DateTime>("HIREDATE").Year,
        };
    });
    // emps is a IEnumerable of a new object { Id, Name, HireYear }
}
```

## ExecuteTable

This method executes the SQL query to map all data rows to a `IEnumerable<T>`.
Only columns that find a property with the same name are mapped. 
The others fields (from the table or class) are ignored.

### ExecuteTable with existing entity

Execute the query and return an array of new instances of typed results 
filled with data table result.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT * FROM EMP";
    var emps = cmd.ExecuteTable<Employee>();
    // emps is a IEnumerable<Employee>.
}
```

> Use the `[Column(name)]` attribute to specify different a column name that the property name. 

### ExecuteTable with anonymous class

Execute the query and return an array of new instances of anonymous class 
filled with data table result.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT * FROM EMP";
    var emps = cmd.ExecuteTable(new
        {
            EmpNo = default(int),
            EName = default(string),
        });
    // emps is a IEnumerable of { EmpNo, EName }.
}
```

### ExecuteTable with a converter function

Execute the query and return an array of new instances of typed results 
filled with data table result, converted by a function.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT * FROM EMP";
    var emps = cmd.ExecuteTable<Employee>((row) => 
    {
        return new
        {
            Id = row.Field<int>("EMPNO"),
            Name = row.Field<string>("ENAME"),
            HireYear = row.Field<DateTime>("HIREDATE").Year,
        };
    });
    // emps is a IEnumerable of a new object { Id, Name, HireYear }
}
```

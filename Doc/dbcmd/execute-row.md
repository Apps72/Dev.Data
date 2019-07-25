## ExecuteRow

This method executes the query and fill the specified object with the first row of results.
Other rows are ignored.
Only columns that find a property with the same name are mapped. 
The others fields (from the table or class) are ignored.

### ExecuteRow with existing entity

This method executes the query and fill the specified object with the first row of results.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT * FROM EMP WHERE EMPNO = 7369";
    var smith = cmd.ExecuteRow<Employee>();
    // smith is a Employee type.
}
```

> Use the `[Column(name)]` attribute to specify different a column name that the property name. 

### ExecuteRow with anonymous class

This method executes the query and fill a new instances of anonymous class, with the first row of results.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT * FROM EMP WHERE EMPNO = 7369";
    var smith = cmd.ExecuteRow(new
        {
            EmpNo = default(int),
            EName = default(string),
        });
    // smith is a anonymous object { EmpNo, EName }.
}
```

### ExecuteRow with a converter function

This method executes the query and fill the specified object with the first row of results, 
converted by a function.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT * FROM EMP WHERE EMPNO = 7369";
    var emps = cmd.ExecuteRow<Employee>((row) => 
    {
        return new
        {
            Id = row.Field<int>("EMPNO"),
            Name = row.Field<string>("ENAME"),
            HireYear = row.Field<DateTime>("HIREDATE").Year,
        };
    });
    // smith is a anonymous object { Id, Name, HireYear }
}
```

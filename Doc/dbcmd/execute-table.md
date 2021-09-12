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

### MapTo for partial mapping

Some C# objects contains extra complex properties. 
For example, MyEmployee contains a property Department of type MyDepartment.

```CSharp
class MyEmployee
{
    public int EmpNo { get; set; }
    public string EName { get; set; }
    public MyDepartment Department { get; set; }
}
class MyDepartment
{
    public string DName { get; set; }
}
```

You can use the `MapTo<T>` method to automatically map all MyEmployee properties and, next, all MyDepartment properties to a final object.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @"SELECT EMP.EMPNO,
                               EMP.ENAME,                                         
                               DEPT.DNAME
                          FROM EMP 
                         INNER JOIN DEPT ON DEPT.DEPTNO = EMP.DEPTNO
                         WHERE EMPNO = 7369";

    var emps = cmd.ExecuteTable(row => 
    {
        MyEmployee emp = row.MapTo<MyEmployee>();
        emp.Department = row.MapTo<MyDepartment>();
        return emp;
    });}
```

> All execution commands are available in synchronous and asynchronous (Async) mode.
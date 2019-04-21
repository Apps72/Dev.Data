## Basic samples

All these examples use an **EMP** employees table:

|EMPNO |ENAME  |JOB      |MGR  |
|---   |---    |---      |---  |
|7369  |SMITH  |CLERK    |7566 |
|7499  |ALLEN  |SALESMAN |7566 |
|7521  |WARD   |SALESMAN |7566 |
|7566  |JONES  |MANAGER  |NULL |

And this table is mapped to an **Employee** C# class:

```CSharp
class Employee
{
    public int    EmpNo { get; set; }
    public string EName { get; set; }
    public string Job   { get; set; }
    public string Mgr   { get; set; }
}
```

### 1. ExecuteTable - Get **all data**.

Call the [ExecuteTable](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteTable__1) method to map all data rows to a `IEnumerable<T>`.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.Append("SELECT * FROM EMP");
    var emps = cmd.ExecuteTable<Employee>();
    // emps is a IEnumerable<Employee>.
}
```

### 2. ExecuteRow - Get the **first row**.

Call the [ExecuteRow](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteRow__1) method to map a row to an object.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.Append("SELECT * FROM EMP WHERE EMPNO = 7369");
    var smith = cmd.ExecuteRow<Employee>();
    // smith is a Employee object.
}
```

### 3. AddParameter - Get a row using a **SQL Parameter**.

Call the [AddParameter](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_AddParameter_System_String_System_Object_) method to define a SQL parameter.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.Append("SELECT * FROM EMP WHERE EMPNO = @ID");
    cmd.AddParameter("@ID", 7369);
    var smith = cmd.ExecuteRow<Employee>();
    // smith is a Employee object.
}
```

### 4. Dynamic - Get the first row **without creating the class**.

Call the [ExecuteRow](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteRow__1) method, using the `dynamic` keyword, to map the result dynamically (properties are created dynamically, based on name/type of SQL results).

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.Append("SELECT * FROM EMP WHERE EMPNO = 7369");
    var smith = cmd.ExecuteRow<dynamic>();
    // smith is a object with properties EMPNO, ENAME, JOB, MGR.
}
```

### 5. ExecuteScalar - Get a **single value**.

Call the [ExecuteScalar](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteScalar) method to map a value (first column, first row) to an simple type.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.Append("SELECT COUNT(*) FROM EMP");
    int count = cmd.ExecuteScalar<int>();
    // count = 4.
}
```

### 6. Using a **Fluent** syntax.

All methods are available using the [Fluent](https://en.wikipedia.org/wiki/Fluent_interface) syntax. To do this, call the `Query()` method.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    var smith = cmd.Query(@"SELECT * 
                              FROM EMP 
                             WHERE EMPNO = @ID")
                   .AddParameter("ID", 7369)
                   .ExecuteRow<Employee>();
    // smith is a Employee object.
}
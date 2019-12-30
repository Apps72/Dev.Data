## Parameters

To avoid [SQL injections](https://en.wikipedia.org/wiki/SQL_injection), it is recommended to use parameters in queries.
In SQL Server or SQLite, a parameter is set via the `@` symbol; and in Oracle Server, a parameter is set via the `:` symbol.

Parameters automatically handle data types: a `@MyText` parameter of type string will be replaced by its value, surrounded by the apostrophes necessary for the SQL language `'Value of variable'`.
The same applies to dates, Booleans and other numerical values.

### AddParameter with name and value

You can add a parameter using `AddParameter(string name, object value)`.
The _dbtype_ is deducted from the `value` provided.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT * 
                           FROM EMP 
                          WHERE EMPNO = @EmpNo 
                            AND ENAME LIKE @Ename ";

    cmd.AddParameter("@EmpNo", 7369);
    cmd.AddParameter("@Ename", "%SM%");
}
```

### AddParameter with name, value and DbType

You can add a parameter using `AddParameter(string name, object value, DbType type)`.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT * 
                           FROM EMP 
                          WHERE COMM = @Comm";

    cmd.AddParameter("@Comm", null, DbType.Currency);
}
```

### AddParameter with name, value, DbType and size

You can add a parameter using `AddParameter(string name, object value, DbType type, int? size)`.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT * 
                           FROM EMP 
                          WHERE ENAME = @Name";

    cmd.AddParameter("@Name", "Smith", DbType.String, 20);
}
```

### AddParameter with a typed object

You can add multiple parameters using a typed object.
All properties are used to define a parameter.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT * 
                           FROM EMP 
                          WHERE EMPNO = @EmpNo 
                            AND HIREDATE = @HireDate";

    cmd.AddParameter(new 
    {
        EmpNo = 7369,
        HireDate = new DateTime(1980, 12, 17),
    });
}
```

### Parameters collection

All parameters added are listed in the property `Parameters`.
You can add, remove or change parameters using this property.
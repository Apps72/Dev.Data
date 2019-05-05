## CommandText

The `CommandText` property is of the [SqlString](../api/dbcmd/Apps72.Dev.Data.SqlString.yml) type.
`SQLString` is convertible with `string` and `StringBuilder` types.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText= @"SELECT COUNT(*)
                         FROM EMP
                        WHERE EMPNO = 7369";
    var count = cmd.ExecuteScalar<int>();
}
```

## Append methods

Like a `StringBuilder` you can dynamcally create your SQL command text using these methods:

- **Append**: Appends the specified string to end of the current command text.
- **AppendLine**: Appends the specified string followed by the default line terminator to the end of the current command text.
- **AppendFormat**: Appends the string returned by processing a composite format string, 
  which contains zero or more format items, to this instance. Each format item is replaced by
  the string representation of a corresponding argument in a parameter array.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText.AppendLine("SELECT COUNT(*)");
    cmd.CommandText.AppendLine("  FROM EMP");
    cmd.CommandText.AppendLine(" WHERE EMPNO = 7369");
    var count = cmd.ExecuteScalar<int>();
}
```

## FormattedText

When debugging or tracing SQL queries, it is easier to view the final query directly, 
where the parameters are replaced by their values. 
This is what the `Formatted` property provides.

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = @" SELECT * 
                           FROM EMP 
                          WHERE EMPNO = @EmpNo 
                            AND ENAME LIKE @Ename ";

    cmd.AddParameter("@EmpNo", 7369);
    cmd.AddParameter("@Ename", "%SM%");

    string commandAsText      = cmd.Formatted.CommandAsText;
    string commandAsHtml      = cmd.Formatted.CommandAsHtml;
    string commandAsVariables = cmd.Formatted.CommandAsVariables;
```

### Formatted.CommandAsText

This property returns the SQL query where all parameters are replaced by their values.

```Text
SELECT * 
  FROM EMP 
 WHERE EMPNO = 7369 
   AND ENAME LIKE '%SM%'
```

### Formatted.CommandAsHtml

Similar to CommandAsText but the result is colored in SQL format: SQL keywords are syntactically recognized.
This result is sometimes interesting to trace requests in HTML files.

```SQL
SELECT * 
  FROM EMP 
 WHERE EMPNO = 7369 
   AND ENAME LIKE '%SM%'
```

### Formatted.CommandAsVariables

This property returns the SQL query where all parameters are declared at the beginning of the request.
This command can be executed directly in a query analyzer. 
This is the mode closest to what will be executed by the database server.

```Text
DECLARE @Ename AS VARCHAR(4) = '%SM%'
DECLARE @EmpNo AS INT = 7369

SELECT * 
  FROM EMP 
 WHERE EMPNO = 7369 
   AND ENAME LIKE '%SM%'
```
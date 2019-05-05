# Check the SQL Server query syntax

## Has a valid command text

Call the method `Mocks.HasValidSqlServerCommandText()` 
to check if your string **CommandText** respect the SQL Server syntax.
**Without connection to SQL Server** 
(but using the (Microsoft.SqlServer.SqlParser)[https://www.nuget.org/packages/Microsoft.SqlServer.SqlParser] package).

```CSharp
conn.Mocks
    .HasValidSqlServerCommandText()
    .WhenTag("MyTag")
    .ReturnsScalar(14);
```

So the `CommandText="SELECT ** FROM EMP"` (double `*`) 
will raised a **MockException** with the message 
_Incorrect syntax near '*'_.

## Default validation

You can also define a default value using the 
`MockDbConnection.HasValidSqlServerCommandText` property.

```CSharp
var conn = new MockDbConnection()
{
    HasValidSqlServerCommandText = true
};
```

> If your database engine is SQL Server, we recommand to use 
> this flag, to validate all your queries.

## Specific activation or desactivation

Even if you have disable syntax checking for all queries, 
you can enable it for a single query.

```CSharp
conn.Mocks
    .When(cmd => cmd.CommandText.Contains("FROM EMP") &&
                 cmd.HasValidSqlServerCommandText() )
    .ReturnsScalar(14);
```
# Conditions

## When

Use the `When` method to describe the condition to be detected.
This condition is based on a Lambda expression containing 
a CommandText or Parameters check.

```CSharp
conn.Mocks
    .When(cmd => cmd.CommandText.Contains("SELECT COUNT") &&
                 cmd.Parameters.Count() == 0)
    .ReturnsTable(...);
```

Based on this condition, when a SQL query will be detected, 
the `Returns` values will be sent.

## WhenTag

Use the `WhenTag` method to detect query containing a row starting 
with a comment `-- MyTag`. This is compatible with [EFCore 2.2](https://docs.microsoft.com/ef/core/querying/tags), 
containing a new extension method `TagWith` to identity a request.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(...);
```

> **Best practice**: use a _Tag_ to easily find your SQL queries. 
> Each request in your application must have the equivalent _Tag_ 
> (via a SQL comment or by using the `TagWith` method)

See the [TagWith](../dbcmd/tag.md) method included in Database Command toolkit.

## WhenAny

Use the `WhenAny` method to detect all SQL queries. 
In this case, all queries to the database will return the data specified 
by WhenAny.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(...);
```

## Checking order

DbMocker uses the condition encoding order to return the correct table.

We recommend creating a new instance of MockDbConnection for each unit test.
Indeed, the list of Mocks Conditions is global at the SQL connection.
# ReturnsScalar

## Value

When a condition occured, a single simple value will be return.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsScalar<int>(14);
```

## Expression

Using an expression to customize the return.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsScalar<int>(cmd => DateTime.Today.Year > 2000 ? 14 : 0);
```

> This method is mainly used to return a value from the 
> `ExecuteScalar` or `ExecuteNonQuery` methods of the [DbCommand class](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand).
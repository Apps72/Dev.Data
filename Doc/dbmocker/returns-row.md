# ReturnsRow

## Data

When a condition occured, a single data row will be return. 
The specified typed object will generate a MockTable 
where property names will be the column names 
and property values will be the first row data.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsRow(new 
        { 
            Id = 1, 
            Name = "Denis" 
        });
```

## Expression

Using an expression to customize the return.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsRow(cmd => 
    {
        int i = cmd.Parameters.Count();
        return new 
        { 
            Id = i,
            Name = "Denis"
        }
    });
```
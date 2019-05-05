# ReturnsTable

When a condition [condition](conditions.md) is verified, 
a mocked table will be return. There are many ways to define a **MockTable**.

## New instance

Creating an new instance of `MockTable`.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(new MockTable().AddColumns("ID", "Name")
                                 .AddRow(1, "Scott")
                                 .AddRow(2, "Bill"));
```

## Empty table

Using a `MockTable.Empty()` table... to complete.
It is particularly interesting to build the table data dynamically, 
according to external criteria.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(MockTable.Empty()
                           .AddColumns("ID", "Name")
                           .AddRow(1, "Scott")
                           .AddRow(2, "Bill"));
```

## Static methods

Using a `MockTable.WithColumns()` table... to complete.
It is a simplification of the writing of the previous methods.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(MockTable.WithColumns("ID", "Name")
                           .AddRow(1, "Scott")
                           .AddRow(2, "Bill"));
```

## Typed columns

Using a `MockTable.WithColumns()` typed columns. 
In this case, columns are defined using a tuple (ColumnName, ColumnType).

> Often it is essential to specify the types of data to be returned, 
> so that SQL to C# conversion can be done correctly.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(MockTable.WithColumns(("ID", typeof(int?)),
                                        ("Name", typeof(string)))
                           .AddRow(null, "Scott")
                           .AddRow(2, "Bill"));
```

## SingleCell

You can return a table containing only one column and one row,
using method `MockTable.SingleCell`.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(MockTable.SingleCell("Count", 14));
```

It's possible to customize the value returned, 
using a lambda expression.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(cmd => cmd.Parameters.Count() > 0 ? 14 : 99);
```

## FromCsv

You can create a MockTable, using a CSV string with all data.
Go to the [next page for more information](returns-table-csv.md).
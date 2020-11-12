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

## LoadTagsFromResources

To avoid creating dozens of `.WhenTag().ReturnsTable()`, you can use the method `LoadTagsFromResources`.
This method search all text files embedded in your projects (whatever folder it is in)
and use the name as the Tag Name.
The embedded file is a fixed formatted file and must respect this format:

  - First row contains **column names**, and define the position of other rows.
  - Second row contains **data types** (C# types) associated to these columns.
  - Other rows contain data. Empty rows or row started with a hashtag (#) are omited. 

Example of file **SampleTable1**: 

```
Id      Name            Birthdate
(int)   (string)        (DateTime?)
123     Denis           2020-01-12
456     Anne            NULL
```

> To embed a file in Visual Studio, add a new text file in a folder, display the file Properties (F4)
> and the **Build Action to Embedded resource**.

```CSharp
// Search all .txt embedded files with these names: 
// SampleTable1.txt and SampleTable2.txt
conn.Mocks.LoadTagsFromResources("SampleTable1", "SampleTable2");
```

After this method, the Mocks contains 2 conditions with 2 tags (SampleTable1 and SampleTable2) 
and 2 associated MockTable with these typed data.

You can define multiple resource file for the same Tab Name. Use the `MockResourceOptions.TagSeparator` (by default '-') character 
to separate a file identifier from the TagName. Ex. "01-MyTag.txt" and "02-MyTag.txt" will be linked to the same tag (MyTag).

## Checking order

DbMocker uses the condition encoding order to return the correct table.

We recommend creating a new instance of MockDbConnection for each unit test.
Indeed, the list of Mocks Conditions is global at the SQL connection.
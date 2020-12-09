# ReturnsTable from a Fixed formatted content

You can create a MockTable, using a fixed formatted string. This string must respect this format:

  - First row contains **column names**, and define the position of other rows.
  - Second row contains **data types** (C# types) associated to these columns (see below).
  - Other rows contain data. 

> Empty rows or row started with a hashtag (#) are omited.

Example of `data` string: 

```
Id      Name            Birthdate      Male
(int)   (string)        (DateTime?)    (bool)

123     Denis           2020-01-12     true
456     Anne            NULL           false
```

You can generate a MockTable using this method.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(MockTable.FromFixed(data));
```

The second row contains **data types** associated to these columns. These types are mapped like that:

|Fixed format type|Converted to C# type|Example|
|---|---|---|
|(datetime)<br/>(smalldatetime)<br/>(date)    | **DateTime**    | `2020-01-15`                  |
|(time)<br/>(timespan)                        | **TimeSpan**    | `01:02:03` ; `01:02:03.123` |
|(decimal)                                    | **decimal**     | `123.45`                      |
|(double)                                     | **double**      | `123.45`                      |
|(single)<br/>(float)                         | **float**       | `123.45`                      |
|(string)<br/>(varchar)<br/>(nvarchar)        | **string**      | `abc` ; `"ab cd"`             |
|(char)<br/>(nchar)                           | **char**        | `a`                           |
|(bool)<br/>(boolean)                         | **bool**        | `true` ; `false`              |
|(byte)                                       | **byte**        |                               |
|(sbyte)                                      | **sbyte**       |                               |
|(int16)<br/>(short)                          | **short**       | `123`                         |
|(uint16)<br/>(ushort)                        | **ushort**      | `123`                         |
|(int32)<br/>(int)                            | **int**         | `123`                         |
|(uint32)<br/>(uint)                          | **uint**        | `123`                         |
|(int64)<br/>(long)                           | **long**        | `123`                         |
|(uint64)<br/>(ulong)                         | **ulong**       | `123`                         |

# ReturnsTable from a CSV

You can create a MockTable, using a CSV string with all data.
The first row contains the column names. 
The first data row defines types for each columns (like in a Excel importation).

```CSharp
// Columns are aligned, using a tab char (\t) and not spaces.
string csv = @" Id  Name     Birthdate
                1   Scott    1980-02-03
                2   Bill     1972-01-12
                3   Anders   1965-03-14 ";

conn.Mocks
    .WhenTag("MyTag")
    .ReturnsTable(MockTable.FromCsv(csv));
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

> You can define **multiple resource files** for the same Tab Name. Use the `MockResourceOptions.TagSeparator` (by default '-') character 
> to separate a file identifier from the TagName. Ex. "01-MyTag.txt" and "02-MyTag.txt" will be linked to the same tag (MyTag).
> Ex. `conn.Mocks.LoadTagsFromResources("001-MyTag")` will load the file "001-MyTag.txt" and will associated to the tag "MyTag".
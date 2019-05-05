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

> **Remarks**: In a future release, this method will be improved 
> to manage alignments via spaces, to manage data types,...
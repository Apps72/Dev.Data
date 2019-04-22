# Getting Started with DatabaseCommand

**DatabaseCommand** is a set of components helping .NET developers to execute SQL Queries and to retrieve data.

This C# library simplify all SQL Queries to external databases, using the base system class DbConnection. 
Your can use your favorites libraries for SQLServer, Oracle, SQLite, ...

**DatabaseCommand** is an Open Source project. Go to https://github.com/Apps72/Dev.Data to fork or improve the source code.

## Quick start

1. Add the **NuGet package** [Apps72.Dev.Data](https://www.nuget.org/packages/Apps72.Dev.Data).

2. Create a `SqlConnection`, or other **database connection**, in your project, and call the `Open()` method.

```CSharp
var mySqlConnection = new SqlConnection("server=.;Database=Scott;");
mySqlConnection.Open();
```
   
3. Use the **DatabaseCommand** methods to retrieve data (see other samples below).

```CSharp
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    var count = cmd.Query("SELECT COUNT(*) FROM EMP")
                   .ExecuteScalar<int>();
}
```

*Requirements: Microsoft .NET Core 2.0 or Microsoft Framework 4.0 (Client Profile).*

## Execute methods

To retrieve data from a database server, you must write a correct SQL command, 
possibly inject parameters, and execute this query using one of the following methods:

- [**ExecuteNonQuery**](basic-samples.md): Execute the query and return the count of modified rows (for `INSERT`, `UPDATE`, `DELETE`).
- [**ExecuteScalar**](basic-samples.md#ExecuteScalar): Execute the query and return the first column of the first row of results (for `SELECT COUNT() FROM EMP`).
- [**ExecuteRow**](basic-samples.md#ExecuteRow): Execute the query and return a new instance of typed results filled with the first row of results
- [**ExecuteTable**](basic-samples.md#ExecuteTable): Execute the query and return an array of new instances of typed results filled with data table result.
- [**ExecuteDataSet**](basic-samples.md): Execute the query and return a list or array of new instances of typed results filled with data table results (multiple tables).

## Property mapping

By default, each data table columns are mapped to C# class properties with same names: 
`SELECT ENAME FROM EMP` will be mapped to the same C# property name `public string Ename { get; set; }`.
The mapping is case insensitive.

You can use the `[Column()]` attribute to define the database column name to use with another C# property.

```CSharp
[Column("Ename")]
public string EmployeeName { get; set;}
```

## Live samples

Watch this video to see a complete sample to retrieve data from SQL Server.

[![Samples](http://img.youtube.com/vi/DRfM15Paw8k/0.jpg)](http://www.youtube.com/watch?v=DRfM15Paw8k)
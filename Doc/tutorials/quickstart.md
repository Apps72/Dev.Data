# Getting Started with DatabaseCommand

DatabaseCommand is a set of components helping .NET developers to execute SQL Queries and to retrieve data.

This C# library simplify all SQL Queries to external databases, using the base system class DbConnection. 
Your can use your favorites libraries for SQLServer, Oracle, SQLite, ...

## Quick start

1. Add the [NuGet package Apps72.Dev.Data](https://www.nuget.org/packages/Apps72.Dev.Data).

2. Create a **SqlConnection** (or other database connection) in your project, and call the `Open()` method.

```CSharp
var mySqlConnection = new SqlConnection("server=.;Database=Scott;");
mySqlConnection.Open();
```
   
3. Use the DatabaseCommand methods to retrieve data (see other samples below).

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

- [**ExecuteNonQuery**](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteNonQuery): No data, but only the number of data affected (ex. INSERT, UPDATE, DELETE).
- [**ExecuteScalar**](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteScalar): One value is retrieved: the first row, first colum (ex. SELECT COUNT() FROM).
- [**ExecuteRow**](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteRow__1): One data row is retrieved and casted to the defined type.
- [**ExecuteTable**](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteTable__1): An array of rows are retrieved and casted to the defined type.
- [**ExecuteDataSet**](../api/Apps72.Dev.Data.DatabaseCommand.yml#Apps72_Dev_Data_DatabaseCommand_ExecuteDataSet__2): A list of differents rows are retrieved and casted to defined types.
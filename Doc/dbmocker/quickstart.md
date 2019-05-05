# Getting Started with DbMocker

This .NET library simplifies data mocking for UnitTests, to avoid a connection to a relational database.
DbMocker use the standard Microsoft .NET DbConnection object. So, you can mock any toolkit, 
including EntityFramework, Dapper or ADO.NET; And for all database servers (SQL Server, Oracle, SQLite).

**DbMocker** is an Open Source project. Go to https://github.com/Apps72/DbMocker to fork or improve the source code.

## Quick start

1. Add the **NuGet package** [DbMocker ](https://www.nuget.org/packages/DbMocker).

2. Instanciate a `MockDbConnection`. 
   This object implements all features of `DbConnection` 
   with only one extra property called `Mocks` to define your conditions (see step 3).

    ```CSharp
    [TestMethod]
    public void MyUnitTest()
    {
        var conn = new MockDbConnection();
    }
    ```

3. Intercept you SQL queries executions, using a condition and return a DataTable.
   For example, when the SQL query containing `SELECT COUNT` is executed in your app, 
   it will be intercepted by **DbMocker** which will return a table 
   containing the value 14.

    ```CSharp
    conn.Mocks
        .When(cmd => cmd.CommandText.Contains("SELECT COUNT"))
        .ReturnsTable(MockTable.WithColumns("Count")
                               .AddRow(14));
    ```

4. Don't change your app source code. 
   Call your methods using this `MockDbConnection` reference, 
   and validate your results.

Go to the next page to see a [complete basic sample](basic-sample.md).
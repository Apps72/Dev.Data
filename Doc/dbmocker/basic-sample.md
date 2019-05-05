# A complete basic sample

Create a console applicaion and instanciate a `SqlConnection` 
to your SQL Server database.

All these examples use an **EMP** [employees table](../dbcmd/db-scott.md):

|EMPNO |ENAME  |JOB      |MGR  |
|---   |---    |---      |---  |
|7369  |SMITH  |CLERK    |7566 |
|7499  |ALLEN  |SALESMAN |7566 |
|7521  |WARD   |SALESMAN |7566 |
|7566  |JONES  |MANAGER  |NULL |

```CSharp
public class MyApplication
{
    public static DbConnection MyConnection;

    public static void Main(string[] args)
    {
        MyConnection = new SqlConnection("Server=.;Database=Scott;");
        MyConnection.Open();

        var count = GetNumberOfEmployees();

        MyConnection.Close();
    }

    // Returns the number of employees
    public static int GetNumberOfEmployees()
    {
        using (var cmd = MyConnection.CreateCommand())
        {
            cmd.CommandText = "SELECT COUNT(*) FROM Employees";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
}
```

Create a Unit Tests project, referencing your application project.
And add the **NuGet package** [DbMocker ](https://www.nuget.org/packages/DbMocker).

```CSharp
[TestClass]
public class MyTests
{
    [TestMethod]
    public void UnitTest1()
    {
        var conn = new MockDbConnection();

        // Use the DBMocker connection instead of your connection
        MyApplication.MyConnection = conn;

        // When a specific SQL command is detected,
        // Don't execute the query to your database engine (SQL Server, Oracle, SQLite, ...),
        // But returns this Table.
        conn.Mocks
            .When(cmd => cmd.CommandText.StartsWith("SELECT COUNT"))
            .ReturnsTable(MockTable.WithColumns("Count")
                                   .AddRow(14));

        // Call your "classic" methods to tests
        int count = MyApplication.GetNumberOfEmployees(conn);

        Assert.AreEqual(14, count);
    }
}
```

> **Tips**: When you integrate DBMocker into an existing project, 
> the easiest way is to replace your **SqlConnection** with **MockDbConnection**.
> And to execute the unit tests in _Debug_ mode. 
> You will receive an error for each SQL query that will have to be mocked.
> Step by step, this allows you to create mocks and simulate your data returns.
# DatabaseCommand - Simple Object Mapping

---
# **Go to the new [documentation web site](http://apps72.com) to learn how to use DatabaseCommand.**
---

## Introduction

This C# library simplify SQL Queries to external databases, using the standard class **DbConnection** and [**DotNetCore**](https://dotnet.github.io).
Many implementations are compiled for **SQL Server**, **Oracle Server** or **SQLite**.

First, create a SqlConnection or an other DbConnection. 


```csharp   
using (var cmd = new DatabaseCommand(mySqlConnection))
{
    cmd.CommandText = "SELECT ID, Name FROM EMployee";
    var all = cmd.ExecuteTable<Employee>();     // List of all employees
    var smith = cmd.ExecuteRow<Employee>();     // First employee
    var id = cmd.ExecuteScalar<int>();          // ID of first employee
    
    var emps = cmd.Query(" SELECT * FROM Employee WHERE ID > @ID ")
                  .AddParameter("@ID", 10)
                  .ExecuteTable<Employee>();
}
```

Requirements: Microsoft Framework 4.0 (Client Profile) for desktop applications, or SQL Server 2008 R2 for SQL CLR Stored procedures, or .NET Standard 2.0 for .NET Core library.

## Basic Samples (video)

[![Samples](http://img.youtube.com/vi/DRfM15Paw8k/0.jpg)](http://www.youtube.com/watch?v=DRfM15Paw8k)

## Performances

Performance is very important during development. 
You can check these values by starting the *Performance* project in the source code. 
Comparing to the famous projects **Dapper** and **Entity Framework**, you can see 
that: 
- **DatabaseCommand** has identical performance when executing Scalar queries. 
- **DatabaseCommand** is slightly less efficient than Dapper (35%) but significantly more efficient than EFCore. 

This slight reduction of performance is due to all additional features included in **DatabaseCommand**:
management of exceptions, Tags, Logs, ActionBeforeExecution and ActionAfterExecution events. 

```
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores

------------------------------------------------------------------------------
           Execute Scalar             |           Execute Table
------------------------------------------------------------------------------
Dapper            |  80.18 탎         | Dapper           | 103.88 탎	
DatabaseCommand   |  81.86 탎  +2%    | DatabaseCommand  | 141.68 탎  +36%
EF Core           | 342.05 탎  +327%  | EF Core          | 315.30 탎  +204%
------------------------------------------------------------------------------
```

## Commands

- [ExecuteTable](#ExecuteTable): Execute a SQL query and retrieve all data to a list of C# objects.
- [AddParameter](#ExecuteTableWithParameters): Execute a SQL query, add some parameters and retrieve all data to a list of C# objects.
- [ExecuteRow](#ExecuteRow): Execute a SQL query and retrieve the first row to one serialized C# object.
- [ExecuteScalar](#ExecuteScalar): Execute a SQL query and retrieve the first value (first row / first column) to a C# data type.
- [ExecuteDataSet](#ExecuteDataSet): Execute multiple SQL queries and retrieve all tables serialized C# objets.
- [FluentQuery](#FluentQuery): Define and execute queries unsing a Fluent style.
- [TransactionBegin](#TransactionBegin): Manage your SQL Transactions.
- [Logging](#Logging): Trace all SQL queries sent to the server (in Text or HTML format).
- [ActionBeforeExecution and ActionAfterExecution](#ActionsBeforeAfter): Define actions to execute immediately before and after the query execution.
- [ThrowException](#ThrowException): Disable the SqlException to avoid application crashes... and catch it via the Exception property or ExceptionOccured event.
- [RetryIfExceptionsOccureds](#RetryIfExceptionsOccured): Avoid DeadLocks with retrying your Execute commands maximum 3 times.
- [Best Practices](#BestPractices): Copy our samples and use it as templates.
- [Entities Generators](#EntitiesGenerator): Generate automatically all classes from your database classes (via a T4 file).


#### <a name="ExecuteTable"></a>ExecuteTable

```csharpharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText = " SELECT * FROM EMP ";
        var emps = cmd.ExecuteTable<Employee>();
    }
```

Using a Fluent syntax.

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        var emps = cmd.Query(" SELECT * FROM EMP WHERE EMPNO > @ID ")
                      .AddParameter("ID", 10)
                      .ExecuteTable<Employee>();
    }
```

Calling an Execute method using a **dynamic** return type.

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText = " SELECT * FROM EMP ";
        var emps = cmd.ExecuteTable<dynamic>();
    }
```

#### ExecuteTable customized

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText = " SELECT EMPNO, HIREDATE FROM EMP ";
        var data = cmd.ExecuteTable<Employee>((row) =>
        {
            return new Employee()
            {
                EmpNo = row.Field<int>("EMPNO"),
                Age = DateTime.Today.Year - row.Field<DateTime>("HIREDATE").Year
            };
        });
    }
```

#### <a name="ExecuteTableWithParameters"></a>ExecuteTable with parameters

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText.AppendLine(" SELECT * ")
                       .AppendLine("   FROM EMP ")
                       .AppendLine("  WHERE EMPNO = @EmpNo ")
                       .AppendLine("    AND HIREDATE = @HireDate ");

        cmd.AddParameter(new
                {
                    EmpNo = 7369,
                    HireDate = new DateTime(1980, 12, 17)
                });

        var emps = cmd.ExecuteTable<Employee>();
    }
```

#### <a name="ExecuteRow"></a>ExecuteRow

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText = " SELECT * FROM EMP WHERE EMPNO = 7369 ";
        var emp = cmd.ExecuteRow<EMP>();
    }
```

#### ExecuteRow customized

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText = " SELECT * FROM EMP WHERE EMPNO = 7369 ";
        var emp = cmd.ExecuteRow((row) =>
        {
            return new
            {
                Number = Convert.ToInt32(row["EMPNO"]),
                Name = Convert.ToString(row["ENAME"])
            };
        });
    }
```

#### <a name="ExecuteScalar"></a>ExecuteScalar

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText = " SELECT COUNT(*) FROM EMP ";
        int data = cmd.ExecuteScalar<int>();
    }
```

#### <a name="ExecuteDataSet"></a>ExecuteDataSet

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText.AppendLine(" SELECT * FROM EMP; ");
        cmd.CommandText.AppendLine(" SELECT * FROM DEPT; ");
        var data = cmd.ExecuteDataSet<Employee, Department>();

        int empCount = data.Item1.Count(); 
    }
```

#### <a name="FluentQuery"></a>FluentQuery

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        int count = cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID")
                       .AddParameter("ID", 10)
                       .ExecuteScalar<int>();

        var employees = cmd.Query(@"SELECT EMPNO, ENAME 
                                      FROM EMP 
                                     WHERE EMPNO > @ID",
                           .AddParameter( new { ID = 10 } )
                           .ExecuteTable( new 
                           { 
                               EmpNo = 0, 
                               EName = String.Empty 
                           });
    }
```

#### <a name="TransactionBegin"></a>TransactionBegin

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText = " DELETE FROM EMP ";

        cmd.TransactionBegin();
        cmd.ExecuteNonQuery();
        cmd.TransactionRollback();
    }
```

Other sample

```csharp
    using (var cmd1 = new DatabaseCommand(_connection))
    {
        cmd1.CommandText.AppendLine(" DELETE FROM EMP ");
        cmd1.TransactionBegin();
        cmd1.ExecuteNonQuery();
        using (var cmd2 = new DatabaseCommand(_connection, cmd1.Transaction))
        {
            cmd2.CommandText = " SELECT COUNT(*) FROM EMP ";
            int count = cmd2.ExecuteScalar<int>();
        }
        cmd1.TransactionRollback();
    }
```

#### <a name="Logging"></a>Logging
All SQL queries can be traced via the **Log** property.

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        // Easy
        cmd.Log = Console.WriteLine;
        
        // Lambda expression
        cmd.Log = (query) => 
        {
            Console.WriteLine(cmd.Formatted.CommandAsVariables);
        };
    }
```

#### <a name="ActionsBeforeAfter"></a>ActionBeforeExecution and ActionAfterExecution
Define actions to execute code immediately before or after query execution.
For example, to simplify unit tests or intergations with extra loggers.

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
        
        cmd.ActionBeforeExecution = (command) =>
        {
            command.CommandText.Clear();
            command.CommandText.Append("SELECT 1+1 FROM EMP");
        };

        int count = cmd.ExecuteScalar<int>();       // Returns 2, and not 14
    }
```

#### <a name="ThrowException"></a>ThrowException

```csharp
    cmd.ThrowException = false;
    cmd1.ExceptionOccured += (sender, e) =>
    {
        // Manage SQL Exceptions
    };
```

#### <a name="RetryIfExceptionsOccured"></a>RetryIfExceptionsOccured

When a specific error occured (DeadLock exception), you can define an automatic retry process.

```csharp
    using (var cmd = new DatabaseCommand(_connection))
    {
        cmd.Retry.Activate(options =>
        {
            options.SetDefaultCriteriaToRetry(RetryDefaultCriteria.SqlServer_DeadLock);
            options.MillisecondsBetweenTwoRetries = 1000;
            options.NumberOfRetriesBeforeFailed = 3;
        });

        cmd.CommandText = "SELECT COUNT(*) FROM EMP";
        int count = cmd.ExecuteScalar<int>();
    }
```

#### <a name="BestPractices"></a>Best practices

In you project, create a <b>DataService</b> implementing IDisposable and add a method GetDatabaseCommand.

##### 1. Using ConnectionString for all applications or threads (ex. Web Applications, WebAPI, Web Services, ...)

```csharp
        public class DataService : IDataService
        {
            public DatabaseCommand GetDatabaseCommand()
            {
                return new DatabaseCommand(CONNECTION_STRING);
            }

            public DatabaseCommand GetDatabaseCommand(SqlTransaction transaction)
            {
                return new DatabaseCommand(transaction.Connection, transaction);
            }
        }
```

##### 2. Using One SqlConnection for the application (ex. Desktop Apps, Universal Apps, ...)

```csharp
        public class DataService : IDataService, IDisposable
        {
            private SqlConnection _connection = null;

            public DataService()
            {
                _connection = new SqlConnection(CONNECTION_STRING);
                _connection.Open();
            }

            public DatabaseCommand GetDatabaseCommand()
            {
                return new DatabaseCommand(_connection);
            }

            public DatabaseCommand GetDatabaseCommand(SqlTransaction transaction)
            {
                return new DatabaseCommand(_connection, transaction);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                        _connection.Dispose();
                        _connection = null;
                    }
                }
            }

            ~DataService()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }   
```

#### <a name="EntitiesGenerator"></a>Entities Generator

You can use a <a href="https://en.wikipedia.org/wiki/Text_Template_Transformation_Toolkit">T4 file</a> to generate all classes associated to your database tables.
Copy this [sample .tt file](https://github.com/Apps72/Dev.Data/blob/master/Test/SqlServer/Entities/Scott.tt) in your project and set your correct **Connection String**. Check if the .tt file properties are **Build Action** = Content and **Custom Tool = TextTemplatingFileGenerator**.

**NEW** Since version 2.6, you can use the <a href="https://www.nuget.org/packages/Apps72.Dev.Data.Generator.Tools/">Data.Generator.Tools</a> to quickly generate all classes associated to your database tables.
Requirements: install the .NET Core 2.1 SDK.
Example: `DbCmd GenerateEntities -cs="Server=localhost;Database=Scott;" --provider=SqlServer` will create a Output.cs file with all entities.

```csharp
    // UPDATE THIS CONNECTION STRING
    const string CONNECTION_STRING = @"Server=(localdb)\ProjectsV12;Database=Scott;Integrated Security=true;";
```

Each time you save this .tt file, you create an equivalent .cs file with all classes.

For example:

```csharp
    // *********************************************
    // Code Generated with Apps72.Dev.Data.Generator
    // *********************************************
    using System;

    namespace Apps72.Dev.Data.Tests.Entities
    {
        /// <summary />
        public partial class DEPT
        {
            /// <summary />
            public virtual Int32 DEPTNO { get; set; }
            /// <summary />
            public virtual String DNAME { get; set; }
            /// <summary />
            public virtual String LOC { get; set; }
        }
        /// <summary />
        public partial class EMP
        {
            /// <summary />
            public virtual Int32 EMPNO { get; set; }
            /// <summary />
            public virtual String ENAME { get; set; }
            /// <summary />
            public virtual Int32? MGR { get; set; }
            /// <summary />
            public virtual DateTime? HIREDATE { get; set; }
            /// <summary />
            public virtual Int32? SAL { get; set; }
            /// <summary />
            public virtual Int32? DEPTNO { get; set; }
        }
    }
```

## <a name="ReleaseNotes"></a>Release Notes

### Version 1.2

* Initial version with all basic features.

### Version 1.3

* Add a extension method **SqlParameterCollection.AddValues** to simplify the creation of parameters.

### Version 1.4

* Add an **EntitiesGenerator** class to generate all classes associated to an existing Database, via the file **Entities.tt**.

### Version 1.5

* All code reviewed and rebuilt with .NET Core framework (https://dotnet.github.io)
* Fix the Numeric SQL type to Decimal C# type.

### Version 1.5.2

* Fix using a Transaction in constructors: the transaction will be not disposed with the DatabaseCommandBase.

### Version 2.0

* Source code Refactoring.
* Add the **ExecuteTableSet** method to get multiple tables, using multiple SELECT commands in one query.
* Add OracleDatabaseCommand to manage Oracle Server databases (need the Oracle.ManagedDataAccess assembly).

### Version 2.1

* Fix using the constructor with ConnectionString and CommandText parameters (the CommandText was not correctly assigned).

### Version 2.2

* Add a **DotNetCore** version with features based on DbConnection.
* Add the method **AddParameter** in DatabaseCommandBase, usable for all projects (SqlServer, Oracle, Sqlite, ...).
* Remove DataInjection concept. That will be replaced by pre and post execution events.

### Version 2.3

* Fix using Dispose method with AutoDisconnect mode.
* Fix when ThrowException = False: returns the default value and not an exception.

### Version 2.4

* Add **dynamic** return value. Example: *var emps = cmd.ExecuteTable&lt;**dynamic**&gt;();*

### Version 2.5

* Add properties **ActionBeforeExecution** and **ActionAfterExecution** to inject code before and after SQL query executions.

### Version 2.6

* Add a FluentQuery feature to create quickly new commands.
  Example:  cmd.Query("SELECT COUNT(*) FROM EMP WHERE EMPNO > @ID", new { ID = 10 }).ExecuteScalar<int>();
* Update SqlEntitiesGenerator to generate SQL Server, Oracle or SQLite entities.
* Add a command line tool to generate entity classes.

### Version 2.7

* Add DataRow converter when executing a command (ExecuteTable<T>(Func<DataRow, T> converter)).
* Renamed IDatabaseCommandBase to IDatabaseCommand to simplify interface usage. IDatabaseCommandBase is always usable.

### Version 2.7.5

* FIX: When the CommandText is empty, returns a empty value (zero array, null value or zero).
* FIX: For the Generator, set the correct type for SQL Server type TINYINT (System.Byte).

### Version 2.8

* FIX: Check if the argument of `AddParameter<T>(T values)` method is a DbParameter.
* Add a new argument to `AddParameter` method, to define the parameter size.

### Version 3.0

* Migrate the code to .NET Standard 2.0
* Refactoring some methods (old methods are flagged [Obsolete])
* Add `Tags` property and `TagWith` method to identify SQL queries.
* Add `Formatted.CommandAsVariables` property to get the SQL query with parameters defined as SQL variables (to be executable in Query tool).
* Add `Reply` property to automatically reply a query when an specified error occured (ex. for DeadLock).

### [RoadMap]

* Include Insert, Delete, Update method to simplify the CRUD operations in one table of database.
* Include Asynchronous methods.

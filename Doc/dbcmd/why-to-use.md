# Why to use DatabaseCommand?

I developed a first version of this project several years ago. My personal reasons are mainly.
These criteria are based on years of experience in the development of applications connected to databases and they only commit me.

---

" **DatabaseCommand** is a very lightweight command library that is based on SQL queries and never modifies these queries.

Software development often requires data to be stored in a relational database such as SQL Server or Oracle. It is very important that the performance is optimal. Solutions such as **Entity Framework** generate most of the queries for you, which often leads to slow data retrieval several months after the project is put into production.

DatabaseCommand is very simple and very powerful, similar to the [**Dapper** project](https://github.com/DapperLib/Dapper).
The big difference with **Dapper**, is that **DatabaseCommand** is not based on extension methods, but on a more configurable object (for logs, traces, properties, ...). A best practice is to add a methode `GetDatabaseCommand()` from a **DataService** object. 

Therefore, **DatabaseCommand** can be built and configured once and globally in the application. This allows for a centralized further development. This simplifies maintenance and the risk of errors. 
DatabaseCommand has a more detailed syntax, which helps to understand and maintain the code. " (_Denis Voituron_)

---

## Samples:

**Using Dapper**

```csharp
var dog = connection.Query<Dog>("SELECT Age = @Age, Id = @Id", new { Age = (int?)null, Id = guid });
```

**Using DatabaseCommand**

```CSharp
using (var cmd = new DatabaseCommand(connection))
{
    cmd.CommandText = "SELECT Age = @Age, Id = @Id";
    cmd.AddParameter("@Age",  (int?)null);
    cmd.AddParameter("@Id",  guid);
    var dog = cmd.ExecuteTable<Dog>();
}
```
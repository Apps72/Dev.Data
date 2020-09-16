Installation: 
`dotnet tool install -g Apps72.Dev.Data.Generator.Tools`

Example: 

- This command **gets all tables/columns** and generates an Entities.cs file with all equivalent classes.
`DbCmd GenerateEntities -cs=Server=localhost;Database=Scott; --provider=SqlServer`

- This command **merges all sql files**, of the current directory, to a new one.
`DbCmd Merge --source=C:\Temp --output=allScripts.sql`

- This command **executes all sql files**, of the current directory, to this SQL Server.
`DbCmd Run --source=C:\Temp -cs=Server=localhost;Database=Scott;`

  With **Run** command, use `--DbConfigAfter` and `--DbConfigUpdate` to set SQL queries to
  select and to update a database field with the name of the last executed script. 
  Only next scripts will be read and executed.

**More details**:
Use `DbCmd --Help` to display all commands and options.
Go to [https://apps72.com](https://apps72.com) for the documentation.

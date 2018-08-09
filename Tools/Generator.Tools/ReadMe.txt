To create the NuGet tool package: 
   C:\> dotnet pack -c Release 

To install and use this package locally:
   Copy the generated package to C:\Program Files\dotnet\sdk\NuGetFallbackFolder\Apps72.Dev.Data.Generator.Tools
   C:\> dotnet tool install -g Apps72.Dev.Data.Generator.Tools

The application will be installer in the current user folder: %USERPROFILE%\.dotnet\tools

To display all tool installed:
  C:\> dotnet tool list -g

To uninstall the tool:
  C:\> dotnet tool uninstall -g Apps72.Dev.Data.Generator.Tools


----***************************
---- Package Documentation 
----***************************

DbCmd is a command line tools to generate entities (class) from existing databases (SQL Server, Oracle or SQLite). 
This tool is also an assembly usable by your core project to retrieve tables and columns, 
and to generate C# or TypeScript code.

**Installation**
`dotnet tool install -g Apps72.Dev.Data.Generator.Tools`

**Update (if already installer)**
`dotnet tool update -g Apps72.Dev.Data.Generator.Tools`

**Use Help to display all commands and options.**
`DbCmd --Help`

**Example:**
This command gets all tables/columns and generates an Output.cs file with all equivalent classes.
```
C:\> DbCmd GenerateEntities -cs="Server=MyServer;Database=Scott;" --provider=SqlServer

SqlDatabase Command Line Tools
Project on https://github.com/Apps72/Dev.Data
  Entities generating...
  4 entities generated in Entities.cs. 1.22 seconds.
```

```
namespace Entities
{
    using System;

    /// <summary />
    public partial class DEPT
    {
        /// <summary />
        public virtual int DEPTNO { get; set; }
        /// <summary />
        public virtual string DNAME { get; set; }
        /// <summary />
        public virtual string LOC { get; set; }
    }
    ...
}
```
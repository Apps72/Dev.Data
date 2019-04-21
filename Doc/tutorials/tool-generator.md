# Tools - Entity generator

A simple command line tools allows you to generate all the entities (classes) 
from existing databases (SQL Server, Oracle or SQLite).
This is often needed to retrieve typed data from the database server.
This tool is also an assembly usable by your core project to retrieve tables and columns, 
and to generate C# or TypeScript code.

## Installation: 

This tool is hosted by [Nuget.org](https://www.nuget.org/packages/Apps72.Dev.Data.Generator.Tools) server.

This package contains a .NET Core Global Tool you can call from the shell/command line.

```Shell
dotnet tool install -g Apps72.Dev.Data.Generator.Tools
```

## First sample

This command gets all tables/columns and generates an Entities.cs file with all equivalent classes.

```Shell
DbCmd GenerateEntities -cs="Server=localhost;Database=Scott;" --provider=SqlServer
```

Result:
```CSharp
public partial class EMP
{
    public virtual int EMPNO { get; set; }
    public virtual string ENAME { get; set; }
    public virtual string JOB { get; set; }
    public virtual int? MGR { get; set; }
    public virtual DateTime? HIREDATE { get; set; }
    public virtual decimal? SAL { get; set; }
}
```

## Options for _GenerateEntities_ command

Use `DbCmd --Help` to display all commands and options (see below).

```Shell
Usage: DbCmd <command> [options]

 Commands:
   GenerateEntities   | ge     Generate a file (see --Output) with all entities
                               extracted from tables and view of specified database.

 Options:
   --ConnectionString | -cs    Required. Connection string to the database server.
                               See https://www.connectionstrings.com
   --Attribute        | -a     Include the Column attribute if necessary.
                               If value is empty, the Apps72 Column attribute is added.
                               You can set the full qualified name of attribute to add
                               in addition of Apps72.
                               Ex: -a="System.ComponentModel.DataAnnotations.Schema.Column"
   --ClassFormat      | -cf    Format of class: NameOnly or SchemaAndName.
   --CodeAnalysis     | -ca    Exception codes to add in top of file to avoid Code Analysis
                               warning. Code separator is ','. Ex: AV1706, AV1507).
   --Language         | -l     Target format: CSharp (only this one at this moment).
   --Namespace        | -ns    Name of the namespace to generate.
   --Output           | -o     File name where class will be written.
   --OnlySchema       | -os    Only for the specified schema.
   --Provider         | -p     Type of server: SqlServer, Oracle or SqLite.

     By default, Provider=SqlServer, Output=Entities.cs, Language=CSharp
                 Namespace=[Empty], ClassFormat=NameOnly

Example:
  DbCmd ge -cs="Server=localhost;Database=Scott;" -p=SqlServer -a
```
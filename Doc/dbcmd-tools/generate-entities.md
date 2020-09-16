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
   Merge              | mg     Merge all script files to a single global file.

   Run                | rn     Run all script files into the database specified by --ConnectionString.

 'GenerateEntities' options:
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
   --NullableRefTypes | -nrt   Use the C# 8.0 nullable reference types.
                               See https://docs.microsoft.com/dotnet/csharp/nullable-references
   --Output           | -o     File name where class will be written.
   --OnlySchema       | -os    Only for the specified schema.
   --Provider         | -p     Type of server: SqlServer, Oracle or SqLite.
   --SortProperties   | -sp    Sort generated properties alphabetically.

     By default, Provider=SqlServer, Output=Entities.cs, Language=CSharp
                 Namespace=[Empty], ClassFormat=NameOnly

 'Merge' options:
   --Source           | -s     Source directory pattern containing all files to merged.
                               Default is "*.sql" in current directory.
   --Output           | -o     File name where all files will be merged.
                               If not set, the merged file will be written to the console.
   --Separator        | -sp    Add this separator between each merged files.
                               Ex: -sp=GO

 'Run' options:
   --ConnectionString | -cs    Required. Connection string to the database server.
                               See https://www.connectionstrings.com
   --Source           | -s     Source directory pattern containing all files to merged.
                               Default is "*.sql" in current directory.
   --Separator        | -sp    Split script using this separator, to execute part of script
                               and not a global script. Set -sp=GO for SQL Server.
   --Provider         | -p     Type of server: SqlServer, Oracle or SqLite.
   --DbConfigAfter    | -ca    Query to get the last file executed (without extension);
                               And run only files with name greater than this value.
                               Ex: -ca="SELECT [Value] FROM [Configuration] WHERE [Key] = 'DbVer'"
   --DbConfigUpdate   | -cu    Query to update the last file executed.
                               The variable @Filename will be replace with the SQL file name (without extension).
                               Ex: -cu="UPDATE [Configuration] SET [Value] = @Filename WHERE [Key] = 'DbVer'"

Example:
  DbCmd GenerateEntities -cs="Server=localhost;Database=Scott;" -p=SqlServer -a
  DbCmd Merge --source="C:\Temp\*.sql" --output=allScripts.sql
  DbCmd Run --source="C:\Temp\*.sql" -cs="Server=localhost;Database=Scott;"
```
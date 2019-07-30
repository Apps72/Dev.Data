# Tools - Merge files

A simple command line tools allows you to merge multiple SQL files to one full script.

## Installation: 

This tool is hosted by [Nuget.org](https://www.nuget.org/packages/Apps72.Dev.Data.Generator.Tools) server.

This package contains a .NET Core Global Tool you can call from the shell/command line.

```Shell
dotnet tool install -g Apps72.Dev.Data.Generator.Tools
```

## First sample

This command merges all SQL files from the Temp folder to _AllScripts.sql_ file.
By default, scripts are merged using `GO` keyword (see _Separator_ flag).

```Shell
DbCmd Merge --source="C:\Temp\*.sql" --output=AllScripts.sql
```

## Options for _Merge_ command

Use `DbCmd --Help` to display all commands and options (see below).

```Shell
Usage: DbCmd GenerateEntities [options]

Options:
  --Source           | -s     Source directory pattern containing all files to merged.
                              Default is "*.sql" in current directory.
  --Output           | -o     File name where all files will be merged.
                              If not set, the merged file will be written to the console.
  --Separator        | -sp    Add this separator between each merged files.
                              Ex: -sp=GO
```
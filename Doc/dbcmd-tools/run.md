# Tools - Run multiple SQL Scripts

A simple command line tools allows you to execute multiple SQL files to a database server.

## Installation: 

This tool is hosted by [Nuget.org](https://www.nuget.org/packages/Apps72.Dev.Data.Generator.Tools) server.

This package contains a .NET Core Global Tool you can call from the shell/command line.

```Shell
dotnet tool install -g Apps72.Dev.Data.Generator.Tools
```

## First sample

This command runs all SQL files from the Temp folder to your localhost server and database _Scott_.
By default, scripts are merged using `GO` keyword (see _Separator_ flag).

```Shell
DbCmd Run --source="C:\Temp\*.sql" -cs="Server=localhost;Database=Scott;"
```

If a script contains the keyword `Go` (by default, for SQL Server), the script will be splitted to multiple sub-script, before to execute each sub-scripts.

## Second sample

Use `--DbConfigAfter` and `--DbConfigUpdate` flags to select or update a version number to execute only files with name > this version.

```Shell
DbCmd Run --source="C:\Temp\*.sql" 
          -cs="Server=localhost;Database=Scott;" 
          -ca="SELECT [Value] FROM [Configuration] WHERE [Key] = 'DbVer'" 
          -cu="UPDATE [Configuration] SET [Value] = @Filename WHERE [Key] = 'DbVer'"
```

For example, if the database contains _Configuration.DbVer = '0002'_, thse scripts will be skipped or executed, depending the filename.

  |File|Used|
  |---|---|
  |0001.sql|Skipped|
  |0002.sql|Skipped|
  |0003.sql|Executed|
  |0004.sql|Executed|

Using `--DbConfigUpdate` the _Configuration.DbVer_ will be updated with the last value (0004), to pass all script to _Skipped_ in the next command run.

 `@Filename` will be replace by the last executed filename (without extension): _0004_.

## Options for _Merge_ command

Use `DbCmd --Help` to display all commands and options (see below).

```Shell
Usage: DbCmd GenerateEntities [options]

Options:
  --ConnectionString | -cs    Required. Connection string to the database server.
                              See https://www.connectionstrings.com
  --Source           | -s     Source directory pattern containing all files to merged.
                              Default is "*.sql" in current directory.
  --Separator        | -sp    Split script using this separator, to execute part of script
                              and not a global script. Set -sp=GO for SQL Server.
  --Provider         | -p     Type of server: SqlServer, Oracle or SqLite.
  --DbConfigAfter    | -ca    Query to get the last file executed (without extension); 
                              And run only files with name greater than this value.
  --DbConfigUpdate   | -cu    Query to update the last file executed.
                              The variable @Filename will be replace with the SQL file name.
```
Installation: 
`dotnet tool install -g Apps72.Dev.Data.Generator.Tools`

Example: 
This command gets all tables/columns and generates an Output.cs file with all equivalent classes.
`DbCmd GenerateEntities -cs="Server=localhost;Database=Scott;" --provider=SqlServer`
`DbCmd Merge --source="C:\Temp" --output=allScripts.sql`
`DbCmd Run --source="C:\Temp" -cs="Server=localhost;Database=Scott;"`

Use DbCmd --Help to display all commands and options.
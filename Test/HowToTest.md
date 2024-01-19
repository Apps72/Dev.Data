# How to test

## Create the **Scott** database

You need to use SQL Server LocalDB to execute the tests. You can download it from [here](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb).
If you already have installed Visual Studio 2017 or 2019, you can use the LocalDB instance that comes with it.

1. Create a new local server called **MyServer**: `SqlLocalDB c MyServer`
2. Start this server using: `SqlLocalDB s MyServer`
3. You can use [SqlCmd](https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-start-utility) to execute the the **Scott.sql** script.
   For example, from the **Test\Core** folder: `SqlCmd -S "(localdb)\MyServer" -i Data\Scott.sql`

> Check the connection string included in the file `Configuration.cs`.
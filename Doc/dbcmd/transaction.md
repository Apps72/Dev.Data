## Transaction

A [transaction](https://docs.microsoft.com/en-us/sql/t-sql/language-elements/transactions-transact-sql) is a single unit of work. If a transaction is successful, all of the data modifications made during the transaction are committed and become a permanent part of the database. 

### TransactionBegin

Start a SQL transaction using `TransactionBegin` method, and keep the Transaction for future command executions.

### TransactionCommit

Use the `TransactionCommit` method to commit the current transaction to the database.

### TransactionRollback

Use the `TransactionRollback` method to rollback the current transaction to the database.

### Example

```csharp
cmd.TransactionBegin();
int count = await cmd.ExecuteNonQueryAsync();
cmd.TransactionRollback();
```
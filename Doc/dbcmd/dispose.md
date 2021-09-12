## Resource disposing

**DatabaseCommand** implement the standard [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable) interface.

The `Dispose` method call the overridable `Cleanup` method to dispose the internal **DbCOmmand** object.
By default, the flag `DatabaseCommand.AlwaysDispose` is **False** to dispose the DbCommand object only if the garbage collector ask that. You can override the Cleanup method or set this flag to True, to always dispose DbCommand.
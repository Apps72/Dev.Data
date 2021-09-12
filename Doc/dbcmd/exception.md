## Exceptions

By default, when an exception occured with the query execution, the library raise a standard [DbException](https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbexception).

### ThrowException property

Sets this **ThrowException** property from True (default value) to **False**, to not raise the standard exception if an error occured with the query.

### Exception property

Gets the last raised exception 

### ExceptionOccured event

When a SQL exception occured, this event is raised to catch all details about this error.

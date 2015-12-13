# Apps72.Dev.Data

## Documentation

This library simplify all SQL Queries to external database. An implementation for SQL Server is included.
Requirements: Microsoft Framework 4.0 for desktop applications or SQL Server 2008 R2 for SQL CLR Stored procedures.

First, you need to create a SqlConnection or to use a ConnectionString. 
The SqlConnection will be not closed by this library
The ConnectionString will instanciated a temporary SqlConnection for this query and will be closed after using.

#### ExecuteTable

	using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
	{
	    cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
	    var emps = cmd.ExecuteTable<Employee>();
	}

#### ExecuteRow

    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
    {
        cmd.CommandText.AppendLine(" SELECT * FROM EMP WHERE EMPNO = 7369");
        EMP emp = cmd.ExecuteRow<EMP>();
    }

#### ExecuteScalar

    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
    {
        cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
        int data = cmd.ExecuteScalar<int>();
    }

#### Logging
All SQL queries can be traced via the <b>.log</b> property.

    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
    {
        cmd.Log = Console.WriteLine;
        ...
    }

#### ExecuteTable customized

    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
    {
        cmd.CommandText.AppendLine(" SELECT EMPNO, HIREDATE FROM EMP ");
        var data = cmd.ExecuteTable<Employee>((row) =>
        {
            return new Employee()
            {
                EmpNo = row.Field<int>("EMPNO"),
                Age = DateTime.Today.Year - row.Field<DateTime>("HIREDATE").Year
            };
        });
    }

#### using DBNull values

To add a <b>null</b> parameter to convert to <b>DBNull.Value</b> :

    cmd.Parameters.AddWithValueOrDBNull("@Comm", null);

To convert a <b>null</b> parameter to <b>DBNull.Value</b> :

    cmd.Parameters.AddWithValue("@Comm", null).ConvertToDBNull();

#### Data Injection - For Unit testing

    // Intercept Query executions to set predefined data.
    _connection.DefineDataInjection((cmd) =>
    {
        List<Employee> employees = new List<Employee>();
        employees.Add(new Employee() { EmpNo = 1 });
        employees.Add(new Employee() { EmpNo = 2 });
        return DataTypedConvertor.ToDataTable(employees);
    });

    // Query executed in "main" program.
    using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(conn))
    {
        cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
        if (cmd.ExecuteTable().Rows.Count >= 2)
            ...
    }

#### Best practices

In you project, create a <b>DataService</b> implementing IDisposable and add a method GetDatabaseCommand.

#####1. Using ConnectionString for all applications or threads (ex. Web Applications, WebAPI, Web Services, ...)

        public class DataService : IDataService
        {
            public SqlDatabaseCommand GetDatabaseCommand()
            {
                return new SqlDatabaseCommand(CONNECTION_STRING);
            }

            public SqlDatabaseCommand GetDatabaseCommand(SqlTransaction transaction)
            {
                return new SqlDatabaseCommand(transaction.Connection, transaction);
            }
        }

#####2. Using One SqlConnection for the application (ex. Desktop Apps, Universal Apps, ...)

        public class DataService : IDataService, IDisposable
        {
            private SqlConnection _connection = null;

            public DataService()
            {
                _connection = new SqlConnection(CONNECTION_STRING);
                _connection.Open();
            }

            public SqlDatabaseCommand GetDatabaseCommand()
            {
                return new SqlDatabaseCommand(_connection);
            }

            public SqlDatabaseCommand GetDatabaseCommand(SqlTransaction transaction)
            {
                return new SqlDatabaseCommand(_connection, transaction);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                        _connection.Dispose();
                        _connection = null;
                    }
                }
            }

            ~DataService()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }   
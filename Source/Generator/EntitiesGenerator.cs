using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// Extractor of all tables and columns properties from Database specified in the ConnectionString.
    /// </summary>
    public class EntitiesGenerator
    {
        private string _connectionString = string.Empty;
        private SqlConnection _connection = null;
        private List<Table> _tables = new List<Table>();

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public EntitiesGenerator(string connectionString)
        {
            _connectionString = connectionString;
            this.GetAllTablesAndColumns();
        }


        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summaryconnection
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public EntitiesGenerator(SqlConnection connection)
        {
            _connection = connection;
            this.GetAllTablesAndColumns();
        }

        /// <summary>
        /// Gets all tables founds
        /// </summary>
        public IEnumerable<Table> Tables
        {
            get
            {
                return _tables.Where(t => t.IsView == false);
            }
        }

        /// <summary>
        /// Search all table names and columns names in SQL Server
        /// </summary>
        private void GetAllTablesAndColumns()
        {
            using (SqlDatabaseCommand cmd = _connection == null ? new SqlDatabaseCommand(_connectionString) : new SqlDatabaseCommand(_connection))
            {
                cmd.CommandText.AppendLine(" SELECT sys.schemas.name AS SchemaName, ");
                cmd.CommandText.AppendLine("        sys.tables.name AS TableName, ");
                cmd.CommandText.AppendLine("        sys.columns.name AS ColumnName, ");
                cmd.CommandText.AppendLine("        sys.systypes.name AS ColumnType, ");
                cmd.CommandText.AppendLine("        sys.columns.max_length AS ColumnSize, ");
                cmd.CommandText.AppendLine("        sys.columns.is_nullable AS IsColumnNullable, ");
                cmd.CommandText.AppendLine("        CAST(0 AS BIT) AS IsView ");
                cmd.CommandText.AppendLine(" FROM   sys.tables ");
                cmd.CommandText.AppendLine("        INNER JOIN sys.schemas ON sys.tables.schema_id = sys.schemas.schema_id ");
                cmd.CommandText.AppendLine("        INNER JOIN sys.columns ON sys.tables.object_id = sys.columns.object_id ");
                cmd.CommandText.AppendLine("        INNER JOIN sys.systypes ON sys.systypes.xtype = sys.columns.system_type_id ");
                cmd.CommandText.AppendLine(" ORDER BY SchemaName, TableName, column_id ");

                var tableAndColumns = cmd.ExecuteTable<TableAndColumn>();

                foreach (TableAndColumn column in tableAndColumns)
                {
                    // If this table is not already existing, create it.
                    if (!_tables.Any(t => t.Name == column.TableName && t.Schema == column.SchemaName))
                    {
                        _tables.Add(new Table()
                        {
                            Schema = column.SchemaName,
                            Name = column.TableName,
                            IsView = column.IsView,
                            Columns = new List<Column>()
                        });
                    }

                    // Fill all columns
                    List<Column> columns = _tables.First(t => t.Name == column.TableName && t.Schema == column.SchemaName).Columns as List<Column>;
                    columns.Add(new Column()
                    {
                        Name = column.ColumnName,
                        SqlType = column.ColumnType,
                        IsNullable = column.IsColumnNullable
                    });
                }
            }
        }

        /// <summary />
        private class TableAndColumn
        {
            /// <summary />
            public string SchemaName { get; set; }
            /// <summary />
            public string TableName { get; set; }
            /// <summary />
            public string ColumnName { get; set; }
            /// <summary />
            public string ColumnType { get; set; }
            /// <summary />
            public int ColumnSize { get; set; }
            /// <summary />
            public bool IsColumnNullable { get; set; }
            /// <summary />
            public bool IsView { get; set; }
        }

    }

    /// <summary />
    public class Table
    {
        /// <summary />
        public string Name { get; set; }
        /// <summary />
        public string Schema { get; set; }
        /// <summary />
        public bool IsView { get; set; }
        /// <summary />
        public IEnumerable<Column> Columns { get; set; }
    }

    /// <summary />
    public class Column
    {
        /// <summary />
        public string Name { get; set; }
        /// <summary />
        public bool IsNullable { get; set; }
        /// <summary />
        public string SqlType { get; set; }
        /// <summary />
        public System.Data.SqlDbType? SqlDbType
        {
            get
            {
                System.Data.SqlDbType sqlDbType;
                if (Enum.TryParse(this.SqlType, true, out sqlDbType))
                {
                    return sqlDbType;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary />
        public string CSharpType
        {
            get
            {
                System.Data.SqlDbType? sqlDbType = this.SqlDbType;
                if (sqlDbType.HasValue)
                    return SqlDataTypedConvertor.ToNetType(sqlDbType.Value).Name;
                else
                    return "Object";
            }
        }

    }
}

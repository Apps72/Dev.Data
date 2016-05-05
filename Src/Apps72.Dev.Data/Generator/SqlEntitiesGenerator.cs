using Apps72.Dev.Data.Convertor;
using System;
using System.Collections.Generic;
using System.Linq;

#if NET451
using System.Data.SqlClient;
#endif

namespace Apps72.Dev.Data.Generator
{
#if NET451
    /// <summary>
    /// Extractor of all tables and columns properties from Database specified in the ConnectionString.
    /// </summary>
    public class SqlEntitiesGenerator
    {
        private string _connectionString = string.Empty;
        private SqlConnection _connection = null;

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(string connectionString)
        {
            _connectionString = connectionString;
            this.FillAllTablesAndColumns();
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summaryconnection
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(SqlConnection connection)
        {
            _connection = connection;
            this.FillAllTablesAndColumns();
        }

        /// <summary>
        /// Gets all tables founds
        /// </summary>
        public virtual IEnumerable<Table> Tables { get; private set; }

        /// <summary>
        /// Search all table names and columns names in SQL Server
        /// </summary>
        protected virtual void FillAllTablesAndColumns()
        {
            List<Table> tablesFound = new List<Table>();
            IEnumerable<TableAndColumn> tableAndColumns;

            using (SqlDatabaseCommand cmd = this.GetDatabaseCommand())
            {
                cmd.CommandText.AppendLine(" SELECT sys.schemas.name AS SchemaName, ");
                cmd.CommandText.AppendLine("        sys.tables.name AS TableName, ");
                cmd.CommandText.AppendLine("        sys.columns.name AS ColumnName, ");
                cmd.CommandText.AppendLine("        sys.types.name AS ColumnType, ");
                cmd.CommandText.AppendLine("        sys.columns.max_length AS ColumnSize, ");
                cmd.CommandText.AppendLine("        sys.columns.is_nullable AS IsColumnNullable, ");
                cmd.CommandText.AppendLine("        CAST(0 AS BIT) AS IsView ");
                cmd.CommandText.AppendLine(" FROM   sys.tables ");
                cmd.CommandText.AppendLine("        INNER JOIN sys.schemas ON sys.tables.schema_id   = sys.schemas.schema_id ");
                cmd.CommandText.AppendLine("        INNER JOIN sys.columns ON sys.tables.object_id   = sys.columns.object_id ");
                cmd.CommandText.AppendLine("        INNER JOIN sys.types   ON sys.types.user_type_id = sys.columns.user_type_id  ");
                cmd.CommandText.AppendLine(" ORDER BY SchemaName, TableName, column_id ");

                tableAndColumns = cmd.ExecuteTable<TableAndColumn>();
            }

            foreach (TableAndColumn column in tableAndColumns)
            {
                // If this table is not already existing, create it.
                if (!tablesFound.Any(t => t.Name == column.TableName && t.Schema == column.SchemaName))
                {
                    tablesFound.Add(new Table()
                    {
                        Schema = column.SchemaName,
                        Name = column.TableName,
                        IsView = column.IsView,
                        Columns = new List<Column>()
                    });
                }

                // Fill all columns
                List<Column> columns = tablesFound.First(t => t.Name == column.TableName && t.Schema == column.SchemaName).Columns as List<Column>;
                columns.Add(new Column()
                {
                    Name = column.ColumnName,
                    SqlType = column.ColumnType,
                    IsNullable = column.IsColumnNullable
                });
            }

            this.Tables = tablesFound.Where(t => t.IsView == false);

            // Remove extra chars
            foreach (var table in this.Tables)
            {
                table.Name = RemoveExtraChars(table.Name);
                table.Schema = RemoveExtraChars(table.Schema);
                foreach (var col in table.Columns)
                {
                    col.Name = RemoveExtraChars(col.Name);
                }
            }
        }

        /// <summary>
        /// Remove invalid chars for CSharp class and property names.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string RemoveExtraChars(string name)
        {
            return name.Replace(" ", string.Empty);     // TODO: Remove all others chars
        }

        /// <summary>
        /// Returns a new reference to SqlDatabaseCommand
        /// </summary>
        /// <returns></returns>
        protected virtual SqlDatabaseCommand GetDatabaseCommand()
        {
            SqlDatabaseCommand command = _connection == null ? new SqlDatabaseCommand(_connectionString) : new SqlDatabaseCommand(_connection);
            command.ThrowException = false;
            return command;
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
    
#endif
}

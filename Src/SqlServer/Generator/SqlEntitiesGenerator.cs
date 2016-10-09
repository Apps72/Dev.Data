using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// Extractor of all tables and columns properties from Database specified in the ConnectionString.
    /// </summary>
    public class SqlEntitiesGenerator : SqlEntitiesGeneratorBase
    {
        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(string connectionString) : this(connectionString, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connectionStringName">ConnectionString or App.Config Name of the ConnectionString to retrieve all tables and columns</param>
        /// <param name="appConfig">Path and name of the App.Config file where the connection string is written</param>
        public SqlEntitiesGenerator(string connectionStringName, string appConfig) : base()
        {
            if (!String.IsNullOrEmpty(appConfig))
            {
                ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
                configFile.ExeConfigFilename = appConfig;
                var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
                this.ConnectionString = configuration.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;
            }
            else
            {
                this.ConnectionString = connectionStringName;
            }
            this.FillAllTablesAndColumns();
        }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summaryconnection
        /// <param name="connectionString">ConnectionString to retrieve all tables and columns</param>
        public SqlEntitiesGenerator(SqlConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Search all table names and columns names in SQL Server
        /// </summary>
        protected override void FillAllTablesAndColumns()
        {
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

            // Select all tables
            var tables = tableAndColumns.GroupBy(i => new { i.TableName, i.SchemaName, i.IsView })
                                        .Select(i => new Schema.DataTable()
                                        {
                                            Schema = i.Key.SchemaName,
                                            Name = i.Key.TableName,
                                            IsView = i.Key.IsView
                                        })
                                        .ToArray();

            // Assign all columns
            for (int i = 0; i < tables.Length; i++)
            {
                var table = tables[i];
                table.Columns = tableAndColumns.Where(c => c.SchemaName == table.Schema && c.TableName == table.Name)
                                               .Select(c => new Schema.DataColumn(table)
                                               {
                                                   ColumnName = RemoveExtraChars(c.ColumnName),
                                                   SqlType = c.ColumnType,
                                                   IsNullable = c.IsColumnNullable
                                               })
                                               .ToArray();
            }
            
            this.Tables = tables.Where(t => t.IsView == false);

            // Remove extra chars
            foreach (var table in this.Tables)
            {
                table.Name = RemoveExtraChars(table.Name);
                table.Schema = RemoveExtraChars(table.Schema);
            }
        }

        /// <summary>
        /// Remove invalid chars for CSharp class and property names.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/gg615485.aspx </remarks>
        private string RemoveExtraChars(string name)
        {
            StringBuilder newName = new StringBuilder();
            int ascii = 0;

            // Keep only digits, letters or underscore
            foreach (char c in name)
            {
                // Ascii code of the current Char
                ascii = (int)c;

                // 0 .. 9, A .. Z, a .. z, _
                if (ascii >= 48 && ascii <= 57 ||
                    ascii >= 65 && ascii <= 90 ||
                    ascii >= 97 && ascii <= 122 ||
                    ascii == 95)
                {
                    newName.Append(c);
                }
            }

            // First char must be a letter or underscore
            if (newName.Length > 0)
            {
                ascii = (int)newName[0];
                if (ascii >= 65 && ascii <= 90 ||
                    ascii >= 97 && ascii <= 122 ||
                    ascii == 95)
                {
                    return newName.ToString();
                }
                else
                {
                    return $"_{newName.ToString()}";
                }
            }
            else
            {
                return $"__{Guid.NewGuid().ToString().Replace('-', '_')}";
            }
            
        }

        /// <summary>
        /// Returns a new reference to SqlDatabaseCommand
        /// </summary>
        /// <returns></returns>
        protected virtual SqlDatabaseCommand GetDatabaseCommand()
        {
            SqlDatabaseCommand command = this.Connection == null ? new SqlDatabaseCommand(this.ConnectionString) : new SqlDatabaseCommand((SqlConnection)this.Connection);
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
}

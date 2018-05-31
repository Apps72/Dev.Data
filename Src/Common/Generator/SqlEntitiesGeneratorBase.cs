using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlEntitiesGeneratorBase
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        protected SqlEntitiesGeneratorBase() { }

        /// <summary>
        /// Initializes a new instance of EntitiesGenerator
        /// </summary>
        /// <param name="connection">Connection to retrieve all tables and columns</param>
        public SqlEntitiesGeneratorBase(DbConnection connection)
        {
            SearchAndFill(connection);
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the DbConnection connected to the database
        /// </summary>
        protected virtual DbConnection Connection { get; set; }

        /// <summary>
        /// Gets all tables and views founds
        /// </summary>
        public virtual IEnumerable<Schema.DataTable> TablesAndViews { get; protected set; }

        /// <summary>
        /// Gets all tables founds
        /// </summary>
        public virtual IEnumerable<Schema.DataTable> Tables => this.TablesAndViews.Where(t => t.IsView == false);

        /// <summary>
        /// Gets all tables founds
        /// </summary>
        public virtual IEnumerable<Schema.DataTable> Views => this.TablesAndViews.Where(t => t.IsView == true);

        /// <summary>
        /// Gets or sets the SQL query used to retrieve all columns information about tables and views.
        /// Must be return these data: SchemaName (string), TableName (string), ColumnName (string), ColumnType (string), ColumnSize (int), IsColumnNullable (bool), IsView (bool)
        /// </summary>
        public virtual string QueryToDescribeTablesAndViews { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Search and returns all table names and columns names in database
        /// </summary>
        protected virtual IEnumerable<TableAndColumn> GetTablesDescription()
        {
            if (string.IsNullOrEmpty(this.QueryToDescribeTablesAndViews))
                throw new ArgumentNullException("The QueryToDescribeTablesAndViews property must be set.");

            // Read description in Database
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = this.QueryToDescribeTablesAndViews;
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    Schema.DataTable data = new Schema.DataTable(dr, firstRowOnly: false);
                    return data.ConvertTo<TableAndColumn>();
                }
            }
        }

        /// <summary>
        /// Search all tables and columns, and fill <see cref="TablesAndViews"/> property.
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void SearchAndFill(DbConnection connection)
        {
            if (connection != null)
            {
                this.Connection = connection;
                this.QueryToDescribeTablesAndViews = this.GetSqlQueryToDescribeTables();
                this.TablesAndViews = ConvertDescriptionsToTables(GetTablesDescription());
            }
        }

        /// <summary>
        /// Tranform the list of tables and columns (description of database tables) to a list of DataTable.
        /// </summary>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Schema.DataTable> ConvertDescriptionsToTables(IEnumerable<TableAndColumn> descriptions)
        {
            // Select all tables
            var tables = descriptions.GroupBy(i => new { i.TableName, i.SchemaName, i.IsView })
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
                table.Columns = descriptions.Where(c => c.SchemaName == table.Schema && c.TableName == table.Name)
                                               .Select(c => new Schema.DataColumn(table)
                                               {
                                                   ColumnName = RemoveExtraChars(c.ColumnName),
                                                   SqlType = c.ColumnType,                                                   
                                                   IsNullable = c.IsColumnNullable
                                               })
                                               .ToArray();
            }

            // Remove extra chars
            foreach (var table in tables)
            {
                table.Name = RemoveExtraChars(table.Name);
                table.Schema = RemoveExtraChars(table.Schema);
            }

            return tables;
        }

        /// <summary>
        /// Remove invalid chars for CSharp class and property names.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/gg615485.aspx </remarks>
        protected virtual string RemoveExtraChars(string name)
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
        /// Returns the SQL query will be executed to retrieve description of tables and columns
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSqlQueryToDescribeTables()
        {
            return string.Empty;
        }

        #endregion

        #region SUBCLASS

        /// <summary />
        protected class TableAndColumn
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
            public Type ColumnDataType { get; set; }
            /// <summary />
            public int ColumnSize { get; set; }
            /// <summary />
            public bool IsColumnNullable { get; set; }
            /// <summary />
            public bool IsView { get; set; }
        }

        #endregion
    }
}

using System;

namespace Apps72.Dev.Data.Generator
{
    /// <summary>
    /// Fields used with DBConnection.GetSchema("Columns")
    /// </summary>
    internal class SchemaColumnsFields
    {
        /// <summary>
        /// Initializes a list of fields to use with DBConnection.GetSchema("Columns")
        /// </summary>
        /// <param name="productName"></param>
        public SchemaColumnsFields(string productName)
        {
            // Fields for SQL Server
            if (IsSqlServerFamily(productName))
            {
                NAME = "Columns";
                SequenceNumber = "ORDINAL_POSITION";
                ColumnName = "COLUMN_NAME";
                TableName = "TABLE_NAME";
                SchemaName = "TABLE_SCHEMA";
                ColumnType = "DATA_TYPE";
                ColumnSize = "CHARACTER_OCTET_LENGTH";
                IsColumnNullable = "IS_NULLABLE";
            }

            // Fields for Oracle Server
            else if (IsOracleFamily(productName))
            {
                NAME = "Columns";
                SequenceNumber = "ID";
                ColumnName = "COLUMN_NAME";
                TableName = "TABLE_NAME";
                SchemaName = "OWNER";
                ColumnType = "DATATYPE";
                ColumnSize = "LENGTH";
                IsColumnNullable = "NULLABLE";
            }

            // Fields for SQLite
            else if (IsSqliteFamily(productName))
            {
                NAME = "Columns";
                SequenceNumber = "ORDINAL_POSITION";
                ColumnName = "COLUMN_NAME";
                TableName = "TABLE_NAME";
                SchemaName = "TABLE_SCHEMA";
                ColumnType = "DATA_TYPE";
                ColumnSize = "CHARACTER_MAXIMUM_LENGTH";
                IsColumnNullable = "IS_NULLABLE";
            }
        }

        /// <summary />
        public string NAME { get; private set; } = "Columns";
        /// <summary />
        public string SequenceNumber { get; private set; } = "ORDINAL_POSITION";
        /// <summary />
        public string SchemaName { get; private set; } = "TABLE_SCHEMA";
        /// <summary />
        public string TableName { get; private set; } = "TABLE_NAME";
        /// <summary />
        public string ColumnName { get; private set; } = "COLUMN_NAME";
        /// <summary />
        public string ColumnType { get; private set; } = "DATA_TYPE";
        /// <summary />
        public string ColumnSize { get; private set; } = "LENGTH";
        /// <summary />
        public string IsColumnNullable { get; private set; } = "IS_NULLABLE";

        /// <summary>
        /// Returns True if the Database Server name is "Oracle"
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        private bool IsOracleFamily(string productName)
        {
            return productName.ToLower().Contains("oracle");
        }

        /// <summary>
        /// Returns True if the Database Server name is "SQLServer"
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        private bool IsSqlServerFamily(string productName)
        {
            return productName.ToLower().Contains("sqlserver") || productName.ToLower().Contains("sql server");
        }

        /// <summary>
        /// Returns True if the Database Server name is "SQLite"
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        private bool IsSqliteFamily(string productName)
        {
            return productName.ToLower().Contains("sqlite");
        }
        
    }
}

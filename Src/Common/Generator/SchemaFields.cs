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
            FindDatabaseFamily(productName);

            switch (this.DatabaseFamily)
            {
                // Fields for SQL Server
                case DatabaseFamily.Unknown:
                case DatabaseFamily.SqlServer:
                    NAME = "Columns";
                    SequenceNumber = "ORDINAL_POSITION";
                    ColumnName = "COLUMN_NAME";
                    TableName = "TABLE_NAME";
                    SchemaName = "TABLE_SCHEMA";
                    ColumnType = "DATA_TYPE";
                    ColumnSize = "CHARACTER_OCTET_LENGTH";
                    IsColumnNullable = "IS_NULLABLE";
                    NumericPrecision = "NUMERIC_PRECISION";
                    NumericScale = "NUMERIC_SCALE";
                    break;

                // Fields for Oracle Server
                case DatabaseFamily.Oracle:
                    NAME = "Columns";
                    SequenceNumber = "ID";
                    ColumnName = "COLUMN_NAME";
                    TableName = "TABLE_NAME";
                    SchemaName = "OWNER";
                    ColumnType = "DATATYPE";
                    ColumnSize = "LENGTH";
                    IsColumnNullable = "NULLABLE";
                    NumericPrecision = "PRECISION";
                    NumericScale = "SCALE";
                    break;

                // Fields for SQLite
                case DatabaseFamily.Sqlite:
                    NAME = "Columns";
                    SequenceNumber = "ORDINAL_POSITION";
                    ColumnName = "COLUMN_NAME";
                    TableName = "TABLE_NAME";
                    SchemaName = "TABLE_SCHEMA";
                    ColumnType = "DATA_TYPE";
                    ColumnSize = "CHARACTER_MAXIMUM_LENGTH";
                    IsColumnNullable = "IS_NULLABLE";
                    NumericPrecision = "NUMERIC_PRECISION";
                    NumericScale = "NUMERIC_SCALE";
                    break;
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
        public string ColumnSize { get; private set; } = "CHARACTER_OCTET_LENGTH";
        /// <summary />
        public string NumericPrecision { get; private set; } = "NUMERIC_PRECISION";
        /// <summary />
        public string NumericScale { get; private set; } = "NUMERIC_SCALE";
        /// <summary />
        public string IsColumnNullable { get; private set; } = "IS_NULLABLE";
        /// <summary />
        public DatabaseFamily DatabaseFamily { get; private set; } = DatabaseFamily.Unknown;

        /// <summary />
        private void FindDatabaseFamily(string productName)
        {
            if (productName.ToLower().Contains("oracle"))
                this.DatabaseFamily = DatabaseFamily.Oracle;
            else if (productName.ToLower().Contains("sqlserver") || productName.ToLower().Contains("sql server"))
                this.DatabaseFamily = DatabaseFamily.SqlServer;
            else if (productName.ToLower().Contains("sqlite"))
                this.DatabaseFamily = DatabaseFamily.Sqlite;
        }

    }
}

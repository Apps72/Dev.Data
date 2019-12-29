using System;

namespace Apps72.Dev.Data.Generator
{
    /// <summary />
    public class TableAndColumn
    {
        /// <summary />
        public DatabaseFamily DatabaseFamily { get; internal set; }
        /// <summary />
        public int SequenceNumber { get; internal set; }
        /// <summary />
        public string SchemaName { get; internal set; }
        /// <summary />
        public string TableName { get; internal set; }
        /// <summary />
        public string ColumnName { get; internal set; }
        /// <summary />
        public string ColumnType { get; internal set; }
        /// <summary />
        public int ColumnSize { get; internal set; }
        /// <summary />
        public int? NumericPrecision { get; internal set; }
        /// <summary />
        public int? NumericScale { get; internal set; }
        /// <summary />
        public bool IsColumnNullable { get; internal set; }

        internal Type GetDataType()
        {
            Type dataType = Convertor.DbTypeMap.FirstType(this.ColumnType);

            if (DatabaseFamily == DatabaseFamily.Oracle)
            {
                // For NUMERIC, check the Scale and Precision to transform to an Int64
                if (this.NumericScale == 0 && this.NumericPrecision > 0 &&
                    (dataType == typeof(decimal) || dataType == typeof(float) || dataType == typeof(double)))
                {
                    if (this.NumericPrecision <= 4)
                        dataType = typeof(System.Int16);
                    else if (this.NumericPrecision <= 9)
                        dataType = typeof(System.Int32);
                    else
                        dataType = typeof(System.Int64);
                }
            }

            return dataType;
        }
    }
}

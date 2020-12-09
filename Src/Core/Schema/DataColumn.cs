using Apps72.Dev.Data.Convertor;
using System;
using System.Diagnostics;

namespace Apps72.Dev.Data.Schema
{
    /// <summary>
    /// Represents the schema of a column in a Table.
    /// </summary>
    [DebuggerDisplay("{ColumnName} {SqlType}")]
    public partial class DataColumn
    {
        /// <summary>
        /// Initialize a new instance of <see cref="DataColumn"/>.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="columnName"></param>
        /// <param name="sqlType"></param>
        /// <param name="dataType"></param>
        /// <param name="isNullable"></param>
        internal DataColumn(int ordinal, string columnName, string sqlType, Type dataType, bool isNullable, int? size = 0, int? precision = 0, int? scale = 0)
        {
            Ordinal = ordinal;
            ColumnName = columnName;
            SqlType = sqlType;
            DataType = dataType;
            IsNullable = isNullable;
            Size = size;
            Precision = precision;
            Scale = scale;

            // If unicode, size is divided by 2
            if (sqlType != null)
            {
                if (sqlType.StartsWith("NCHAR", StringComparison.OrdinalIgnoreCase) ||
                    sqlType.StartsWith("NVARCHAR", StringComparison.OrdinalIgnoreCase) ||
                    sqlType.StartsWith("NTEXT", StringComparison.OrdinalIgnoreCase))
                {
                    Size = size / 2;
                }
            }
        }

        /// <summary>
        /// Gets the (zero-based) position of the column in the Columns collection.
        /// </summary>
        public int Ordinal { get; }

        /// <summary>
        /// Gets the name of the column
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the name of the column
        /// </summary>
        public string DotNetColumnName => this.ColumnName.RemoveExtraChars();

        /// <summary>
        /// Gets the Original SQL DataType retrieve in the database (ex. INTEGER)
        /// </summary>
        public string SqlType { get; }

        /// <summary>
        /// Gets the type of data stored in the column.
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// Gets a value that indicates whether null values are allowed in this column
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Gets the DataType formated as a string
        /// </summary>
        public string DotNetType => DataType.ToString().Replace("System.", String.Empty);

        /// <summary>
        /// Gets the DataType formated as a string
        /// </summary>
        public string CSharpType => Convertor.DbTypeMap.DotNetToCSharpType(DataType);

        /// <summary>
        /// Gets the DataType formated as a string suffixed by an option "?"
        /// </summary>
        public string DotNetTypeNullable => GetTypeNullable(isCSharp: false, withNullableReferenceTypes: false);

        /// <summary>
        /// Gets the DataType formated as a string suffixed by an option "?"
        /// </summary>
        public string CSharpTypeNullable => GetTypeNullable(isCSharp: true, withNullableReferenceTypes: false);

        /// <summary>
        /// Gets the DataType formated as a string suffixed by an option "?"
        /// Also for object?, string? as defined in Nullable reference types.
        /// https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references
        /// </summary>
        public string CSharp8TypeNullable => GetTypeNullable(isCSharp: true, withNullableReferenceTypes: true);

        /// <summary>
        /// Gets the type size (length).
        /// </summary>
        public int? Size { get; }

        /// <summary>
        /// Gets the type precision.
        /// </summary>
        public int? Precision { get; }

        /// <summary>
        /// Gets the type scale.
        /// </summary>
        public int? Scale { get; }

        /// <summary>
        /// Gets the min/max value, based on Precision and Scale.
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string> GetMinMax()
        {
            // Ex. 1234.56 => Precision = 6  ;  Scale = 2

            const string NINES = "9999999999999999999999999999999999999999";

            if (Precision != null && Scale != null &&
                Precision > 0 && Precision < 38 &&
                Scale >= 0 && Scale < 38 &&
                Precision > Scale)
            {
                string leftPart = NINES.Substring(0, Precision.Value - Scale.Value);
                string rightPart = Scale.Value > 0 ? NINES.Substring(0, Scale.Value) : "0";
                return new Tuple<string, string>($"-{leftPart}.{rightPart}", $"{leftPart}.{rightPart}");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the C# type suffixed by ?, if allowed
        /// Ex. Int32  -> Int32?
        ///     String -> String
        /// </summary>
        /// <returns></returns>
        private string GetTypeNullable(bool isCSharp, bool withNullableReferenceTypes)
        {
            string dataType = DataType.ToString();
            string type = isCSharp ? Convertor.DbTypeMap.DotNetToCSharpType(this.DataType)
                                   : dataType;

            bool addNullableChar = this.IsNullable;

            if (addNullableChar)
            {
                if (dataType == "System.String" ||
                    dataType == "System.Object" ||
                    dataType == "System.Byte[]")
                {
                    if (withNullableReferenceTypes)
                        addNullableChar = true;         // Add '?' for C# 8.0 code
                    else
                        addNullableChar = false;        // Don't add '?' for String, Object or Byte[]
                }
            }

            if (addNullableChar)
            {
                return $"{type}?".Replace("System.", String.Empty);
            }
            else
            {
                return type.Replace("System.", String.Empty);
            }
        }
    }
}

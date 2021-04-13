using System;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Manage the CommantText to display SQL Query including parameters. x
    /// </summary>
    public class CommandTextFormatted
    {
        private DatabaseCommand _dbCommand;

        /// <summary>
        /// Initializes a new instance of CommandTextFormatted
        /// </summary>
        /// <param name="command"></param>
        internal CommandTextFormatted(DatabaseCommand command)
        {
            _dbCommand = command;
        }

        /// <summary>
        /// Gets the <see cref="DatabaseCommand.CommandText"/> 
        /// with parameters replaced by values.
        /// </summary>
        public virtual string CommandAsText => GetSqlFormatted(QueryFormat.Text);


        /// <summary>
        /// Gets the <see cref="DatabaseCommand.CommandText"/> 
        /// with parameters replaced by values, and colorized using HTML tags.
        /// </summary>
        public virtual string CommandAsHtml => GetSqlFormatted(QueryFormat.Html);

        /// <summary>
        /// Gets the <see cref="DatabaseCommand.CommandText"/> 
        /// with parameters replaced by SQL variable delcarations.
        /// </summary>
        public virtual string CommandAsVariables => GetSqlFormatted(QueryFormat.Variables);

        /// <summary>
        /// Gets the CommandText formatted with specified format
        /// </summary>
        /// <param name="format">Use Text to format as Simple SQL Query or use HTML to format as Colored SQL Query.</param>
        /// <returns>Formatted query</returns>
        public virtual string GetSqlFormatted(QueryFormat format)
        {
            switch (format)
            {
                case QueryFormat.Text:
                    return this.GetQueryFormattedAsText(_dbCommand);

                case QueryFormat.Html:
                    return this.GetQueryFormattedAsHtml(_dbCommand);

                case QueryFormat.Variables:
                    return this.GetQueryFormattedAsVariables(_dbCommand);

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns the CommandText with all parameter values included.
        /// </summary>
        /// <param name="command">DBCommand to manage</param>
        /// <returns></returns>
        protected virtual string GetQueryFormattedAsText(DatabaseCommand command)
        {
            string commandText = command.GetCommandTextWithTags();

            // Sort by ParameterName DESC to replace @abcdef before @abc.
            foreach (DbParameter param in command.Parameters.Cast<DbParameter>().OrderByDescending(i => i.ParameterName))
            {
                string paramName = param.ParameterName;
                string prefix = Schema.DataParameter.GetPrefixParameter(command.GetInternalCommand());

                if (!paramName.StartsWith(prefix)) paramName = prefix + paramName;

                commandText = Regex.Replace(commandText, paramName, GetValueFormatted(param), RegexOptions.IgnoreCase);
            }

            return commandText;
        }

        /// <summary>
        /// Format the SQL command in HTML (coloring, ...)
        /// </summary>
        /// <param name="command">Command to format in HTML</param>
        /// <returns></returns>
        protected virtual string GetQueryFormattedAsHtml(DatabaseCommand command)
        {
            string output = this.GetQueryFormattedAsText(command);

            // Transform special characters
            output = output.Replace("&", "&amp;");
            output = output.Replace("\"", "&quot;");
            output = output.Replace("<", "&lt;");
            output = output.Replace(">", "&gt;");
            output = output.Replace(Environment.NewLine, "<br/>");

            // Comments clorized
            output = Regex.Replace(output,
                @"^--(?<comment>[^\r\n]*)(?<post>\r\n|$)",
                @"<span style=""color: Olive; font-weight: bold;"">--${comment}</span>${post}",
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );

            // Keywords clorized
            output = Regex.Replace(output,
                @"(?<=(\[|\b))(?<keyword>(SELECT|FROM|WHERE|ORDER|INNER|JOIN|OUTER|LEFT|RIGHT|CROSS" +
                    @"|DISTINCT|DECLARE|SET|EXEC|NOT|IN|IS|NULL|BETWEEN|GROUP|BY|ASC|DESC|OVER|AS|ON" +
                    @"|AND|OR|TOP|GO|CASE|WHEN|ELSE|THEN|IF|BEGIN|END|LIKE))\b",
                @"<span style=""color: #33f; font-weight: bold;"">${keyword}</span>",
                RegexOptions.IgnoreCase
            );

            // Other keywords clorized
            output = Regex.Replace(output,
                @"(\b(?<keyword>ROW_NUMBER|COUNT|CONVERT|COALESCE|CAST)(?<post>\())",
                @"<span style=""color: #3f6; font-weight: bold;"">${keyword}</span>${post}",
                RegexOptions.IgnoreCase
            );

            // Parameters (@xxx)
            output = Regex.Replace(output,
                @"(?<param>\@[\w\d_]+)",
                @"<span style=""color: #993; font-weight: bold;"">${param}</span>",
                RegexOptions.IgnoreCase
            );

            // Numerics
            output = Regex.Replace(output,
                @"(?<arg>(\b|-)[\d.]+\b)",
                @"<span style=""color: #FF3F00;"">${arg}</span>",
                RegexOptions.IgnoreCase
            );

            // Strings, Dates
            output = Regex.Replace(output,
                @"(?<arg>'([^']|'')*')",
                @"<span style=""color: #FF3F00;"">${arg}</span>",
                RegexOptions.IgnoreCase
            );

            return output;
        }

        /// <summary>
        /// Format the SQL command with parameters as variables.
        /// </summary>
        /// <param name="command">Command to format</param>
        /// <returns></returns>
        protected virtual string GetQueryFormattedAsVariables(DatabaseCommand command)
        {
            string commentTags = command.GetTagsAsSqlComments();
            string commandText = command.CommandText.Value;

            // Sort by ParameterName DESC to replace @abcdef before @abc.
            var declarations = new StringBuilder();
            Convertor.DbTypeMap.Initialize(command.GetInternalCommand().Connection);

            foreach (DbParameter param in command.Parameters.Cast<DbParameter>().OrderByDescending(i => i.ParameterName))
            {
                string paramName = param.ParameterName;
                string prefix = Schema.DataParameter.GetPrefixParameter(command.GetInternalCommand());
                string sqlType = Convertor.DbTypeMap.IsStringRepresentation(param.DbType)
                                 ? $"VARCHAR({(param.Size > 0 ? param.Size : 4000)})"
                                 : Convertor.DbTypeMap.FirstMappedType(param.DbType).SqlTypeName;

                if (!paramName.StartsWith(prefix)) paramName = prefix + paramName;

                declarations.AppendLine($"DECLARE {paramName} AS {sqlType.ToUpper()} = {GetValueFormatted(param)}");
            }

            if (declarations.Length > 0)
                declarations.AppendLine();

            return $"{commentTags}{declarations}{commandText}";
        }

        /// <summary>
        /// Returns the parameter value formatted for SQL request (ex. ABC => 'ABC', 12/01/1972 => '1972-01-12', ...) 
        /// </summary>
        /// <param name="parameter">Parameter to format</param>
        /// <returns>Parameter value formatted</returns>
        protected virtual string GetValueFormatted(DbParameter parameter)
        {
            if (parameter.Value == DBNull.Value || parameter.Value == null)
            {
                return "NULL";
            }
            else
            {
                switch (parameter.DbType)
                {
                    // String
                    case System.Data.DbType.AnsiString:
                    case System.Data.DbType.AnsiStringFixedLength:
                    case System.Data.DbType.String:
                    case System.Data.DbType.StringFixedLength:
                    case System.Data.DbType.Xml:
                        return String.Format("'{0}'", Convert.ToString(parameter.Value).Replace("'", "''"));

                    // Boolean
                    case System.Data.DbType.Boolean:
                        return Convert.ToBoolean(parameter.Value) == true ? "1" : "0";

                    // Integer
                    case System.Data.DbType.Byte:
                    case System.Data.DbType.SByte:
                    case System.Data.DbType.Int16:
                    case System.Data.DbType.UInt16:
                    case System.Data.DbType.Int32:
                    case System.Data.DbType.UInt32:
                    case System.Data.DbType.Int64:
                    case System.Data.DbType.UInt64:
                        return Convert.ToString(parameter.Value);

                    // Numeric
                    case System.Data.DbType.Decimal:
                    case System.Data.DbType.Double:
                    case System.Data.DbType.Currency:
                    case System.Data.DbType.Single:
                    case System.Data.DbType.VarNumeric:
                        return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.0#####}", parameter.Value);

                    // Binary
                    case System.Data.DbType.Binary:
                    case System.Data.DbType.Object:
                        return "'BINARY_DATA'";

                    // Date
                    case System.Data.DbType.Date:
                    case System.Data.DbType.DateTime:
                    case System.Data.DbType.DateTime2:
                    case System.Data.DbType.DateTimeOffset:                    

                        DateTime value = Convert.ToDateTime(parameter.Value);

                        if (value.Hour == 0 && value.Minute == 0 && value.Second == 0 && value.Millisecond == 0)
                        {
                            return String.Format("'{0:yyyy-MM-dd}'", value);
                        }
                        else
                        {
                            if (value.Millisecond == 0)
                            {
                                return String.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);
                            }
                            else
                            {
                                return String.Format("'{0:yyyy-MM-dd HH:mm:ss.ffff}'", value);
                            }
                        }

                    // Time
                    case System.Data.DbType.Time:
                        return String.Format("'{0:c}'", parameter.Value);

                    // GUID
                    case System.Data.DbType.Guid:
                        return String.Format("'{0}'", parameter.Value).ToLower();

                    default:
                        return String.Empty;
                }
            }
        }
    }

    /// <summary>
    /// Type of SQL formats
    /// </summary>
    public enum QueryFormat
    {
        /// <summary>
        /// SQL Command Text included parameters values
        /// </summary>
        Text,
        /// <summary>
        /// SQL Command Text formatted in HTML for coloring, ...
        /// </summary>
        Html,
        /// <summary>
        /// SQL Command Text formatted with parameters as variable declarations, ...
        /// </summary>
        Variables
    }
}

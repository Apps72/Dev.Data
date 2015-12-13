using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// Manage the CommantText to display SQL Query including parameters.
    /// </summary>
    public class CommandTextFormatted
    {
        private IDbCommand _command;

        /// <summary>
        /// Initializes a new instance of CommandTextFormatted
        /// </summary>
        /// <param name="command"></param>
        public CommandTextFormatted(IDbCommand command)
        {
            _command = command;
        }

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
                    return this.GetQueryFormattedAsText(_command);

                case QueryFormat.Html:
                    return this.GetQueryFormattedAsHtml(_command);

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns the CommandText with all parameter values included.
        /// </summary>
        /// <param name="command">DBCommand to manage</param>
        /// <returns></returns>
        protected virtual string GetQueryFormattedAsText(IDbCommand command)
        {
            string commandText = command.CommandText;

            // Sort by ParameterName DESC to replace @abcdef before @abc.
            foreach (IDbDataParameter param in command.Parameters.Cast<IDbDataParameter>().OrderByDescending(i => i.ParameterName))
            {
                string paramName = param.ParameterName;
                if (!paramName.StartsWith("@")) paramName = "@" + paramName;

                commandText = Regex.Replace(commandText, paramName, GetValueFormatted(param), RegexOptions.IgnoreCase);
            }

            return commandText;
        }

        /// <summary>
        /// Format the SQL command in HTML (coloring, ...)
        /// </summary>
        /// <param name="command">Command to format in HTML</param>
        /// <returns></returns>
        protected virtual string GetQueryFormattedAsHtml(IDbCommand command)
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

            // Keywords on new line
            //output = Regex.Replace(output,
            //    @"(?<=(\[|\b))(?<keyword>(SELECT|FROM|WHERE|ORDER|INNER|LEFT|RIGHT|CROSS|GROUP|GO|CASE|WHEN|ELSE|IF|BEGIN|END))\b",
            //    @"<br/>${keyword}",
            //    RegexOptions.IgnoreCase
            //);

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
        /// Returns the parameter value formatted for SQL request (ex. ABC => 'ABC', 12/01/1972 => '1972-01-12', ...) 
        /// </summary>
        /// <param name="parameter">Parameter to format</param>
        /// <returns>Parameter value formatted</returns>
        protected virtual string GetValueFormatted(IDbDataParameter parameter)
        {
            if (parameter.Value == DBNull.Value)
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
                    case System.Data.DbType.Time:

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

                    // GUID
                    case System.Data.DbType.Guid:
                        return String.Format("'{{{0}}}'", parameter.Value).ToLower();

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
        Html
    }
}

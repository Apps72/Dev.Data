using System;
using System.Data.Common;
using System.Text;

namespace Apps72.Dev.Data
{
    /// <summary>
    /// SQL Command Text
    /// </summary>
    public class SqlString
    {
        private StringBuilder _commandText = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of SqlString.
        /// </summary>
        /// <param name="value"></param>
        public SqlString(string value)
        {
            _commandText = new StringBuilder(value);
        }

        /// <summary>
        /// Initializes a new instance of SqlString.
        /// </summary>
        /// <param name="value"></param>
        public SqlString(StringBuilder value)
        {
            _commandText = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Value
        {
            get
            {
                return _commandText.ToString();
            }
        }

        /// <summary>
        /// Removes all characters from the current SqlString instance.
        /// </summary>
        public virtual SqlString Clear()
        {
            _commandText.Clear();
            return this;
        }

        /// <summary>
        /// Appends the specified string to this instance.
        /// </summary>
        public virtual SqlString Append(string value)
        {
            _commandText.Append(value);
            return this;
        }

        /// <summary>
        /// Appends the specified string followed by the default line terminator
        /// to the end of the current SqlString instance.
        /// </summary>
        public virtual SqlString AppendLine(string value)
        {
            _commandText.AppendLine(value);
            return this;
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by
        /// the string representation of a corresponding argument in a parameter array.
        /// </summary>
        public virtual SqlString AppendFormat(string format, params object[] args)
        {
            _commandText.AppendFormat(format, args);
            return this;
        }

        /// <summary>
        /// Appends the string followed by the default line terminator 
        /// returned by processing a composite format string, which contains
        /// zero or more format items, to this instance. Each format item is replaced by
        /// the string representation of a corresponding argument in a parameter array.
        /// </summary>
        public virtual SqlString AppendLineFormat(string format, params object[] args)
        {
            if (format == null || args == null || args.Length <= 0)
            {
                _commandText.AppendLine(format);
            }
            else
            {
                _commandText.AppendLine(String.Format(format, args));
            }
            return this;
        }

        /// <summary>
        /// Implicit conversion of a string to a SqlString.
        /// So, you can use `SqlString sql = "SELECT * FROM MyTable"`.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator SqlString(string value)
        {
            return new SqlString(value);
        }

        /// <summary>
        /// Implicit conversion of a string to a SqlString.
        /// So, you can use `SqlString sql = "SELECT * FROM MyTable"`.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator SqlString(StringBuilder value)
        {
            return new SqlString(value);
        }

        /// <summary>
        /// Returns the SQL String content.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _commandText.ToString();
        }

        //internal DbCommand UpdateDbCommand(DbCommand command)
        //{

        //    string sql = _commandText.ToString();

        //    if (String.CompareOrdinal(sql, command.CommandText) != 0)
        //        command.CommandText = sql;

        //    return command;
        //}
    }
}

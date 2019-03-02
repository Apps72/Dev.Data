using System;

namespace Apps72.Dev.Data
{
    /// <summary />
    public partial class DatabaseCommand
    {
        /// <summary>
        /// Returns a Fluent Query tool to execute SQL request.
        /// </summary>
        /// <param name="commandText">Sql query</param>
        [Obsolete("Use .Query().ForSql(commandText) methods.")]
        public FluentQuery Query(SqlString commandText)
        {
            return new FluentQuery(this).ForSql(commandText);
        }

        /// <summary>
        /// Returns a Fluent Query tool to execute SQL request.
        /// </summary>
        /// <param name="commandText">Sql query</param>
        [Obsolete("Use .Query().ForSql(commandText).AddParameter(values) methods.")]
        public FluentQuery Query<T>(SqlString commandText, T values)
        {
            return new FluentQuery(this).ForSql(commandText).AddParameter(values);
        }
    }
}

using Microsoft.Data.Sqlite;
using System;

namespace Apps72.Dev.Data
{
    public static class SqliteDataExtensions
    {
        /// <summary>
        /// Adds a value to the end of the System.Data.SqlClient.SqlParameterCollection.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value to be added. Null value will be replaced by System.DBNull.Value.</param>
        /// <returns>A System.Data.SqlClient.SqlParameter object.</returns>
        public static SqliteParameter AddWithValueOrDBNull(this SqliteParameterCollection parameters, string parameterName, object value)
        {
            if (parameters != null)
            {
                return parameters.AddWithValue(parameterName, value == null ? System.DBNull.Value : value);
            }
            return null;
        }

        /// <summary>
        /// Add all properties / values to the end of the System.Data.SqlClient.SqlParameterCollection.
        /// If a property is already exist in Parameters collection, the parameter is removed and new added with new value.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="values">Object or anonymous object to convert all properties to parameters</param>
        public static void AddValues<T>(this SqliteParameterCollection parameters, T values)
        {
            Internal.DataParameter.AddValues<T, SqliteParameterCollection, SqliteParameter>(parameters, values);
        }
    }
}

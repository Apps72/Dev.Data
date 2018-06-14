using Apps72.Dev.Data.Convertor;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.Schema
{
    /// <summary>
    /// Tools to manage DbParameters 
    /// </summary>
    internal partial class DataParameter
    {
        /// <summary>
        /// Creates a new instance of DbParameter[] with ParameterName, Value and IsNullable properties 
        /// sets to value's properties.
        /// </summary>
        /// <typeparam name="T">Type of object with properties to convert in Parameters</typeparam>
        /// <typeparam name="V">DbParameter type (SqlParameter, ...)</typeparam>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        public static void AddValues<T, V>(DbParameterCollection parameters, T values) where V : DbParameter, new()
        {
            IEnumerable<DbParameter> properties = ToParameters<T, V>(null, values);
            AddOrRemplaceParameters(parameters, properties);
        }

        /// <summary>
        /// Creates a new instance of DbParameter[] with ParameterName, Value and IsNullable properties 
        /// sets to value's properties.
        /// </summary>
        /// <typeparam name="T">Type of object with properties to convert in Parameters</typeparam>
        /// <param name="command"></param>
        /// <param name="values"></param>
        public static void AddValues<T>(DbCommand command, T values)
        {
            IEnumerable<DbParameter> properties = ToParameters<T, DbParameter>(command, values);
            AddOrRemplaceParameters(command.Parameters, properties);
        }

        /// <summary>
        /// Creates a new instance of DbParameter[] with ParameterName, Value and IsNullable properties 
        /// sets to value's properties.
        /// </summary>
        /// <typeparam name="T">Type of object with properties to convert in Parameters</typeparam>
        /// <typeparam name="U">DbParameter type (SqlParameter, ...)</typeparam>
        /// <param name="command"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IEnumerable<DbParameter> ToParameters<T, U>(DbCommand command, T value) where U : DbParameter
        {
            if (TypeExtension.IsPrimitive(typeof(T)))
            {
                throw new ArgumentException("The value can not be a simple type (string, int, ...), but an object with simple properties.", "value");
            }
            else
            {
                List<DbParameter> parameters = new List<DbParameter>();
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    if (TypeExtension.IsPrimitive(property.PropertyType))
                    {
                        // Data type
                        Type propType = TypeExtension.GetNullableSubType(property.PropertyType);

                        // Value
                        DbParameter parameter = command != null ? 
                                                command.CreateParameter() : 
                                                Activator.CreateInstance(typeof(U)) as DbParameter;
                        parameter.Value = typeof(T).GetProperty(property.Name).GetValue(value, null);
                        parameter.IsNullable = TypeExtension.IsNullable(propType);
                        parameter.DbType = DbTypeMap.FirstDbType(propType);

                        // Parameter name
                        string attribute = Apps72.Dev.Data.Annotations.ColumnAttribute.GetColumnAttributeName(property);
                        if (string.IsNullOrEmpty(attribute))
                        {
                            parameter.ParameterName = property.Name;
                        }
                        else
                        {
                            parameter.ParameterName = attribute;
                        }

                        parameters.Add(parameter);
                    }
                }

                return parameters.AsEnumerable();
            }

        }

        private static void AddOrRemplaceParameters(DbParameterCollection existingParameters, IEnumerable<DbParameter> newParameters)
        {
            string prefix = DataParameter.GetPrefixParameter(existingParameters);

            // Remove existing parameters found in Values properties
            for (int i = existingParameters.Count - 1; i >= 0; i--)
            {
                string parameterName = existingParameters[i].ParameterName;
                if (newParameters.Any(p => String.Compare(p.ParameterName, parameterName, true) == 0 ||
                                           String.Compare($"{prefix}{p.ParameterName}", parameterName, true) == 0))
                {
                    existingParameters.RemoveAt(i);
                }
            }

            // Add parameters found in Values properties
            foreach (var param in newParameters)
            {
                if (!param.ParameterName.EndsWith(prefix))
                    param.ParameterName = $"{prefix}{param.ParameterName}";

                existingParameters.Add(param);
            }
        }

#if SQL_CLR
        internal static string GetPrefixParameter(DbParameterCollection parameters) { return "@"; }
        internal static string GetPrefixParameter(DbCommand command) { return "@"; }
#else
        private static string _prefixParameter = String.Empty;

        internal static string GetPrefixParameter(DbParameterCollection parameters)
        {
            if (_prefixParameter == String.Empty)
            {
                switch (parameters.GetType().Name)
                {
                    case "OracleParameterCollection":
                        _prefixParameter = ":";
                        break;
                    default:
                        _prefixParameter = "@";
                        break;
                }
            }

            return _prefixParameter;
        }

        internal static string GetPrefixParameter(DbCommand command)
        {
            if (_prefixParameter == String.Empty)
            {
                switch (command.GetType().Name)
                {
                    case "OracleCommand":
                        _prefixParameter = ":";
                        break;
                    default:
                        _prefixParameter = "@";
                        break;
                }
            }

            return _prefixParameter;
        }
#endif

    }
}
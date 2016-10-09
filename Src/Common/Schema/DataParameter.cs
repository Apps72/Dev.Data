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
        private static string _prefixParameter = String.Empty;

        /// <summary>
        /// Creates a new instance of DbParameter[] with ParameterName, Value and IsNullable properties 
        /// sets to value's properties.
        /// </summary>
        /// <typeparam name="T">Type of object with properties to convert in Parameters</typeparam>
        /// <typeparam name="U">DbParameterCollection where to insert or replace values</typeparam>
        /// <typeparam name="V">DbParameter type (SqlParameter, ...)</typeparam>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        public static void AddValues<T, U, V>(U parameters, T values) where U : DbParameterCollection where V : DbParameter, new()
        {
            IEnumerable<V> properties = ToParameters<T, V>(values);
            AddOrRemplaceParameters(parameters, properties);
        }

        /// <summary>
        /// Creates a new instance of DbParameter[] with ParameterName, Value and IsNullable properties 
        /// sets to value's properties.
        /// </summary>
        /// <typeparam name="T">Type of object with properties to convert in Parameters</typeparam>
        /// <typeparam name="U">DbParameter type (SqlParameter, ...)</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IEnumerable<U> ToParameters<T, U>(T value) where U : DbParameter, new()
        {
            if (TypeExtension.IsPrimitive(typeof(T)))
            {
                throw new ArgumentException("The value can not be a simple type (string, int, ...), but an object with simple properties.", "value");
            }
            else
            {
                List<U> parameters = new List<U>();
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    if (TypeExtension.IsPrimitive(property.PropertyType))
                    {
                        // Data type
                        Type propType = TypeExtension.GetNullableSubType(property.PropertyType);

                        // Value
                        U parameter = Activator.CreateInstance(typeof(U)) as U;
                        parameter.Value = typeof(T).GetProperty(property.Name).GetValue(value, null);
                        parameter.IsNullable = TypeExtension.IsNullable(propType);
                        parameter.DbType = DataTypedConvertor.ToDbType(propType);

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

        private static void AddOrRemplaceParameters<U>(DbParameterCollection existingParameters, IEnumerable<U> newParameters) where U : DbParameter, new()
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
    }
}
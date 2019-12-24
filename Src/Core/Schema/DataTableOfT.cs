using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apps72.Dev.Data.Schema
{
    public class DataTable<T>
    {
        public DataTable(DbDataReader reader)
        {
            //this.Columns = Enumerable.Range(0, reader.FieldCount)
            //                         .Select(i => new DataColumn()
            //                         {
            //                             ColumnName = reader.GetName(i),
            //                             DataType = reader.GetFieldType(i),
            //                             Ordinal = i,
            //                             IsNullable = reader.IsDBNull(i)
            //                         });

            this.Rows = GetRows(reader);
        }

        public IEnumerable<DataColumn> Columns { get; set; }

        public IEnumerable<T> Rows { get; set; }

        private IEnumerable<T> GetRows(DbDataReader reader)
        {
            var result = new List<T>();

            // Properties to fill
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var names = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)).ToArray();
            var propToFill = new List<PropertyInfo>();
            foreach (var name in names)
            {
                propToFill.Add(properties.FirstOrDefault(x => String.Compare(x.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0));
            }

            while (reader.Read())
            {
                var newItem = Activator.CreateInstance<T>();
                int i = 0;
                foreach (var prop in propToFill)
                {
                    object value = reader.GetValue(i);
                    prop.SetValue(newItem, value == DBNull.Value ? null : value, null);
                    i++;
                }
            }

            return result;
        }

        private T GetConvertedRow<T>(IDataRecord record)
        {
            var newItem = Activator.CreateInstance<T>();
            var allProperties = newItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < record.FieldCount; i++)
            {
                string fieldName = record.GetName(i);
                var property = allProperties.FirstOrDefault(x => String.Compare(x.Name, fieldName, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (property != null)
                {
                    object value = record.GetValue(i);
                    property.SetValue(newItem, value == DBNull.Value ? null : value, null);
                }
            }

            return newItem;
        }

        //private T GetValue(IDataRecord record, int index)
        //{
        //    TypeCode typeCode = Type.GetTypeCode(typeof(T));

        //    switch (typeCode)
        //    {
        //        case TypeCode.Empty:
        //            break;
        //        case TypeCode.Object:
        //            break;
        //        case TypeCode.DBNull:
        //            break;
        //        case TypeCode.Boolean:
        //            break;
        //        case TypeCode.Char:
        //            break;
        //        case TypeCode.SByte:
        //            break;
        //        case TypeCode.Byte:
        //            break;
        //        case TypeCode.Int16:
        //            break;
        //        case TypeCode.UInt16:
        //            break;
        //        case TypeCode.Int32:
        //            break;
        //        case TypeCode.UInt32:
        //            break;
        //        case TypeCode.Int64:
        //            break;
        //        case TypeCode.UInt64:
        //            break;
        //        case TypeCode.Single:
        //            break;
        //        case TypeCode.Double:
        //            break;
        //        case TypeCode.Decimal:
        //            break;
        //        case TypeCode.DateTime:
        //            break;
        //        case TypeCode.String:
        //            return record.GetString(index);
        //        default:
        //            break;
        //    }
        //}
    }
}

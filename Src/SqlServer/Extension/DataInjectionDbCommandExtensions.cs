using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps72.Dev.Data
{
    /// <summary />
    public static class DataInjectionDbCommandExtensions
    {
        /// <summary>
        /// Returns a DataTable with all data and columns associated to properties
        /// </summary>
        /// <returns></returns>
        internal static System.Data.DataTable GetSystemDataTable(this DataInjectionDbCommand dbComand)
        {
            return dbComand.GetDataTable().ConvertToSystemDataTable();
        }
    }
}

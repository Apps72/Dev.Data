using Apps72.Dev.Data;
using Apps72.Dev.Data.Oracle;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.SqlClient;

namespace Data.Oracle.Tests
{
    public class EMPBase
    {
        public DateTime HireDate { get; set; }
        public string EName { get; set; }
        public Int16 EmpNo { get; set; }
        public float? Comm { get; set; }
    }

    public class EMP : EMPBase
    {
        public int? SAL { get; set; }       // Not used, because there are a Salary property tagged [Column("SAL")]
        [Apps72.Dev.Data.Annotations.Column("sal")]
        public float Salary { get; set; }
        [Apps72.Dev.Data.Annotations.Column("MGR")]
        public short Manager { get; set; }
        public int? MGR { get; set; }       // Not used, because there are a Manager property tagged [Column("MGR")]
        public string ColumnNotUse { get; set; }

        public static EMP Smith
        {
            get
            {
                return new EMP() { EmpNo = 7369, EName = "SMITH", HireDate = new DateTime(1980, 12, 17), Comm = null, Salary = 800 };
            }
        }

        public static decimal GetEmployeesCount(OracleConnection currentConnection)
        {
            return GetEmployeesCount(currentConnection, null);
        }

        public static decimal GetEmployeesCount(OracleConnection currentConnection, OracleTransaction currentTransaction)
        {
            using (OracleDatabaseCommand cmd = new OracleDatabaseCommand(currentConnection, currentTransaction, string.Empty))
            {
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                return cmd.ExecuteScalar<decimal>();
            }
        }
    }

    public partial class DEPT
    {
        public virtual int DeptNo { get; set; }
        public virtual string DName { get; set; }
        public virtual string Loc { get; set; }

        public static DEPT Accounting
        {
            get
            {
                return new DEPT() { DeptNo = 10, DName = "ACCOUNTING", Loc = "NEW YORK" };
            }
        }
    }
}

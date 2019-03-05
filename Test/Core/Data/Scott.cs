using Apps72.Dev.Data;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Data.Core.Tests
{
    public class EMPBase
    {
        public DateTime HireDate { get; set; }
        public string EName { get; set; }
        public int EmpNo { get; set; }
        public int? Comm { get; set; }
    }

    public class EMP : EMPBase
    {
        public int? SAL { get; set; }       // Not used, because there are a Salary property tagged [Column("SAL")]

        [Apps72.Dev.Data.Annotations.Column("sal")]
        public decimal? Salary { get; set; }

        [Apps72.Dev.Data.Annotations.Column("MGR")]
        public int? Manager { get; set; }

        public int? MGR { get; set; }       // Not used, because there are a Manager property tagged [Column("MGR")]

        public string ColumnNotUse { get; set; }

        public static EMP Smith
        {
            get
            {
                return new EMP() { EmpNo = 7369, EName = "SMITH", HireDate = new DateTime(1980, 12, 17), Comm = null, Salary = 800, Manager = 7902 };
            }
        }

        public static int GetEmployeesCount(DbConnection currentConnection)
        {
            using (var cmd = new DatabaseCommand(currentConnection))
            {
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                return cmd.ExecuteScalar<int>();
            }
        }

        [Obsolete]
        public static int GetEmployeesCount(DbConnection currentConnection, DbTransaction transaction)
        {
            using (var cmd = new DatabaseCommand(currentConnection, transaction))
            {
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                return cmd.ExecuteScalar<int>();
            }
        }

        public static int GetEmployeesCount(DbTransaction currentTransaction)
        {
            using (var cmd = new DatabaseCommand(currentTransaction))
            {
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                return cmd.ExecuteScalar<int>();
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

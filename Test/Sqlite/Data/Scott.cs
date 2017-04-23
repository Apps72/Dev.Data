using Apps72.Dev.Data.Sqlite;
using System;
using System.Data.SQLite;

namespace Data.Sqlite.Tests
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
        public decimal Salary { get; set; }
        [Apps72.Dev.Data.Annotations.Column("MGR")]
        public int? Manager { get; set; }
        public int? MGR { get; set; }       // Not used, because there are a Manager property tagged [Column("MGR")]
        public string ColumnNotUse { get; set; }

        public static EMP Smith
        {
            get
            {
                return new EMP() { EmpNo = 7369, EName = "SMITH", HireDate = new DateTime(1980, 12, 17), Comm = null, Salary = 800 };
            }
        }

        public static long GetEmployeesCount(SQLiteConnection currentConnection)
        {
            return GetEmployeesCount(currentConnection, null);
        }

        public static long GetEmployeesCount(SQLiteConnection currentConnection, SQLiteTransaction currentTransaction)
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(currentConnection, currentTransaction, string.Empty))
            {
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                return cmd.ExecuteScalar<long>();
            }
        }
    }

    public partial class DEPT
    {
        public virtual long DeptNo { get; set; }
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

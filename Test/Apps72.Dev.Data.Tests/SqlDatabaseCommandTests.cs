using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using Apps72.Dev.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Tests
{
    [TestClass]
    public class SqlDatabaseCommandTests
    {
        #region INITIALIZATION

        public const string CONNECTION_STRING = @"Server=(localdb)\ProjectsV12;Database=Scott;Integrated Security=true;";
        private SqlConnection _connection;

        [TestInitialize]
        public void Initialization()
        {
            _connection = new SqlConnection(CONNECTION_STRING);
            _connection.Open();
        }

        #endregion

        #region TEMPORARY CONNECTION

        [TestMethod]
        public void OpenTemporaryConnection_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(CONNECTION_STRING, string.Empty, -1))
            {
                cmd.Log = (message) =>
                {
                    Console.WriteLine(message);
                };

                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
                DataTable data = cmd.ExecuteTable();

                Assert.AreEqual(data.Rows.Count, 14);
                Assert.AreEqual(data.Columns.Count, 8);
            }
        }

        #endregion

        #region EXECUTE METHODS

        [TestMethod]
        public void ExecuteTable_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
                DataTable data = cmd.ExecuteTable();

                Assert.AreEqual(data.Rows.Count, 14);
                Assert.AreEqual(data.Columns.Count, 8);
            }
        }

        [TestMethod]
        public void ExecuteRow_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
                DataRow row = cmd.ExecuteRow();

                Assert.AreEqual(row.ItemArray.Length, 8);
            }
        }

        [TestMethod]
        public void ExecuteRowTyped_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP WHERE EMPNO = 7369");
                EMP emp = cmd.ExecuteRow<EMP>();

                Assert.AreEqual(emp.EmpNo, 7369);
            }
        }

        [TestMethod]
        public void ExecuteRowPrimitive_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO FROM EMP WHERE EMPNO = 7369");
                int empno = cmd.ExecuteRow<int>();

                Assert.AreEqual(empno, 7369);
            }
        }

        [TestMethod]
        public void ExecuteRowPrimitiveNullable_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COMM FROM EMP WHERE EMPNO = 7369");
                int? comm = cmd.ExecuteRow<int?>();

                Assert.AreEqual(comm, null);
            }
        }

        [TestMethod]
        public void ExecuteRowWithDelegate_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
                EMP emp = cmd.ExecuteRow<EMP>((row) => new EMP() { EmpNo = Convert.ToInt32(row["EMPNO"]) });

                Assert.AreEqual(emp.EmpNo, 7369);
            }
        }

        [TestMethod]
        public void ExecuteScalar_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                object data = cmd.ExecuteScalar();

                Assert.AreEqual(data, 14);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithParameter_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ")
                               .AppendLine("   AND HIREDATE = @HireDate ")
                               .AppendLine("   AND JOB = @Job ");

                cmd.Parameters.AddWithValue("@EMPNO", 7369);                            // Parameter in Upper Case
                cmd.Parameters.AddWithValue("HireDate", new DateTime(1980, 12, 17));    // Parameter without @
                cmd.Parameters.AddWithValue("@Job", "CLERK");                           // Parameter in normal mode

                object data = cmd.ExecuteScalar();

                Assert.AreEqual(data, "SMITH");
            }
        }

        [TestMethod]
        public void ExecuteScalarWithAnonymousParameters_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ")
                               .AppendLine("   AND HIREDATE = @HireDate ")
                               .AppendLine("   AND JOB = @Job ")
                               .AppendLine("   AND 1 = @NotDeleted ");

                cmd.Parameters.AddWithValue("@EMPNO", 1234);                            // Parameter in Upper Case
                cmd.Parameters.AddWithValue("HireDate", new DateTime(1980, 1, 1));      // Parameter without @
                cmd.Parameters.AddWithValue("@Job", "FAKE");                            // Parameter in normal mode
                cmd.Parameters.AddWithValue("@NotDeleted", true);                       // Parameter not replaced by .AddValues

                // Replace previous values wiht these new propery values
                cmd.Parameters.AddValues(new
                {
                    EmpNo = 7369,
                    HireDate = new DateTime(1980, 12, 17),
                    Job = "CLERK"
                });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual(data, "SMITH");
            }
        }

        [TestMethod]
        public void ExecuteScalarWithAnonymousOnlyParameters_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ")
                               .AppendLine("   AND HIREDATE = @HireDate ")
                               .AppendLine("   AND JOB = @Job ");

                // Replace previous values wiht these new propery values
                cmd.Parameters.AddValues(new
                {
                    EmpNo = 7369,
                    HireDate = new DateTime(1980, 12, 17),
                    Job = "CLERK"
                });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual(data, "SMITH");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteScalarWithSimpleParameters_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ");


                // Simple value are not autorized
                cmd.Parameters.AddValues(123);

                object data = cmd.ExecuteScalar();

                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecuteScalarTyped_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                int data = cmd.ExecuteScalar<int>();

                Assert.AreEqual(data, 14);
            }
        }

        [TestMethod]
        public void ExecuteScalarTypedNull_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLineFormat(" SELECT COMM FROM EMP WHERE EMPNO = {0} ", 7369);
                int? data = cmd.ExecuteScalar<int?>();

                Assert.AreEqual(data, null);
            }
        }

        [TestMethod]
        public void ExecuteTableTyped_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP ");
                EMP[] data = cmd.ExecuteTable<EMP>().ToArray();
                EMP smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(smith.EmpNo, EMP.Smith.EmpNo);
                Assert.AreEqual(smith.EName, EMP.Smith.EName);
                Assert.AreEqual(smith.HireDate, EMP.Smith.HireDate);
                Assert.AreEqual(smith.Comm, EMP.Smith.Comm);
            }
        }

        [TestMethod]
        public void ExecuteTableCustomedTyped_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP ");
                var data = cmd.ExecuteTable<EMP>((row) =>
                {
                    return new EMP()
                    {
                        EmpNo = row.Field<int>("EMPNO"),
                        EName = row.Field<string>("ENAME"),
                        HireDate = row.Field<DateTime>("HIREDATE"),
                        Comm = row.Field<int?>("COMM")
                    };
                });
                EMP smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(smith.EmpNo, EMP.Smith.EmpNo);
                Assert.AreEqual(smith.EName, EMP.Smith.EName);
                Assert.AreEqual(smith.HireDate, EMP.Smith.HireDate);
                Assert.AreEqual(smith.Comm, EMP.Smith.Comm);
            }
        }

        [TestMethod]
        public void ExecuteTableTypedWithColumnAttribute_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, SAL, HIREDATE, COMM, MGR FROM EMP ");
                var data = cmd.ExecuteTable<EMP>();
                EMP smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(smith.EmpNo, EMP.Smith.EmpNo);
                Assert.AreEqual(smith.EName, EMP.Smith.EName);
                Assert.AreEqual(smith.HireDate, EMP.Smith.HireDate);
                Assert.AreEqual(smith.Comm, EMP.Smith.Comm);
                Assert.AreEqual(smith.Salary, EMP.Smith.Salary);
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_Transaction_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                cmd.TransactionBegin();
                cmd.ExecuteNonQuery();
                cmd.TransactionRollback();

                Assert.AreEqual(this.GetEmployeesCount(), 14);
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoCommands_Test()
        {
            SqlTransaction currentTransaction;

            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                currentTransaction = cmd.TransactionBegin();
                cmd.ExecuteNonQuery();

                Assert.AreEqual(this.GetEmployeesCount(currentTransaction), 0);     // Inside the transaction

                cmd.TransactionRollback();

                Assert.AreEqual(this.GetEmployeesCount(currentTransaction), 14);    // Inside the transaction
                Assert.AreEqual(this.GetEmployeesCount(), 14);                      // Ouside the transaction
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoIncludedCommands_Test()
        {
            using (SqlDatabaseCommand cmd1 = new SqlDatabaseCommand(_connection))
            {
                cmd1.Log = Console.WriteLine;
                cmd1.CommandText.AppendLine(" DELETE FROM EMP ");
                cmd1.TransactionBegin();
                cmd1.ExecuteNonQuery();

                using (SqlDatabaseCommand cmd2 = new SqlDatabaseCommand(_connection, cmd1.Transaction))
                {
                    cmd2.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    int count = cmd2.ExecuteScalar<int>();
                }

                cmd1.TransactionRollback();
            }
        }

        #endregion

        #region DEADLOCKS

        //[TestMethod()]
        //public void RaiseDeadLock_Test()
        //{
        //    SqlException ex = this.RaiseSqlDeadLock(false);

        //    Assert.IsNotNull(ex);
        //    Assert.AreEqual(ex.Number, 1205);
        //}

        //[TestMethod()]
        //public void RetryWhenDeadLockOccured_Test()
        //{
        //    SqlException ex = this.RaiseSqlDeadLock(true);

        //    Assert.IsNull(ex);
        //}

        //private SqlException RaiseSqlDeadLock(bool withRetry)
        //{
        //    // See: http://stackoverflow.com/questions/22825147/how-to-simulate-deadlock-on-sql-server

        //    SqlConnection connection2 = new SqlConnection(CONNECTION_STRING);
        //    connection2.Open();
        //    SqlException exToReturn = null;

        //    try
        //    {
        //        using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
        //        {
        //            cmd.Log = Console.WriteLine;

        //            cmd.CommandText.AppendLine(" CREATE TABLE ##Employees ( ");
        //            cmd.CommandText.AppendLine("     EmpId INT IDENTITY, ");
        //            cmd.CommandText.AppendLine("     EmpName VARCHAR(16), ");
        //            cmd.CommandText.AppendLine("     Phone VARCHAR(16) ");
        //            cmd.CommandText.AppendLine(" ) ");

        //            cmd.CommandText.AppendLine(" INSERT INTO ##Employees (EmpName, Phone) ");
        //            cmd.CommandText.AppendLine(" VALUES('Martha', '800-555-1212'), ('Jimmy', '619-555-8080') ");

        //            cmd.CommandText.AppendLine(" CREATE TABLE ##Suppliers( ");
        //            cmd.CommandText.AppendLine("     SupplierId INT IDENTITY, ");
        //            cmd.CommandText.AppendLine("     SupplierName VARCHAR(64), ");
        //            cmd.CommandText.AppendLine("     Fax VARCHAR(16) ");
        //            cmd.CommandText.AppendLine(" ) ");

        //            cmd.CommandText.AppendLine(" INSERT INTO ##Suppliers (SupplierName, Fax) ");
        //            cmd.CommandText.AppendLine(" VALUES ('Acme', '877-555-6060'), ('Rockwell', '800-257-1234') ");

        //            cmd.ExecuteNonQuery();

        //        }

        //        using (SqlDatabaseCommand cmd1 = new SqlDatabaseCommand(_connection))
        //        {
        //            using (SqlDatabaseCommand cmd2 = new SqlDatabaseCommand(connection2))
        //            {
        //                cmd1.Log = Console.WriteLine;
        //                cmd2.Log = Console.WriteLine;

        //                cmd1.TransactionBegin();
        //                cmd2.TransactionBegin();

        //                cmd1.Clear();
        //                cmd1.CommandText.AppendLine(" UPDATE ##Employees SET EmpName = 'Mary'    WHERE empid = 1 ");
        //                cmd1.ExecuteNonQuery();

        //                cmd2.Clear();
        //                cmd2.CommandText.AppendLine(" UPDATE ##Suppliers SET Fax = N'555-1212'   WHERE supplierid = 1 ");
        //                cmd2.ExecuteNonQuery();

        //                // Start and when cmd2.ExecuteNonQuery command will be executed, an DeadLock exception will be raised.
        //                Task task1 = Task.Factory.StartNew(() =>
        //                {
        //                    cmd1.Clear();
        //                    cmd1.ThrowException = false;
        //                    if (withRetry)
        //                    {
        //                        cmd1.RetryIfExceptionsOccured.SetDeadLockCodes();
        //                    }
        //                    cmd1.CommandText.AppendLine(" UPDATE ##Suppliers SET Fax = N'555-1212'   WHERE supplierid = 1 ");
        //                    cmd1.ExecuteNonQuery();
        //                });

        //                System.Threading.Thread.Sleep(1000);

        //                cmd2.Clear();
        //                cmd2.CommandText.AppendLine(" UPDATE ##Employees SET phone = N'555-9999' WHERE empid = 1 ");
        //                cmd2.ExecuteNonQuery();

        //                cmd2.Dispose();
        //                connection2.Close();

        //                // Wait cmd1 finished (and raised an Exception)
        //                task1.Wait();

        //                exToReturn = cmd1.Exception;
        //            }
        //        }

        //        using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
        //        {
        //            cmd.Log = Console.WriteLine;

        //            cmd.CommandText.AppendLine(" DROP TABLE ##Employees ");
        //            cmd.CommandText.AppendLine(" DROP TABLE ##Suppliers ");
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    finally
        //    {
        //        connection2.Close();
        //        connection2.Dispose();
        //        connection2 = null;
        //    }

        //    return exToReturn;
        //}

        #endregion

        #region QUERY FORMATTED

        [TestMethod]
        public void GetFormattedAsText_Simple_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.CommandText.Append(" SELECT * FROM EMP ");
                string formatted = cmd.GetCommandTextFormatted(QueryFormat.Text);

                Assert.AreEqual(formatted, " SELECT * FROM EMP ");
            }
        }

        [TestMethod]
        public void GetFormattedAsText_Parameters_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.CommandText.Append(" SELECT * FROM EMP WHERE EMPNO = @EmpNo AND ENAME LIKE @Ename AND HIREDATE > @Hire AND COMM = @Comm ");
                cmd.Parameters.AddWithValue("@EmpNo", 7369);                                    // Parameter normal
                cmd.Parameters.AddWithValue("@ENAME", "%SM%");                                  // Parameter in Upper Case
                cmd.Parameters.AddWithValue("Hire", new DateTime(1970, 05, 04, 14, 15, 16));    // Parameter without @
                cmd.Parameters.AddWithValueOrDBNull("@Comm", null);                             // Parameter NULL

                string formatted = cmd.GetCommandTextFormatted(QueryFormat.Text);

                Assert.AreEqual(formatted, " SELECT * FROM EMP WHERE EMPNO = 7369 AND ENAME LIKE '%SM%' AND HIREDATE > '1970-05-04 14:15:16' AND COMM = NULL ");
            }
        }

        [TestMethod]
        public void GetFormattedAsHtml_Parameters_Test()
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection))
            {
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
                cmd.CommandText.AppendLine("  WHERE EMPNO = @EmpNo AND ENAME LIKE @Ename AND HIREDATE > @Hire AND COMM = @Comm ");
                cmd.Parameters.AddWithValue("@EmpNo", 7369);                                    // Parameter normal
                cmd.Parameters.AddWithValue("@ENAME", "%SM%");                                  // Parameter in Upper Case
                cmd.Parameters.AddWithValue("Hire", new DateTime(1970, 05, 04, 14, 15, 16));    // Parameter without @
                cmd.Parameters.AddWithValueOrDBNull("@Comm", null);                             // Parameter NULL

                string formatted = cmd.GetCommandTextFormatted(QueryFormat.Html);

                Assert.AreEqual(formatted, @" <span style=""color: #33f; font-weight: bold;"">SELECT</span> * <span style=""color: #33f; font-weight: bold;"">FROM</span> EMP <br/>  <span style=""color: #33f; font-weight: bold;"">WHERE</span> EMPNO = <span style=""color: #FF3F00;"">7369</span> <span style=""color: #33f; font-weight: bold;"">AND</span> ENAME <span style=""color: #33f; font-weight: bold;"">LIKE</span> <span style=""color: #FF3F00;"">'%SM%'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> HIREDATE &gt; <span style=""color: #FF3F00;"">'<span style=""color: #FF3F00;"">1970</span><span style=""color: #FF3F00;"">-05</span><span style=""color: #FF3F00;"">-04</span> <span style=""color: #FF3F00;"">14</span>:<span style=""color: #FF3F00;"">15</span>:<span style=""color: #FF3F00;"">16</span>'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> COMM = <span style=""color: #33f; font-weight: bold;"">NULL</span> <br/>");
            }
        }

        #endregion

        #region PRIVATES

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetEmployeesCount()
        {
            return this.GetEmployeesCount(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentTransaction"></param>
        /// <returns></returns>
        private int GetEmployeesCount(SqlTransaction currentTransaction)
        {
            using (SqlDatabaseCommand cmd = new SqlDatabaseCommand(_connection, currentTransaction, string.Empty))
            {
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                return cmd.ExecuteScalar<int>();
            }
        }

        #endregion
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Common;

namespace Data.Core.Tests
{
    [TestClass]
    public class DatabaseCommandTests
    {

        [TestMethod]
        public void ExecuteScalar_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                object data = cmd.ExecuteScalar();

                Assert.AreEqual(14, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithParameter_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ")
                               .AppendLine("   AND HIREDATE = @HireDate ")
                               .AppendLine("   AND JOB = @Job ");

                cmd.AddParameter("@EMPNO", 7369);                            // Parameter in Upper Case
                cmd.AddParameter("HireDate", new DateTime(1980, 12, 17));    // Parameter without @
                cmd.AddParameter("@Job", "CLERK");                           // Parameter in normal mode

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithNullParameter_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine("SET ANSI_NULLS OFF ")
                               .AppendLine(" SELECT COUNT(*) ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE COMM = @Comm ")
                               .AppendLine("SET ANSI_NULLS ON ");

                cmd.AddParameter("@Comm", null);

                int count = cmd.ExecuteScalar<int>();

                Assert.AreEqual(10, count);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithAnonymousParameters_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ")
                               .AppendLine("   AND HIREDATE = @HireDate ")
                               .AppendLine("   AND JOB = @Job ")
                               .AppendLine("   AND 1 = @NotDeleted ");

                cmd.AddParameter("@EMPNO", 1234);                            // Parameter in Upper Case
                cmd.AddParameter("HireDate", new DateTime(1980, 1, 1));      // Parameter without @
                cmd.AddParameter("@Job", "FAKE");                            // Parameter in normal mode
                cmd.AddParameter("@NotDeleted", true);                       // Parameter not replaced 

                // Replace previous values wiht these new propery values
                cmd.AddParameter(new
                {
                    EmpNo = 7369,
                    HireDate = new DateTime(1980, 12, 17),
                    Job = "CLERK"
                });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithAnonymousOnlyParameters_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ")
                               .AppendLine("   AND HIREDATE = @HireDate ")
                               .AppendLine("   AND JOB = @Job ");

                // Replace previous values wiht these new propery values
                cmd.AddParameter(new
                {
                    EmpNo = 7369,
                    HireDate = new DateTime(1980, 12, 17),
                    Job = "CLERK"
                });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteScalarWithSimpleParameters_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ");


                // Simple value are not autorized
                cmd.AddParameter(123);

                object data = cmd.ExecuteScalar();

                Assert.Fail();
            }
        }

        [TestMethod]
        public void ExecuteScalarWithObjectParameter_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ");

                // Add manual parameter
                cmd.AddParameter(new { EmpNo = 7369 });

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithDbParameter_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ");

                // Add manual parameter
                cmd.Parameters.Add(new SqlParameter("@EmpNo", 7369));

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithParameterTyped_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ");

                // Add manual parameter
                cmd.AddParameter("@EmpNo", 7369, System.Data.DbType.Int32, 4);

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarTyped_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLineFormat(" SELECT COUNT(*) FROM EMP ");
                int data = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarTypedNull_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLineFormat(" SELECT COMM FROM EMP WHERE EMPNO = {0} ", 7369);
                int? data = cmd.ExecuteScalar<int?>();

                Assert.AreEqual(null, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWhereNoDataFound_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLineFormat(" SELECT COMM FROM EMP WHERE EMPNO = 99999 ");
                int? data = cmd.ExecuteScalar<int?>();

                Assert.AreEqual(null, data);
            }
        }

        [TestMethod]
        public void ExecuteTableTyped_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP ");
                EMP[] data = cmd.ExecuteTable<EMP>().ToArray();
                EMP smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
                Assert.AreEqual(EMP.Smith.Comm, smith.Comm);
            }
        }

        [TestMethod]
        public void ExecuteTableNullableProperties_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP ");
                var data = cmd.ExecuteTable(new
                {
                    EmpNo = int.MinValue,
                    EName = String.Empty,
                    HireDate = new Nullable<DateTime>(),
                    Comm = (int?)null,
                    Mgr = (int?)4
                });

                var smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
                Assert.AreEqual(EMP.Smith.Comm, smith.Comm);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException), "Properties of your anonymous class must be in the same type and same order of your SQL Query.")]
        public void ExecuteTableCustomedAnonymousTyped_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE FROM EMP ");
                var data = cmd.ExecuteTable(new
                {
                    EmpNo = 0,
                    EName = String.Empty,
                    HireDate = DateTime.Today,
                    MyVar = ""
                });
                var smith = data.FirstOrDefault(i => i.EmpNo == 7369);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
            }
        }

        [TestMethod]
        public void ExecuteTableTypedWithColumnAttribute_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
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
        public void ExecuteTableWithAnonymousConverter_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, SAL, HIREDATE, COMM, MGR  FROM EMP");
                var employees = cmd.ExecuteTable((row) =>
                {
                    return new
                    {
                        Id = row.Field<int>("EMPNO"),
                        Name = row.Field<string>("ENAME"),
                        Salary = row.Field<Decimal>("SAL"),
                        HireDate = row.Field<DateTime>("HIREDATE"),
                        Comm = row.Field<int?>("COMM"),
                        Manager = row.Field<int?>("MGR"),
                    };
                });

                var smith = employees.First();

                Assert.AreEqual(14, employees.Count());
                Assert.AreEqual(EMP.Smith.EmpNo, smith.Id);
                Assert.AreEqual(EMP.Smith.Salary, smith.Salary);
                Assert.AreEqual(EMP.Smith.HireDate, smith.HireDate);
                Assert.AreEqual(EMP.Smith.Comm, smith.Comm);
                Assert.AreEqual(EMP.Smith.Manager, smith.Manager);

            }
        }

        [TestMethod]
        public void ExecuteDataSetTyped_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT ");

                var data = cmd.ExecuteDataSet<EMP, DEPT>();
                var smith = data.Item1.FirstOrDefault(i => i.EmpNo == 7369);
                var accounting = data.Item2.FirstOrDefault(i => i.DeptNo == 10);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(DEPT.Accounting.DName, accounting.DName);
                Assert.AreEqual(DEPT.Accounting.Loc, accounting.Loc);
            }
        }

        [TestMethod]
        public void ExecuteDataSetCustomedTyped_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");
                cmd.CommandText.AppendLine(" SELECT DEPTNO, DNAME FROM DEPT ");

                var data = cmd.ExecuteDataSet
                    (
                    new { EmpNo = 0, EName = "" },
                    new { DeptNo = 0, DName = "" }
                    );
                var smith = data.Item1.FirstOrDefault(i => i.EmpNo == 7369);
                var accounting = data.Item2.FirstOrDefault(i => i.DeptNo == 10);

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(DEPT.Accounting.DeptNo, accounting.DeptNo);
                Assert.AreEqual(DEPT.Accounting.DName, accounting.DName);
            }
        }

        [TestMethod]
        public void ExecuteDataSetTyped_WithSimpleType_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME FROM EMP ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT ");
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                var data = cmd.ExecuteDataSet<EMP, DEPT, int>();
                var smith = data.Item1.FirstOrDefault(i => i.EmpNo == 7369);
                var accounting = data.Item2.FirstOrDefault(i => i.DeptNo == 10);
                var NbOfEmp = data.Item3;

                Assert.AreEqual(EMP.Smith.EmpNo, smith.EmpNo);
                Assert.AreEqual(EMP.Smith.EName, smith.EName);
                Assert.AreEqual(DEPT.Accounting.DName, accounting.DName);
                Assert.AreEqual(DEPT.Accounting.Loc, accounting.Loc);
                Assert.AreEqual(14, NbOfEmp.First());
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_Transaction_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                cmd.TransactionBegin();
                cmd.ExecuteNonQuery();
                cmd.TransactionRollback();

                Assert.AreEqual(EMP.GetEmployeesCount(_connection), 14);
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_DefineTransactionBefore_Test()
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var cmd = GetDatabaseCommand(_connection, transaction))
                {
                    cmd.Log = Console.WriteLine;
                    cmd.CommandText.AppendLine(" INSERT INTO EMP (EMPNO, ENAME) VALUES (1234, 'ABC') ");
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = GetDatabaseCommand(_connection, transaction))
                {
                    cmd.Log = Console.WriteLine;
                    cmd.CommandText.AppendLine(" INSERT INTO EMP (EMPNO, ENAME) VALUES (9876, 'XYZ') ");
                    cmd.ExecuteNonQuery();
                }

                transaction.Rollback();

                Assert.AreEqual(EMP.GetEmployeesCount(_connection, transaction), 14);
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoCommands_Test()
        {
            DbTransaction currentTransaction;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                currentTransaction = cmd.TransactionBegin();
                cmd.ExecuteNonQuery();

                Assert.AreEqual(EMP.GetEmployeesCount(_connection, currentTransaction), 0);     // Inside the transaction

                cmd.TransactionRollback();

                Assert.AreEqual(EMP.GetEmployeesCount(_connection, currentTransaction), 14);    // Inside the transaction
                Assert.AreEqual(EMP.GetEmployeesCount(_connection), 14);                      // Ouside the transaction
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoIncludedCommands_Test()
        {
            using (var cmd1 = GetDatabaseCommand(_connection))
            {
                cmd1.Log = Console.WriteLine;
                cmd1.CommandText.AppendLine(" DELETE FROM EMP ");
                cmd1.TransactionBegin();
                cmd1.ExecuteNonQuery();

                using (var cmd2 = GetDatabaseCommand(_connection, cmd1.Transaction))
                {
                    cmd2.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    int count = cmd2.ExecuteScalar<int>();
                }

                cmd1.TransactionRollback();
            }
        }

        [TestMethod]
        public void ExecuteRowDynamic_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP WHERE EMPNO = 7369");
                var emp = cmd.ExecuteRow<dynamic>();

                Assert.AreEqual(7369, emp.EMPNO);
                Assert.AreEqual("SMITH", emp.ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp.HIREDATE);
                Assert.AreEqual(null, emp.COMM);
            }
        }

        [TestMethod]
        public void ExecuteStarRowDynamic_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP WHERE EMPNO = 7369");
                var emp = cmd.ExecuteRow<dynamic>();

                Assert.AreEqual(7369, emp.EMPNO);
                Assert.AreEqual("SMITH", emp.ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp.HIREDATE);
                Assert.AreEqual(null, emp.COMM);
            }
        }

        [TestMethod]
        public void ExecuteTwoRowsDynamic_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP WHERE EMPNO = 7369");
                var emp1 = cmd.ExecuteRow<dynamic>();

                cmd.Clear();
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP WHERE EMPNO = 7499");
                var emp2 = cmd.ExecuteRow<dynamic>();

                Assert.AreEqual(7369, emp1.EMPNO);
                Assert.AreEqual("SMITH", emp1.ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp1.HIREDATE);
                Assert.AreEqual(null, emp1.COMM);

                Assert.AreEqual(7499, emp2.EMPNO);
                Assert.AreEqual("ALLEN", emp2.ENAME);
                Assert.AreEqual(new DateTime(1981, 02, 20), emp2.HIREDATE);
                Assert.AreEqual(300, emp2.COMM);
            }
        }

        [TestMethod]
        public void ExecuteTableDynamic_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM FROM EMP ORDER BY EMPNO");
                var emp = cmd.ExecuteTable<dynamic>();

                Assert.AreEqual(14, emp.Count());
                Assert.AreEqual("SMITH", emp.First().ENAME);
                Assert.AreEqual(new DateTime(1980, 12, 17), emp.First().HIREDATE);
                Assert.AreEqual(null, emp.First().COMM);
            }
        }

        [TestMethod]
        public void ExecuteScalarDynamic_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                var count = cmd.ExecuteScalar<dynamic>();

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void ChangeCommandType_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                Assert.AreEqual(System.Data.CommandType.StoredProcedure, cmd.CommandType);
            }
        }

        #region QUERY FORMATTED

        [TestMethod]
        public void GetFormattedAsText_Simple_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.CommandText.Append(" SELECT * FROM EMP ");
                string formatted = cmd.GetCommandTextFormatted();

                Assert.AreEqual(formatted, " SELECT * FROM EMP ");
            }
        }

        [TestMethod]
        public void GetFormattedAsText_Parameters_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.CommandText.Append(" SELECT *, @MyGuid FROM EMP WHERE EMPNO = @EmpNo AND ENAME LIKE @Ename AND HIREDATE > @Hire AND COMM = @Comm ");
                cmd.AddParameter("@EmpNo", 7369);                                                  // Parameter normal
                cmd.AddParameter("@ENAME", "%SM%");                                                // Parameter in Upper Case
                cmd.AddParameter("Hire", new DateTime(1970, 05, 04, 14, 15, 16));                  // Parameter without @
                cmd.AddParameter("@Comm", null);                                           // Parameter NULL
                cmd.AddParameter("@MyGuid", new Guid("2fff1b89-b5f9-4a33-ac5b-a3ffee3e8b82"));     // Parameter GUID

                string formatted = cmd.GetCommandTextFormatted();

                Assert.AreEqual(" SELECT *, '2fff1b89-b5f9-4a33-ac5b-a3ffee3e8b82' FROM EMP WHERE EMPNO = 7369 AND ENAME LIKE '%SM%' AND HIREDATE > '1970-05-04 14:15:16' AND COMM = NULL ", formatted);
            }
        }

        [TestMethod]
        public void GetFormattedAsHtml_Parameters_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
                cmd.CommandText.AppendLine("  WHERE EMPNO = @EmpNo AND ENAME LIKE @Ename AND HIREDATE > @Hire AND COMM = @Comm ");
                cmd.AddParameter("@EmpNo", 7369);                                    // Parameter normal
                cmd.AddParameter("@ENAME", "%SM%");                                  // Parameter in Upper Case
                cmd.AddParameter("Hire", new DateTime(1970, 05, 04, 14, 15, 16));    // Parameter without @
                cmd.AddParameter("@Comm", null);                                     // Parameter NULL

                string formatted = (cmd as DatabaseCommand).GetCommandTextFormatted(QueryFormat.Html);

                Assert.AreEqual(formatted, @" <span style=""color: #33f; font-weight: bold;"">SELECT</span> * <span style=""color: #33f; font-weight: bold;"">FROM</span> EMP <br/>  <span style=""color: #33f; font-weight: bold;"">WHERE</span> EMPNO = <span style=""color: #FF3F00;"">7369</span> <span style=""color: #33f; font-weight: bold;"">AND</span> ENAME <span style=""color: #33f; font-weight: bold;"">LIKE</span> <span style=""color: #FF3F00;"">'%SM%'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> HIREDATE &gt; <span style=""color: #FF3F00;"">'<span style=""color: #FF3F00;"">1970</span><span style=""color: #FF3F00;"">-05</span><span style=""color: #FF3F00;"">-04</span> <span style=""color: #FF3F00;"">14</span>:<span style=""color: #FF3F00;"">15</span>:<span style=""color: #FF3F00;"">16</span>'</span> <span style=""color: #33f; font-weight: bold;"">AND</span> COMM = <span style=""color: #33f; font-weight: bold;"">NULL</span> <br/>");
            }
        }

        #endregion

        #region ACTION BEFORE EXECUTIONS

        [TestMethod]
        public void ExecuteNonQuery_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    command.CommandText.Clear();
                    command.CommandText.Append("SELECT 1+1 FROM EMP");
                    isPassed = true;
                };

                cmd.ExecuteNonQuery();

                Assert.IsTrue(isPassed);
                Assert.AreEqual("SELECT 1+1 FROM EMP", cmd.CommandText.ToString());
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionAfterExecution = (command, tables) =>
                {
                    tables.First().Rows[0].ItemArray[0] = 10;
                    isPassed = true;
                };

                int rowsAffected = cmd.ExecuteNonQuery();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(10, rowsAffected);
            }
        }

        [TestMethod]
        public void ExecuteScalar_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    command.CommandText.Clear();
                    command.CommandText.Append("SELECT 1+1 FROM EMP");      // New Count
                    isPassed = true;
                };

                int count = cmd.ExecuteScalar<int>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(2, count);                                  // Check new Count
            }
        }

        [TestMethod]
        public void ExecuteScalar_ActionBefore_ChangeParameter_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT SAL FROM EMP WHERE EMPNO = @EmployeeID");
                cmd.AddParameter("@EmployeeID", 1234);

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.Parameters["@EmployeeID"].Value = 7369;
                    isPassed = true;
                };

                var salary = cmd.ExecuteScalar<decimal>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(800, salary);                                  // Check Salary for 7369 (and not 1234)
            }
        }

        [TestMethod]
        public void ExecuteScalar_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");

                cmd.ActionAfterExecution = (command, tables) =>
                {
                    tables.First().Rows[0].ItemArray[0] = 10;               // New Count
                    isPassed = true;
                };

                int count = cmd.ExecuteScalar<int>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(10, count);                                  // Check new Count
            }
        }

        [TestMethod]
        public void ExecuteRow_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.CommandText.AppendLine(" WHERE EMPNO = 7369 ");
                    isPassed = true;
                };

                var row = cmd.ExecuteRow<EMP>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual("SMITH", row.EName);
            }
        }

        [TestMethod]
        public void ExecuteRow_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    isPassed = true;
                };

                var row = cmd.ExecuteRow<EMP>();

                Assert.IsTrue(isPassed);
            }
        }

        [TestMethod]
        public void ExecuteTable_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.CommandText.AppendLine(" WHERE EMPNO > 7369 ");
                    isPassed = true;
                };

                var data = cmd.ExecuteTable<EMP>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(13, data.Count());
                Assert.AreEqual("ALLEN", data.First().EName);
            }
        }

        [TestMethod]
        public void ExecuteTable_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    isPassed = true;
                };

                var data = cmd.ExecuteTable<EMP>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(14, data.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_ActionBefore_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP; ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT; ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    cmd.CommandText.Replace("FROM EMP;", "FROM EMP WHERE EMPNO > 7369; ");
                    isPassed = true;
                };

                var data = cmd.ExecuteDataSet<EMP, DEPT>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(13, data.Item1.Count());
                Assert.AreEqual(4, data.Item2.Count());
            }
        }

        [TestMethod]
        public void ExecuteDataSet_ActionAfter_Test()
        {
            bool isPassed = false;

            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SELECT * FROM EMP; ");
                cmd.CommandText.AppendLine(" SELECT * FROM DEPT; ");

                cmd.ActionBeforeExecution = (command) =>
                {
                    isPassed = true;
                };

                var data = cmd.ExecuteDataSet<EMP, DEPT>();

                Assert.IsTrue(isPassed);
                Assert.AreEqual(14, data.Item1.Count());
                Assert.AreEqual(4, data.Item2.Count());
            }
        }

        #endregion

        #region EXTENSIONS

        [TestMethod]
        public void Parameter_AddWithValueOrDBNull_Test()
        {
            using (var cmd = GetDatabaseCommand(_connection))
            {
                cmd.Log = Console.WriteLine;
                cmd.CommandText.AppendLine(" SET ANSI_NULLS OFF ");
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP WHERE COMM = @Comm ");
                cmd.AddParameter("@Comm", null);

                Assert.AreEqual(cmd.ExecuteScalar<int>(), 10);
            }
        }

        #endregion

        #region GET DBCOMMAND


        private IDatabaseCommand GetDatabaseCommand(DbConnection connection)
        {
            return new DatabaseCommand(connection);
        }

        private IDatabaseCommand GetDatabaseCommand(DbConnection connection, DbTransaction transaction)
        {
            return new DatabaseCommand(connection, transaction);
        }

        private IDatabaseCommand GetDatabaseCommand(DbConnection connection, string commandText)
        {
            return new DatabaseCommand(connection, commandText);
        }

        #endregion

    }
}

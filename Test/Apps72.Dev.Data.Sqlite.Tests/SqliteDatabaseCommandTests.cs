using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Apps72.Dev.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using Windows.Storage;
using System.Diagnostics;
using System.Linq;

namespace Apps72.Dev.Data.Sqlite.Tests
{
    [TestClass]
    public class SqliteDatabaseCommandTests
    {
        #region INITIALIZATION

        private SqliteConnection _connection;

        [TestInitialize]
        public void Initialization()
        {
            _connection = Scott.CreateScottDatabase();
        }

        #endregion

        #region EXECUTE METHODS

        [TestMethod]
        public void ExecuteRowTyped_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT * FROM EMP WHERE EMPNO = 7369");
                EMP emp = cmd.ExecuteRow<EMP>();

                Assert.AreEqual(emp.EmpNo, 7369);
            }
        }

        [TestMethod]
        public void ExecuteRowPrimitive_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT EMPNO FROM EMP WHERE EMPNO = 7369");
                long empno = cmd.ExecuteRow<long>();

                Assert.AreEqual(empno, 7369);
            }
        }

        [TestMethod]
        public void ExecuteRowPrimitiveNullable_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT COMM FROM EMP WHERE EMPNO = 7369");
                int? comm = cmd.ExecuteRow<int?>();

                Assert.AreEqual(comm, null);
            }
        }

        [TestMethod]
        public void ExecuteRowWithDelegate_Test()
        {
            //using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            //{
            //    cmd.Log = (msg) => { Debug.WriteLine(msg); };
            //    cmd.CommandText.AppendLine(" SELECT * FROM EMP ");
            //    EMP emp = cmd.ExecuteRow<EMP>()
            //    //EMP emp = cmd.ExecuteRow<EMP>((row) => new EMP() { EmpNo = Convert.ToInt32(row["EMPNO"]) });

            //    Assert.AreEqual(emp.EmpNo, 7369);
            //}
        }

        [TestMethod]
        public void ExecuteScalar_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                object data = cmd.ExecuteScalar();

                Assert.AreEqual((Int64)14, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithParameter_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE EMPNO = @EmpNo ")
                               .AppendLine("   AND JOB = @Job ");

                cmd.Parameters.AddWithValue("@EmpNo", 7369);
                cmd.Parameters.AddWithValue("@Job", "CLERK");

                object data = cmd.ExecuteScalar();

                Assert.AreEqual(data, "SMITH");
            }
        }

        [TestMethod]
        public void ExecuteScalarWithDateParameter_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT ENAME ")
                               .AppendLine("  FROM EMP ")
                               .AppendLine(" WHERE HIREDATE = @HireDate ");

                cmd.Parameters.AddWithValue("@HireDate", new DateTime(1980, 12, 17));

                object data = cmd.ExecuteScalar();

                Assert.AreEqual("SMITH", data);
            }
        }

        [TestMethod]
        public void ExecuteScalarWithAnonymousParameters_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
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
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
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
        public void ExecuteScalarTyped_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                Int64 data = cmd.ExecuteScalar<Int64>();

                Assert.AreEqual((Int64)14, data);
            }
        }

        [TestMethod]
        public void ExecuteScalarTypedNull_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLineFormat(" SELECT COMM FROM EMP WHERE EMPNO = {0} ", 7369);
                int? data = cmd.ExecuteScalar<int?>();

                Assert.AreEqual(data, null);
            }
        }

        [TestMethod]
        public void ExecuteTableTyped_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
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
        public void ExecuteTableCustomedTyped_Test()
        {
            //using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            //{
            //    cmd.Log = (msg) => { Debug.WriteLine(msg); };
            //    cmd.CommandText.AppendLine(" SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP ");
            //    var data = cmd.ExecuteTable<EMP>((row) =>
            //    {
            //        return new EMP()
            //        {
            //            EmpNo = row.Field<int>("EMPNO"),
            //            EName = row.Field<string>("ENAME"),
            //            HireDate = row.Field<DateTime>("HIREDATE"),
            //            Comm = row.Field<int?>("COMM")
            //        };
            //    });
            //    EMP smith = data.FirstOrDefault(i => i.EmpNo == 7369);

            //    Assert.AreEqual(smith.EmpNo, EMP.Smith.EmpNo);
            //    Assert.AreEqual(smith.EName, EMP.Smith.EName);
            //    Assert.AreEqual(smith.HireDate, EMP.Smith.HireDate);
            //    Assert.AreEqual(smith.Comm, EMP.Smith.Comm);
            //}
        }

        [TestMethod]
        public void ExecuteTableTypedWithColumnAttribute_Test()
        {
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
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
            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                cmd.TransactionBegin();
                cmd.ExecuteNonQuery();
                cmd.TransactionRollback();

                Assert.AreEqual(14, EMP.GetEmployeesCount(_connection));
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_DefineTransactionBefore_Test()
        {
            using (SqliteTransaction transaction = _connection.BeginTransaction())
            {
                using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection, transaction))
                {
                    cmd.Log = (msg) => { Debug.WriteLine(msg); };
                    cmd.CommandText.AppendLine(" INSERT INTO EMP (EMPNO, ENAME) VALUES (1234, 'ABC') ");
                    cmd.ExecuteNonQuery();
                }

                using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection, transaction))
                {
                    cmd.Log = (msg) => { Debug.WriteLine(msg); };
                    cmd.CommandText.AppendLine(" INSERT INTO EMP (EMPNO, ENAME) VALUES (9876, 'XYZ') ");
                    cmd.ExecuteNonQuery();
                }

                transaction.Rollback();

                Assert.AreEqual(14, EMP.GetEmployeesCount(_connection));
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoCommands_Test()
        {
            SqliteTransaction currentTransaction;

            using (SqliteDatabaseCommand cmd = new SqliteDatabaseCommand(_connection))
            {
                cmd.Log = (msg) => { Debug.WriteLine(msg); };
                cmd.CommandText.AppendLine(" DELETE FROM EMP ");

                currentTransaction = cmd.TransactionBegin();
                cmd.ExecuteNonQuery();

                Assert.AreEqual(0, EMP.GetEmployeesCount(_connection, currentTransaction));     // Inside the transaction

                cmd.TransactionRollback();  // Dissociate the Transaction with the Connection

                Assert.AreEqual(14, EMP.GetEmployeesCount(_connection));                      // Ouside the transaction
            }
        }

        [TestMethod]
        public void ExecuteNonQuery_TransactionForTwoIncludedCommands_Test()
        {
            using (SqliteDatabaseCommand cmd1 = new SqliteDatabaseCommand(_connection))
            {
                cmd1.Log = (msg) => { Debug.WriteLine(msg); };
                cmd1.CommandText.AppendLine(" DELETE FROM EMP ");
                cmd1.TransactionBegin();
                cmd1.ExecuteNonQuery();

                using (SqliteDatabaseCommand cmd2 = new SqliteDatabaseCommand(_connection, cmd1.Transaction))
                {
                    cmd2.CommandText.AppendLine(" SELECT COUNT(*) FROM EMP ");
                    long count = cmd2.ExecuteScalar<long>();
                }

                cmd1.TransactionRollback();
            }
        }

        #endregion
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using Apps72.Dev.Data;

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

        #region EXECUTE SCALAR

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

        #endregion

    }
}

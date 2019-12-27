using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using Apps72.Dev.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Performances
{
    public class BasicSamples
    {
        private DbConnection _connection;
        private ScottContext _context;

        public BasicSamples()
        {
            //_connection = new ScottInMemory().Connection;
            _connection = new ScottFromSqlServer().Connection;
            _context = new ScottContext(_connection);
        }

        [Benchmark]
        public void Dapper_ExecuteScalar_Int()
        {
            var empno = _connection.ExecuteScalar<int>("SELECT TOP 1 EMPNO FROM EMP");
        }

        [Benchmark]
        public void Dapper_ExecuteTable_5Cols_14Rows()
        {
            var data = _connection.Query<EMP>("SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP", buffered: false).ToArray();
        }

        [Benchmark]
        public void DbCmd_ExecuteScalar_Int()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT TOP 1 EMPNO FROM EMP";
                var empno = cmd.ExecuteScalar<int>();
            }
        }

        [Benchmark]
        public void DbCmd_ExecuteTable_5Cols_14Rows()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP";
                var data = cmd.ExecuteTable<EMP>();
            }
        }

        [Benchmark]
        public void EF_ExecuteScalar_Int()
        {
            var empno = _context.Employees.First().EMPNO;
        }

        [Benchmark]
        public void EF_ExecuteTable_5Cols_14Rows()
        {
            var data = _context.Employees.ToArray();
        }

        public void DbCmd_Samples()
        {
            // Functions
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP";
                var data = cmd.ExecuteTable(row =>
                {
                    return new
                    {
                        Id = row[0],
                        Name = row["ENAME"],
                        HireDate = row.Field<DateTime>("HireDate")
                    };
                }).ToArray();
            }

            // Anonymous
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME FROM EMP";
                var data = cmd.ExecuteTable(new { EMPNO = 0, ENAME = "" }).ToArray();
            }

            // Row Typed
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP";
                var data = cmd.ExecuteRow<EMP>();
            }

            // Row Anonymous
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME MGR FROM EMP";
                var data = cmd.ExecuteRow(new { EMPNO = 0, ENAME = "" });
            }

            // Row Function
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP";
                var data = cmd.ExecuteRow(row =>
                {
                    return new
                    {
                        Id = row[0],
                        Name = row["ENAME"],
                        HireDate = row.Field<DateTime>("HireDate")
                    };
                });
            }
        }

        [Table("EMP")]
        class EMP
        {
            [Key]
            public int EMPNO { get; set; }
            public string ENAME { get; set; }
            public DateTime HIREDATE { get; set; }
            public int? COMM { get; set; }
            public int? MGR { get; set; }
        }

        class ScottContext : DbContext
        {
            private DbConnection _connection;

            public ScottContext(DbConnection connection)
            {
                _connection = connection;
            }

            public DbSet<EMP> Employees { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(_connection);
            }
        }
    }
}

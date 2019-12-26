using System;
using System.Data.Common;
using System.Linq;
using Apps72.Dev.Data;
using BenchmarkDotNet.Attributes;
using Dapper;

namespace Performances
{
    public class BasicSamples
    {
        private DbConnection _connection;

        public BasicSamples()
        {
            //_connection = new ScottInMemory().Connection;
            _connection = new ScottFromSqlServer().Connection;
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
                var data = cmd.ExecuteTable<EMP>().ToArray();
            }
        }

        [Benchmark]
        public void DbCmd_ExecuteTable_Anonymous()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME FROM EMP";
                var data = cmd.ExecuteTable(new { EMPNO = 0, ENAME = "" }).ToArray();
            }
        }

        [Benchmark]
        public void DbCmd_ExecuteTable_WithFunction()
        {
            using (var cmd = new DatabaseCommand(_connection))
            {
                cmd.CommandText = "SELECT EMPNO, ENAME, HIREDATE, COMM, MGR FROM EMP";
                var data = cmd.ExecuteTable(row =>
                {
                    return new
                    {
                        Id = row[0],
                        Name = row["ENAME"],
                    };
                }).ToArray();
            }
        }

        class EMP
        {
            public int EMPNO { get; set; }
            public string ENAME { get; set; }
            public DateTime HIREDATE { get; set; }
            public int? COMM { get; set; }
            public int? MGR { get; set; }
        }
    }
}

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Performances
{
    public class ScottInMemory
    {
        public ScottInMemory()
        {
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();

            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = @"CREATE TABLE dept
                                    (
                                        DEPTNO NUMBER(5)  NOT NULL,
                                        DNAME  VARCHAR2(20),
                                        LOC    VARCHAR2(13)
                                    );


                                    CREATE TABLE emp
                                    (
                                        EMPNO     NUMBER(5),
                                        ENAME     VARCHAR2(10),
                                        JOB       VARCHAR2(9),
                                        MGR       NUMBER(5),
                                        HIREDATE  VARCHAR2(9),
                                        SAL       NUMBER(8),
                                        COMM      NUMBER(8),
                                        DEPTNO    NUMBER(5)
                                    );

                                    INSERT INTO DEPT VALUES (10,'ACCOUNTING','NEW YORK');
                                    INSERT INTO DEPT VALUES (20,'RESEARCH','DALLAS');
                                    INSERT INTO DEPT VALUES (30,'SALES','CHICAGO');
                                    INSERT INTO DEPT VALUES (40,'OPERATIONS','BOSTON');

                                    INSERT INTO EMP VALUES  (7369, 'SMITH', 'CLERK',    7902,'1980-12-17',  800, NULL, 20);
                                    INSERT INTO EMP VALUES  (7499, 'ALLEN', 'SALESMAN', 7698,'1981-02-20', 1600, 300,  30);
                                    INSERT INTO EMP VALUES  (7521, 'WARD',  'SALESMAN', 7698,'1981-02-22', 1250, 500,  30);
                                    INSERT INTO EMP VALUES  (7566, 'JONES', 'MANAGER',  7839,'1981-04-02', 2975, NULL, 20);
                                    INSERT INTO EMP VALUES  (7654, 'MARTIN','SALESMAN', 7698,'1981-09-28', 1250, 1400, 30);
                                    INSERT INTO EMP VALUES  (7698, 'BLAKE', 'MANAGER',  7839,'1981-05-01', 2850, NULL, 30);
                                    INSERT INTO EMP VALUES  (7782, 'CLARK', 'MANAGER',  7839,'1981-06-09', 2450, NULL, 10);
                                    INSERT INTO EMP VALUES  (7788, 'SCOTT', 'ANALYST',  7566,'1987-07-13', 3000, NULL, 20);
                                    INSERT INTO EMP VALUES  (7839, 'KING',  'PRESIDENT',NULL,'1981-11-17', 5000, NULL, 10);
                                    INSERT INTO EMP VALUES  (7844, 'TURNER','SALESMAN', 7698,'1981-09-08', 1500, 0,    30);
                                    INSERT INTO EMP VALUES  (7876, 'ADAMS', 'CLERK',    7788,'1987-07-13', 1100, NULL, 20);
                                    INSERT INTO EMP VALUES  (7900, 'JAMES', 'CLERK',    7698,'1981-12-03',  950, NULL, 30);
                                    INSERT INTO EMP VALUES  (7902, 'FORD',  'ANALYST',  7566,'1981-12-03', 3000, NULL, 20);
                                    INSERT INTO EMP VALUES  (7934, 'MILLER','CLERK',    7782,'1982-01-23', 1300, NULL, 10);

";

                cmd.ExecuteNonQuery();
            }
        }

        public SqliteConnection Connection { get; set; }

    }
}

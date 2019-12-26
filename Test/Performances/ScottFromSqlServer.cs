using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Performances
{
    class ScottFromSqlServer
    {
        public ScottFromSqlServer()
        {
            Connection = new SqlConnection("Server=(localdb)\\ProjectsV14;Database=Scott;");
            Connection.Open();
        }

        public SqlConnection Connection { get; set; }
    }
}

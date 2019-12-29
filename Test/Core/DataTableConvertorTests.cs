using Apps72.Dev.Data.Convertor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Core.Tests
{
    [TestClass]
    public class DataTableConvertorTests
    {
        [TestMethod]
        public void DataTableConvertor_ArrayOfObjects_Test()
        {
            var data = new[]
            {
                new Employee { Id = 1, Name = "Denis", Birthdate = new DateTime(2019, 12, 29) },
                new Employee { Id = 2, Name = "Anne", Birthdate = new DateTime(2018, 10, 26) },
            };

            var table = DataTableConvertor.ToDataTable(data).First();

            Assert.AreEqual("Id", table.Columns[0].ColumnName);
            Assert.AreEqual("Name", table.Columns[1].ColumnName);
            Assert.AreEqual("string", table.Columns[1].CSharpType);
            Assert.AreEqual("Birthdate", table.Columns[2].ColumnName);
            Assert.AreEqual("DateTime", table.Columns[2].CSharpType);
            Assert.AreEqual(true, table.Columns[2].IsNullable);

            Assert.AreEqual(1, table.Rows[0][0]);
            Assert.AreEqual(2, table.Rows[1].Field<int>("Id"));

        }

        [TestMethod]
        public void DataTableConvertor_ArrayOfIntegers_Test()
        {
            var data = new[] { 1, 2 };

            var table = DataTableConvertor.ToDataTable(data).First();

            Assert.AreEqual("Column", table.Columns[0].ColumnName);
            Assert.AreEqual(1, table.Rows[0][0]);
            Assert.AreEqual(2, table.Rows[1][0]);

        }

        private class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime? Birthdate { get; set; }
        }
    }
}

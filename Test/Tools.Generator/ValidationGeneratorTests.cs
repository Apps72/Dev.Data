using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Tools.Generator.Tests
{
    [TestClass]
    public class ValidationGeneratorTests
    {
        [TestMethod]
        public void Validation_TryValidateObject_Test()
        {
            var smith = new Person()
            {
                Id = 1,
                LastName = "Smith",
                Age = 50
            };

            var errors = Validate(smith);

            Assert.AreEqual(false, errors.Any());
        }

        private IEnumerable<ValidationResult> Validate(object value)
        {
            var context = new ValidationContext(value, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(value, context, results, validateAllProperties: true);
            return results;
        }

        public class Person
        {
            [Key()]
            public int Id { get; set; }

            [Required]
            [StringLength(50)]
            public string LastName { get; set; }

            [Range(0, 100)]
            public int Age { get; set; }
        }
    }
}

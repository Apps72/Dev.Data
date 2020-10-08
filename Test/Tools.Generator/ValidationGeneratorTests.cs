using Apps72.Dev.Data.Generator.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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
                Id = 2,
                LastName = "Smith",
                Age = 50
            };

            var errors = smith.Validate();

            Assert.AreEqual(false, errors.Any(), String.Join(" ; ", errors.Select(i => i.ErrorMessage)));
        }

        [TestMethod]
        public void Validation_NullAttribute_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"Validations",
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsFalse(code.Contains("[Range"));
            Assert.IsFalse(code.Contains("[StringLength"));
        }

        [TestMethod]
        public void Validation_RangeAttribute_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"Validations=Range",
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("[Range(-999999999999999999.0d, 999999999999999999.0d)]"));
            Assert.IsTrue(code.Contains("public virtual decimal? SAL { get; set; }"));
        }

        [TestMethod]
        public void Validation_RangeStringAttribute_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"Validations=Range;StringLength",
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("[Range(-999999999999999999.0d, 999999999999999999.0d)]"));
            Assert.IsTrue(code.Contains("[StringLength(10)]"));
        }

        [TestMethod]
        public void Validation_StringAttribute_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"Validations=StringLength",
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("[StringLength(10)]"));
            Assert.IsTrue(code.Contains("public virtual string ENAME { get; set; }"));
        }

        [TestMethod]
        public void Validation_ValidateMethod_Test()
        {
            var args = new[]
            {
                $"GenerateEntities",
                $"cs=\"{Configuration.SQLSERVER_CONNECTION_STRING}\"",
                $"Validations=ValidateMethod",
            };
            var generator = new Apps72.Dev.Data.Generator.Tools.Generator(new Arguments(args));
            var code = generator.Code;

            Assert.IsTrue(code.Contains("private static class __DataAnnotationValidator"));
            Assert.IsTrue(code.Contains("Validate() =>"));
        }

        public class Person : IValidatableObject
        {
            [Key()]
            public int Id { get; set; }

            [Required]
            [StringLength(50)]
            public string LastName { get; set; }

            [Range(0d, 100d)]
            public int Age { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (Id % 2 != 0)
                    yield return new ValidationResult("Id must be an Odd number");
            }

            /// <summary />
            public IEnumerable<ValidationResult> Validate() => __DataAnnotationValidator.ValidateObject(this);
            
        }

        /// <summary />
        private static class __DataAnnotationValidator
        {
            /// <summary />
            public static IEnumerable<ValidationResult> ValidateObject(object value)
            {
                var context = new ValidationContext(value, serviceProvider: null, items: null);
                var results = new List<ValidationResult>();
                Validator.TryValidateObject(value, context, results, validateAllProperties: true);

                // By default, IValidatableObject.Validate is not called if errors are already found.
                // https://stackoverflow.com/questions/3400542/how-do-i-use-ivalidatableobject
                if (value is IValidatableObject && results.Any())
                {
                    var otherResults = ((IValidatableObject)value).Validate(context).ToArray();
                    return results.Union(otherResults);
                }
                else
                {
                    return results;
                }
            }
        }

    }
}

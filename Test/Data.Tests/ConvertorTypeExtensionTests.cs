using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apps72.Dev.Data.Convertor;

namespace Data.Tests
{
    [TestClass]
    public class ConvertorTypeExtensionTests
    {
        [TestMethod]
        public void Primitive_Test()
        {
            Type type = typeof(int);

            Assert.AreEqual(typeof(Int32), TypeExtension.GetNullableSubType(type));
            Assert.AreEqual(false, TypeExtension.IsAnonymousType(type));
            Assert.AreEqual(false, TypeExtension.IsNullable(type));
            Assert.AreEqual(true, TypeExtension.IsPrimitive(type));
        }

        [TestMethod]
        public void PrimitiveNullable_Test()
        {
            Type type = typeof(int?);

            Assert.AreEqual(typeof(Int32), TypeExtension.GetNullableSubType(type));
            Assert.AreEqual(false, TypeExtension.IsAnonymousType(type));
            Assert.AreEqual(true, TypeExtension.IsNullable(type));
            Assert.AreEqual(true, TypeExtension.IsPrimitive(type));
        }

        [TestMethod]
        public void Object_Test()
        {
            Type type = typeof(System.IO.FileInfo);

            Assert.AreEqual(typeof(System.IO.FileInfo), TypeExtension.GetNullableSubType(type));
            Assert.AreEqual(false, TypeExtension.IsAnonymousType(type));
            Assert.AreEqual(true, TypeExtension.IsNullable(type));
            Assert.AreEqual(false, TypeExtension.IsPrimitive(type));
        }

        [TestMethod]
        public void StaticClass_Test()
        {
            Type type = typeof(System.IO.File);

            Assert.AreEqual(typeof(System.IO.File), TypeExtension.GetNullableSubType(type));
            Assert.AreEqual(false, TypeExtension.IsAnonymousType(type));
            Assert.AreEqual(true, TypeExtension.IsNullable(type));
            Assert.AreEqual(false, TypeExtension.IsPrimitive(type));
        }

        [TestMethod]
        public void Anonymous_Test()
        {
            Type type = (new { MyName = "ABC" }).GetType();

            Assert.AreEqual(true, TypeExtension.IsAnonymousType(type));
            Assert.AreEqual(true, TypeExtension.IsNullable(type));
            Assert.AreEqual(false, TypeExtension.IsPrimitive(type));
        }
    }
}

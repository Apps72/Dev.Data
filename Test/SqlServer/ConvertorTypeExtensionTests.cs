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
        #region INITIALIZATION

        private const string ASSEMBLY_NAME = "Apps72.Dev.Data";
        private const string TYPE_NAME = "Apps72.Dev.Data.Convertor.TypeExtension";

        private PrivateType _extensionType;

        [TestInitialize]
        public void Initialization()
        {
            _extensionType = new PrivateType(ASSEMBLY_NAME, TYPE_NAME);
        }


        #endregion
        [TestMethod]
        public void Primitive_Test()
        {
            Type type = typeof(int);

            Assert.AreEqual(typeof(Int32), _extensionType.InvokeStatic("GetNullableSubType", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsAnonymousType", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsNullable", type));
            Assert.AreEqual(true, _extensionType.InvokeStatic("IsPrimitive", type));
        }

        [TestMethod]
        public void PrimitiveNullable_Test()
        {
            Type type = typeof(int?);

            Assert.AreEqual(typeof(Int32), _extensionType.InvokeStatic("GetNullableSubType", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsAnonymousType", type));
            Assert.AreEqual(true, _extensionType.InvokeStatic("IsNullable", type));
            Assert.AreEqual(true, _extensionType.InvokeStatic("IsPrimitive", type));
        }

        [TestMethod]
        public void Object_Test()
        {
            Type type = typeof(System.IO.FileInfo);

            Assert.AreEqual(typeof(System.IO.FileInfo), _extensionType.InvokeStatic("GetNullableSubType", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsAnonymousType", type));
            Assert.AreEqual(true, _extensionType.InvokeStatic("IsNullable", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsPrimitive", type));
        }

        [TestMethod]
        public void StaticClass_Test()
        {
            Type type = typeof(System.IO.File);

            Assert.AreEqual(typeof(System.IO.File), _extensionType.InvokeStatic("GetNullableSubType", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsAnonymousType", type));
            Assert.AreEqual(true, _extensionType.InvokeStatic("IsNullable", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsPrimitive", type));
        }

        [TestMethod]
        public void Anonymous_Test()
        {
            Type type = (new { MyName = "ABC" }).GetType();

            Assert.AreEqual(true, _extensionType.InvokeStatic("IsAnonymousType", type));
            Assert.AreEqual(true, _extensionType.InvokeStatic("IsNullable", type));
            Assert.AreEqual(false, _extensionType.InvokeStatic("IsPrimitive", type));
        }
    }
}

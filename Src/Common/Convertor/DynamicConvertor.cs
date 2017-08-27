using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace Common.Convertor
{

    /// <summary>
    /// Helpers to build and instanciate class.
    /// </summary>
    internal static class DynamicConvertor
    {
        internal const string DYNAMIC_NAMESPACE = "Apps72.Dev.Data.Dynamic";
        internal const string DYNAMIC_CLASS_NAME = "AnonymousClass";

#if NETCOREAPP1_1
        /// <summary>
        /// Returns always False.
        /// </summary>
        /// <param name="type">NA</param>
        /// <returns></returns>
        public static bool IsDynamic(Type type)
        {
            return false;
        }

        /// <summary>
        /// Returns always null.
        /// </summary>
        /// <param name="className">NA</param>
        /// <param name="properties">NA</param>
        /// <returns></returns>
        public static Type GetDynamicType(string className, IDictionary<string, Type> properties)
        {
            return null;
        }
#else
        /// <summary>
        /// Returns True if the <paramref name="type"/> is dynamic.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDynamic(Type type)
        {
            // TODO: To find a best method !
            if (type.Namespace == "System" && type.Name == "Object")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns a new object created and instanciated dynamically, and containing specified properties.
        /// </summary>
        /// <param name="className">Name of the class to create.</param>
        /// <param name="properties">List of properties to add to this class.</param>
        /// <returns></returns>
        public static dynamic GetDynamicObject(string className, IDictionary<string, Type> properties)
        {
            return Activator.CreateInstance(GetDynamicType(className, properties));
        }

        /// <summary>
        /// Returns a new type created dynamically and containing specified properties.
        /// </summary>
        /// <param name="className">Name of the type to create.</param>
        /// <param name="properties">List of properties to add to this type.</param>
        /// <returns></returns>
        public static Type GetDynamicType(string className, IDictionary<string, Type> properties)
        {

#if NETCOREAPP2_0
            // Create the Builder (in .NET Core)
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(DYNAMIC_NAMESPACE);
#else
            // Create the builder (in .NET)
            AssemblyName assembly = new AssemblyName(DYNAMIC_NAMESPACE);
            AppDomain appDomain = System.Threading.Thread.GetDomain();
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assembly.Name);
#endif
            // Create the class
            TypeBuilder typeBuilder = moduleBuilder.DefineType(className, 
                                                               TypeAttributes.Public | 
                                                               TypeAttributes.AutoClass | 
                                                               TypeAttributes.AnsiClass | 
                                                               TypeAttributes.BeforeFieldInit, 
                                                               typeof(System.Object));

            // Add properties
            foreach (var prop in properties)
            {
                AddProperty(typeBuilder, prop.Key, prop.Value);
            }

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Add a new property dynamically to the <paramref name="typeBuilder"/>.
        /// </summary>
        /// <param name="typeBuilder">Builder of this type.</param>
        /// <param name="propertyName">Name of the new property</param>
        /// <param name="propertyType">Type of the new property</param>
        private static void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            string propName = KeepOnlyLettersOrDigits(propertyName);
            string privateField = $"_{propName.ToLower()}";
            string getMethod = $"get_{propName}";
            string setMethod = $"set_{propName}";

            // Create an attribute to hide the private field
            var attributeToHideField = new CustomAttributeBuilder(
                typeof(DebuggerBrowsableAttribute).GetConstructor(new Type[] { typeof(DebuggerBrowsableState) }),
                new Object[] { DebuggerBrowsableState.Never });

            // Field
            FieldBuilder field = typeBuilder.DefineField(privateField, propertyType, FieldAttributes.Private);
            field.SetCustomAttribute(attributeToHideField);

            // Property
            PropertyBuilder property = typeBuilder.DefineProperty(propName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Get_Method
            MethodBuilder propertyGetter = typeBuilder.DefineMethod(getMethod, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator propertyGetterIL = propertyGetter.GetILGenerator();
            propertyGetterIL.Emit(OpCodes.Ldarg_0);
            propertyGetterIL.Emit(OpCodes.Ldfld, field);
            propertyGetterIL.Emit(OpCodes.Ret);

            // Set_Method
            MethodBuilder propertySetter = typeBuilder.DefineMethod(setMethod, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { propertyType });
            ILGenerator propertySetterIL = propertySetter.GetILGenerator();
            propertySetterIL.Emit(OpCodes.Ldarg_0);
            propertySetterIL.Emit(OpCodes.Ldarg_1);
            propertySetterIL.Emit(OpCodes.Stfld, field);
            propertySetterIL.Emit(OpCodes.Ret);

            // Assign getter and setter
            property.SetGetMethod(propertyGetter);
            property.SetSetMethod(propertySetter);
        }

        /// <summary>
        /// Returns a similar string with only digits (0 .. 9) or letter (a .. z; A .. Z).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string KeepOnlyLettersOrDigits(string text)
        {
            var result = new StringBuilder();
            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }
#endif
        }
    }

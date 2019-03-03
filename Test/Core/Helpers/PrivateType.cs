using System;
using System.Linq;
using System.Reflection;

namespace Core.Tests.Helpers
{
    /// <summary>
    /// PrivateType is not (yet) included in .NET Core
    /// </summary>
    public class PrivateType
    {
        private const BindingFlags BindToEveryThing = BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        private readonly Type _type;

        public PrivateType(string assemblyName, string typeName)
        {
            var assembly = Assembly.Load(assemblyName);
            _type = assembly.GetType(typeName);
        }

        public object InvokeStatic(string method, params object[] args)
        {
            MethodInfo staticMethodInfo;

            if (args == null || args.Length == 0)
                staticMethodInfo = _type.GetMethod(method, BindToEveryThing);
            else
                staticMethodInfo = _type.GetMethod(method, GetAllTypes(args));

            return staticMethodInfo.Invoke(null, args);
        }

        private Type[] GetAllTypes(object[] args)
        {
            return args.Select(i => i.GetType()).ToArray();
        }
    }
}

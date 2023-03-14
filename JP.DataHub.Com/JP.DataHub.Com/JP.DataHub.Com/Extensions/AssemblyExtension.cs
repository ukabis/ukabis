using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JP.DataHub.Com.Extensions
{
    public static class AssemblyExtension
    {
        public static Type[] GetTypes<T>(this Assembly assembly) where T : Attribute =>
            assembly.GetTypes().Where(x => x.GetCustomAttribute<T>() != null).ToArray();

        public static Type[] GetTypes<T, INHERITANCE>(this Assembly assembly) where T : Attribute =>
            assembly.GetTypes().Where(x => x.GetCustomAttribute<T>() != null && x.HasInheritance<INHERITANCE>() == true).ToArray();

        public static Type[] GetTypesByInterface<INHERITANCE>(this Assembly assembly) =>
            assembly.GetTypes().Where(x => x.HasInheritance<INHERITANCE>() == true).ToArray();
    }
}

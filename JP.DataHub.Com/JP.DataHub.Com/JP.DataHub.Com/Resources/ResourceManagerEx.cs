using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace JP.DataHub.Com.Resources
{
    public static class ResourceManagerEx
    {
        private static Dictionary<Type, ResourceManager> dic = new Dictionary<Type, ResourceManager>();

        private static object obj = new object();

        private static ResourceManager CreateTypedResourceManager(Type type)
        {
            var list = RuntimeReflectionExtensions.GetRuntimeProperties(type).ToList();
            var prop = list?.Where(x => x.Name == "ResourceManager").FirstOrDefault();
            var result = (ResourceManager)prop?.GetValue(null);
            return result;
        }

        public static ResourceManager GetResourceManager(Type type)
        {
            lock (obj)
            {
                if (dic.ContainsKey(type) == false)
                {
                    dic.Add(type, CreateTypedResourceManager(type));
                }
                return dic[type];
            }
        }
    }
}
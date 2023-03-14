using System;
using System.Reflection;
using JP.DataHub.Com.Cache.Attributes;

namespace JP.DataHub.Com.Cache
{
    internal class ClassMethodAttributeDefine
    {
        public string MethodName { get; set; }
        public string MethodKey { get; set; }
        public MethodInfo Method { get; set; }
        public string KeyPrefix { get; set; }
        public CacheAttribute CacheAttr { get; set; }
        public CacheArgAttribute CacheArgument { get; set; }
        public Attributes.CacheEntityAttribute CacheEntity { get; set; }
    }
}

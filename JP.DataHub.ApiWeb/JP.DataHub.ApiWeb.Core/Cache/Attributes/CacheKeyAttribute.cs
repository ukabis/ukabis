using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Core.Cache.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
    public class CacheKeyAttribute : Attribute
    {
        public CacheKeyType CacheKeyType { get; set; }
        public string[] Params { get; set; }

        public CacheKeyAttribute()
        {
        }

        public CacheKeyAttribute(CacheKeyType cacheKeyType, params string[] param)
        {
            CacheKeyType = cacheKeyType;
            Params = param;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;

namespace JP.DataHub.ApiWeb.Core.Cache
{
    public class CacheItems
    {
        public CacheKeyType CacheKeyType { get; set; }
        public string CacheKey { get; set; }

        public Type Type { get; set; }
        public List<string> KeyList { get; set; }

        public CacheItems(Type type, string cacheKey, CacheKeyAttribute attr)
        {
            Type = type;
            CacheKey = cacheKey;
            CacheKeyType = attr.CacheKeyType;
            KeyList = new List<string>(attr.Params);
        }

        public bool Containts(CacheKeyType cacheKeyType, string key)
        {
            return CacheKeyType == cacheKeyType && KeyList.Contains(key);
        }
    }
}

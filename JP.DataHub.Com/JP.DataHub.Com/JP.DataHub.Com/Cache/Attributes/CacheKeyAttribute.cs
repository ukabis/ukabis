using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache.Attributes
{
    public enum CacheKeyType
    {
        Entity,
        Id,
        EntityWithKey
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
    public class CacheKeyAttribute : Attribute
    {
        public CacheKeyAttribute()
        {
        }
    }
}

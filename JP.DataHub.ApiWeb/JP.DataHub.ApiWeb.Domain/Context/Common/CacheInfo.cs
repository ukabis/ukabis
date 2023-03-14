using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record CacheInfo : IValueObject
    {
        public bool IsCache { get; }
        public int CacheMinute { get; }
        public int CacheSecond { get { return CacheMinute * 60; } }
        public string CacheKey { get; }

        public CacheInfo(bool isCache, int cacheMinute, string cacheKey)
        {
            IsCache = isCache;
            CacheMinute = cacheMinute;
            CacheKey = cacheKey;
        }

        public static bool operator ==(CacheInfo me, object other) => me?.Equals(other) == true;

        public static bool operator !=(CacheInfo me, object other) => !me?.Equals(other) == true;
    }
}

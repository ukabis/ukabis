using JP.DataHub.Com.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using EasyCaching.Core;
using System.Reflection;
using System.Collections;
using StackExchange.Redis;
using Unity;

namespace JP.DataHub.Com.Cache
{
    public class RedisCache : AbstractCache 
    {
        public RedisCache(string name, int maxSizeLimit)
            : base(name, maxSizeLimit)
        {
        }

        public RedisCache(string name)
            : base(name)
        {
        }

        public RedisCache(int maxSizeLimit)
            : base(maxSizeLimit)
        {
        }

        public RedisCache()
            : base()
        {
        }

        public override IEnumerable<string> Keys()
        {
            var serversId = typeof(EasyCaching.Redis.DefaultRedisCachingProvider).GetField("_servers", BindingFlags.NonPublic | BindingFlags.Instance);
            var servers = serversId.GetValue(this.Provider) as IEnumerable<IServer>;
            foreach(var server in servers)
            {
                foreach (var key in server.Keys())
                {
                    yield return key;
                }
            }
        }
    }
}

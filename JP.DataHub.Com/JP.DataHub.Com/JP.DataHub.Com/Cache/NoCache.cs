using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Com.Cache
{
    public class NoCache : AbstractCache
    {

        public NoCache(string name, int maxSizeLimit)
            : base(name, maxSizeLimit)
        {
        }

        public NoCache(string name)
            : base(name)
        {
        }

        public NoCache(int maxSizeLimit)
            : base(maxSizeLimit)
        {
        }

        public NoCache()
            : base()
        {
        }

        public override void Clear()
        {
        }

        public override bool Contains(string key)
        {
            return false;
        }

        public override void Add(string key, object obj, TimeSpan absoluteExpiration, int maxSaveSize)
        {
        }

        public override void Add(string key, object obj, TimeSpan absoluteExpiration)
        {
        }

        public override void Add(string key, object obj, int hourExpiration, int minuteExpiration, int secExpiration)
        {
        }

        public override void Add(string key, object obj, int secExpiration)
        {
        }

        public override void Add(string key, object obj, int secExpiration, int maxSaveSize)
        {
        }

        public override void Add(string key, object obj)
        {
        }

        public override T Get<T>(string key, out bool isNullValue, bool isUseMessagePack = false)
        {
            isNullValue = false;
            return default(T);
        }
        public override object Get(Type type, string key)
        {
            return default;
        }

        public override void Remove(string key)
        {
        }

        public override void RemovePattern(List<object> param)
        {
        }

        public override void RemovePatternByKeyOnly(string key)
        {
        }

        public override Task RemoveFirstMatchAsync(string key) => Task.CompletedTask;

        public override void RemoveFirstMatch(string key)
        {
        }

        public override IEnumerable<string> Keys()
        {
            yield break;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Com.Cache
{
    public delegate object ActionObject();

    public interface ICache
    {
        bool IsFlash { get; set; }
        Action<bool> CacheStatusNotify { get; set; }
        void Close();
        void Clear();
        bool Contains(string key);
        void Add(string key, object obj);
        void Add(string key, object obj, int secExpiration);
        void Add(string key, object obj, int secExpiration, int maxSaveSize);
        void Add(string key, object obj, int hourExpiration, int minuteExpiration, int secExpiration);
        void Add(string key, object obj, TimeSpan absoluteExpiration);
        void Add(string key, object obj, TimeSpan absoluteExpiration, int maxSaveSize);
        void Remove(string key);
        Task RemoveAsync(string key);
        void RemovePattern(List<object> param);
        void RemovePatternByKeyOnly(string key);
        void RemoveFirstMatch(string key);
        Task RemoveFirstMatchAsync(string key);
        object Get(Type type, string key);
        object Get(Type type, string key, ActionObject misshit_action);
        object Get(Type type, string key, int secExpiration, ActionObject misshit_action);
        object Get(Type type, string key, int hourExpiration, int minuteExpiration, int secExpiration, ActionObject misshit_action);
        object Get(Type type, string key, TimeSpan absoluteExpiration, ActionObject misshit_action);
        T Get<T>(string key, out bool isNullValue, bool isUseMessagePack = false);
        T Get<T>(string key, ActionObject misshit_action);
        T Get<T>(string key, int secExpiration, ActionObject misshit_action);
        T Get<T>(string key, int hourExpiration, int minuteExpiration, int secExpiration, ActionObject misshit_action);
        T Get<T>(string key, TimeSpan absoluteExpiration, ActionObject misshit_action);
        IEnumerable<string> Keys();
    }
}

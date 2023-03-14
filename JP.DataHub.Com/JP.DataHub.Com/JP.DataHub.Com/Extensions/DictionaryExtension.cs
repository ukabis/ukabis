using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Interception.Utilities;

namespace JP.DataHub.Com.Extensions
{
    public static class DictionaryExtension
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key) => dic.ContainsKey(key) ? dic[key] : default;
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue) => dic.ContainsKey(key) ? dic[key] : defaultValue;

        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dic, TKey key) => dic.ContainsKey(key) ? dic[key] : default;
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dic, TKey key, TValue defaultValue) => dic.ContainsKey(key) ? dic[key] : defaultValue;

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dic, IDictionary<TKey, TValue> other) 
        {
            if (other?.Any() == true)
            {
                dic ??= new Dictionary<TKey, TValue>();
                other.ToList().ForEach(pair => dic[pair.Key] = pair.Value);
            }
            return dic;
        }

        public static Dictionary<string, TValue> DeepCopy<TValue>(this IDictionary<string, TValue> src) => DeepCopy(src, StringComparer.InvariantCultureIgnoreCase);

        public static Dictionary<string, TValue> DeepCopy<TValue>(this IDictionary<string, TValue> src, StringComparer comparer)
        {
            var result = new Dictionary<string, TValue>(comparer);
            if (src != null)
            {
                src.Keys.ToList().ForEach(x => result.Add(x, src[x]));
            }
            return result;
        }

        public static IDictionary<string, object> Merge(this IDictionary<string, object> dic, object param)
        {
            var paramdic = param.ObjectToDictionary();
            paramdic.ForEach(x =>
            {
                if (IsPrimitive(x.Value?.GetType()) == true)
                {
                    dic.Add(x.Key, x.Value);
                }
            });
            return dic;
        }

        private static bool IsPrimitive(Type type)
        {
            if (type == null)
            {
                return false;
            }
            if (type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime))
            {
                return true;
            }
            return type.IsPrimitive;
        }
    }
}

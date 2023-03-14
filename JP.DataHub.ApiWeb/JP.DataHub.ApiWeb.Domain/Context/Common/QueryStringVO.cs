using System.Collections.ObjectModel;
using System.Web;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record QueryStringVO : IValueObject
    {
        public ReadOnlyDictionary<QueryStringKey, QueryStringValue> Dic { get; }

        public bool HasValue { get => Dic.Count() > 0; }

        /// <summary>
        /// 元のQueryString（?hoge=fuga）
        /// </summary>
        public string OriginalQueryString { get; }

        public QueryStringVO(IDictionary<QueryStringKey, QueryStringValue> dictionary, string orignalQueryString = null)
        {
            Dic = new ReadOnlyDictionary<QueryStringKey, QueryStringValue>(dictionary);
            OriginalQueryString = orignalQueryString;
        }

        public bool ContainKey(string key)
        {
            return Dic.ContainsKey(new QueryStringKey(key));
        }

        public string GetValue(string key)
        {
            var qkey = new QueryStringKey(key);
            if (Dic.ContainsKey(qkey) == true)
            {
                return Dic[qkey]?.Value;
            }
            return null;
        }

        /// <summary>
        /// QueryStringを取得する
        /// （純粋なQueryString以外にもUrlパラメータも設定されるため注意。）
        /// </summary>
        public string GetQueryString(bool encode = false)
        {
            return GetQueryString(new List<string>(), encode);
        }

        public string GetQueryString(List<string> ignoreList, bool encode = false)
        {
            if (encode)
            {
                return string.Join("&", this.Dic.Where(x => !ignoreList.Contains(x.Key.Value)).Select(x => string.IsNullOrEmpty(x.Value.Value) ? $"{x.Key.Value}" : $"{x.Key.Value}={HttpUtility.UrlEncode(x.Value.Value)}"));
            }
            else
            {
                return string.Join("&", this.Dic.Where(x => !ignoreList.Contains(x.Key.Value)).Select(x => string.IsNullOrEmpty(x.Value.Value) ? $"{x.Key.Value}" : $"{x.Key.Value}={x.Value.Value}"));
            }
        }

        public static bool operator ==(QueryStringVO me, object other) => me?.Equals(other) == true;

        public static bool operator !=(QueryStringVO me, object other) => !me?.Equals(other) == true;
    }
}

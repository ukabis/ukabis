using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JP.DataHub.Com.Collections;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class QueryString : ReadOnlyStringDictionary
    {
        public bool HasValue { get => this.Count > 0; }

        /// <summary>
        /// 元のQueryString（?hoge=fuga）
        /// </summary>
        public string Value { get; }

        public QueryString(IDictionary<string, string> dictionary, string orignalQueryString = null)
            : base(dictionary)
        {
            Value = orignalQueryString;
        }

        public bool ContainKey(string key) => base.ContainsKey(key);

        public string GetValue(string key)
        {
            if (this.ContainsKey(key) == true)
            {
                return this[key];
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
                return string.Join("&", this.Where(x => !ignoreList.Contains(x.Key)).Select(x => string.IsNullOrEmpty(x.Value) ? $"{x.Key}" : $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));
            }
            else
            {
                return string.Join("&", this.Where(x => !ignoreList.Contains(x.Key)).Select(x => string.IsNullOrEmpty(x.Value) ? $"{x.Key}" : $"{x.Key}={x.Value}"));
            }
        }

        public override int GetHashCode() => this.PropertiesGetHashCode();

        public override string ToString() => this.Count < 1 ? "" : this.JoinedValueShallowJProperties();
    }
}
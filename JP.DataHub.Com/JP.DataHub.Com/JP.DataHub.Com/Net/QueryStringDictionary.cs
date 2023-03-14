using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.Com.Net
{
    public class QueryStringDictionary : Dictionary<string, string>
    {
        public QueryStringDictionary(string queryString)
        {
            var list = HttpUtility.ParseQueryString(queryString);
            list.AllKeys.ToList().ForEach(x => {
                if (x != null)
                {
                    Add(x, list[x]);
                }
            });
        }

        public QueryStringDictionary Validate<T>(string key, Func<QueryStringDictionary, Exception> func)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("keyが指定されていません");
            }
            if (this.ContainsKey(key) == false)
            {
                throw func(this);
            }
            var val = this[key]?.ToString();
            if (val.IsValid<T>() == false)
            {
                throw func(this);
            }
            return this;
        }

        public bool IsValid<T>(string key)
        {
            var val = this.GetOrDefault(key);
            return IsValid<T>(val);
        }

        public QueryStringDictionary ThrowMessage(Func<QueryStringDictionary, bool> predicate, Func<QueryStringDictionary, Exception> func)
        {
            if (predicate(this) == true && func != null)
            {
                var exception = func(this);
                if (exception != null)
                {
                    throw exception;
                }
            }
            return this;
        }

        public string CollectUrl(string url)
        {
            foreach (var key in Keys)
            {
                url = url?.Replace($"{{{key}}}", this[key]);
            }
            return url;
        }

        public string ToQueryString()
        {
            return string.Join("&", this.Select(p => $"{p.Key}={p.Value}"));
        }

        public bool IsNullOrEmpty(string key)
        {
            if (ContainsKey(key) == false)
            {
                return true;
            }
            return string.IsNullOrEmpty(this[key]);
        }

        public string GetOrDefault(string key)
        {
            return this.ContainsKey(key) ? this[key] : null;
        }
    }
}

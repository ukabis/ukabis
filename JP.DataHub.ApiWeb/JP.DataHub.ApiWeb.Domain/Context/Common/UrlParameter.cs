using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record UrlParameter : IValueObject
    {
        public ReadOnlyDictionary<UrlParameterKey, UrlParameterValue> Dic { get; }

        public UrlParameter(IDictionary<UrlParameterKey, UrlParameterValue> dictionary)
        {
            Dic = new ReadOnlyDictionary<UrlParameterKey, UrlParameterValue>(dictionary);
        }

        public bool ContainKey(string key)
        {
            foreach (var dickey in Dic.Keys)
            {
                if (dickey == key)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainKey(UrlParameterKey key)
            => Dic.ContainsKey(key);

        public static bool operator ==(UrlParameter me, object other) => me?.Equals(other) == true;

        public static bool operator !=(UrlParameter me, object other) => !me?.Equals(other) == true;
    }
}

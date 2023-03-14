using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record QueryStringValue : IValueObject
    {
        public string Value { get; }

        public QueryStringValue(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(QueryStringValue me, object other) => me?.Equals(other) == true;

        public static bool operator !=(QueryStringValue me, object other) => !me?.Equals(other) == true;
    }

    internal static class QueryStringValueExtension
    {
        public static QueryStringValue ToUrlParameterKey(this string str) => str == null ? null : new QueryStringValue(str);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record QueryStringKey : IValueObject
    {
        public string Value { get; }

        public QueryStringKey(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(QueryStringKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(QueryStringKey me, object other) => !me?.Equals(other) == true;
    }

    internal static class QueryStringKeyExtension
    {
        public static QueryStringKey ToQueryStringKey(this string val) => val == null ? null : new QueryStringKey(val);
    }
}

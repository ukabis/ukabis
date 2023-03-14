using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class Query : IValueObject
    {
        public string Value { get; }

        public Query(string query)
        {
            this.Value = query;
        }

        public static bool operator ==(Query me, object other) => me?.Equals(other) == true;

        public static bool operator !=(Query me, object other) => !me?.Equals(other) == true;
    }

    internal static class QueryExtension
    {
        public static Query ToQuery(this string val) => val == null ? null : new Query(val);
    }
}

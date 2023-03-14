using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiUri : IValueObject
    {
        public string Value { get; }

        public ApiUri(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(ApiUri me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiUri me, object other) => !me?.Equals(other) == true;
    }

    internal static class ApiUriExtension
    {
        public static ApiUri ToApiUri(this string str) => str == null ? null : new ApiUri(str);
    }
}

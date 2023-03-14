using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiQuery : IValueObject
    {
        public string Value { get; }

        public ApiQuery(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(ApiQuery me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiQuery me, object other) => !me?.Equals(other) == true;
    }

    internal static class ApiQueryExtension
    {
        public static ApiQuery ToApiQuery(this string val) => val == null ? null : new ApiQuery(val);
    }
}
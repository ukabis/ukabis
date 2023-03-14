using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiAccessVendorId : IValueObject
    {
        public string Value { get; }

        public ApiAccessVendorId(string apiAccessVendorId)
        {
            this.Value = apiAccessVendorId;
        }

        public static bool operator ==(ApiAccessVendorId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiAccessVendorId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ApiAccessVendorIdExtension
    {
        public static ApiAccessVendorId ToApiAccessVendorId(this string str) => str == null ? null : new ApiAccessVendorId(str);
    }
}

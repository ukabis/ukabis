using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record ClientIpaddress : IValueObject
    {
        public string Value { get; }

        public ClientIpaddress(string value)
        {
            Value = value;
        }

        public static bool operator ==(ClientIpaddress me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ClientIpaddress me, object other) => !me?.Equals(other) == true;
    }

    internal static class ClientIpaddressExtension
    {
        public static ClientIpaddress ToClientIpaddress(this string val) => val == null ? null : new ClientIpaddress(val);
    }
}

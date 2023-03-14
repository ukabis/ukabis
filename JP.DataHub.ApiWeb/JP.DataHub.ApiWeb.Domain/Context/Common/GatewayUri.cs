using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record GatewayUri : IValueObject
    {
        public string Value { get; }

        public GatewayUri(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(GatewayUri me, object other) => me?.Equals(other) == true;

        public static bool operator !=(GatewayUri me, object other) => !me?.Equals(other) == true;
    }

    internal static class GatewayUriExtension
    {
        public static GatewayUri ToGatewayUri(this string val) => val == null ? null : new GatewayUri(val);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record JwtSecurityTokenValue : IValueObject
    {
        public string Value { get; }

        public JwtSecurityTokenValue(string value)
        {
            Value = value;
        }

        public static bool operator ==(JwtSecurityTokenValue me, object other) => me?.Equals(other) == true;

        public static bool operator !=(JwtSecurityTokenValue me, object other) => !me?.Equals(other) == true;
    }

    internal static class JwtSecurityTokenValueExtension
    {
        public static JwtSecurityTokenValue ToJwtSecurityTokenValue(this string val) => val == null ? null : new JwtSecurityTokenValue(val);
    }
}

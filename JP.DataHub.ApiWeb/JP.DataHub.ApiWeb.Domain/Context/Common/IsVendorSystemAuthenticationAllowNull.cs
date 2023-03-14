using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsVendorSystemAuthenticationAllowNull : IValueObject
    {
        public bool Value { get; }

        public IsVendorSystemAuthenticationAllowNull(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsVendorSystemAuthenticationAllowNull me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsVendorSystemAuthenticationAllowNull me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsVendorSystemAuthenticationAllowNullExtension
    {
        public static IsVendorSystemAuthenticationAllowNull ToIsVendorSystemAuthenticationAllowNull(this bool? flag) => flag == null ? null : new IsVendorSystemAuthenticationAllowNull(flag.Value);
        public static IsVendorSystemAuthenticationAllowNull ToIsVendorSystemAuthenticationAllowNull(this bool flag) => new IsVendorSystemAuthenticationAllowNull(flag);
        public static IsVendorSystemAuthenticationAllowNull ToIsVendorSystemAuthenticationAllowNull(this string str) => ToIsVendorSystemAuthenticationAllowNull(str.Convert<bool?>());
    }
}

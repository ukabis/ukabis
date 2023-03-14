using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsOpenIdAuthentication : IValueObject
    {
        public bool Value { get; }

        public IsOpenIdAuthentication(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsOpenIdAuthentication me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsOpenIdAuthentication me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsOpenIdAuthenticationExtension
    {
        public static IsOpenIdAuthentication ToIsOpenIdAuthentication(this bool? flag) => flag == null ? null : new IsOpenIdAuthentication(flag.Value);
        public static IsOpenIdAuthentication ToIsOpenIdAuthentication(this bool flag) => new IsOpenIdAuthentication(flag);
        public static IsOpenIdAuthentication ToIsOpenIdAuthentication(this string str) => ToIsOpenIdAuthentication(str.Convert<bool?>());
    }
}

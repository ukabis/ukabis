using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsAdminAuthentication : IValueObject
    {
        public bool Value { get; }

        public IsAdminAuthentication(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsAdminAuthentication me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsAdminAuthentication me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsAdminAuthenticationExtension
    {
        public static IsAdminAuthentication ToIsAdminAuthentication(this bool? val) => val == null ? null : new IsAdminAuthentication(val.Value);
        public static IsAdminAuthentication ToIsAdminAuthentication(this bool val) => new IsAdminAuthentication(val);
        public static IsAdminAuthentication ToIsAdminAuthentication(this string val) => ToIsAdminAuthentication(val.Convert<bool?>());
    }
}

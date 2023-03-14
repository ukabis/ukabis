using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsClientCertAuthentication : IValueObject
    {
        public bool Value { get; }

        public IsClientCertAuthentication(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsClientCertAuthentication me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsClientCertAuthentication me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsClientCertAuthenticationExtension
    {
        public static IsClientCertAuthentication ToIsClientCertAuthentication(this bool? val) => val == null ? null : new IsClientCertAuthentication(val.Value);
        public static IsClientCertAuthentication ToIsClientCertAuthentication(this bool val) => new IsClientCertAuthentication(val);
        public static IsClientCertAuthentication ToIsClientCertAuthentication(this string val) => ToIsClientCertAuthentication(val.Convert<bool?>());
    }
}

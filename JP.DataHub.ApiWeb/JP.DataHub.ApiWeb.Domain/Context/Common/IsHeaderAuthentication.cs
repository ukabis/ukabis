using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class IsHeaderAuthentication : IValueObject
    {
        public bool Value { get; }

        public IsHeaderAuthentication(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsHeaderAuthentication me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsHeaderAuthentication me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsHeaderAuthenticationExtension
    {
        public static IsHeaderAuthentication ToIsHeaderAuthentication(this bool? flag) => flag == null ? null : new IsHeaderAuthentication(flag.Value);
        public static IsHeaderAuthentication ToIsHeaderAuthentication(this bool flag) => new IsHeaderAuthentication(flag);
        public static IsHeaderAuthentication ToIsHeaderAuthentication(this string str) => ToIsHeaderAuthentication(str.Convert<bool?>());
    }
}

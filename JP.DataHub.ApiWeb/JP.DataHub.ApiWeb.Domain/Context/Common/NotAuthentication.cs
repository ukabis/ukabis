using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record NotAuthentication : IValueObject
    {
        public bool Value { get; set; }

        public NotAuthentication(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(NotAuthentication me, object other) => me?.Equals(other) == true;

        public static bool operator !=(NotAuthentication me, object other) => !me?.Equals(other) == true;
    }

    internal static class NotAuthenticationExtension
    {
        public static NotAuthentication ToNotAuthentication(this bool value) => new NotAuthentication(value);
    }
}

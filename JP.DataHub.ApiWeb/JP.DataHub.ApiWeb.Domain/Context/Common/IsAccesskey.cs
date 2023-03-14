using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsAccesskey : IValueObject
    {
        public bool Value { get; }

        public IsAccesskey(bool isEnabled)
        {
            this.Value = isEnabled;
        }

        public static bool operator ==(IsAccesskey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsAccesskey me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsAccesskeyExtension
    {
        public static IsAccesskey ToIsAccesskey(this bool? val) => val == null ? null : new IsAccesskey(val.Value);
        public static IsAccesskey ToIsAccesskey(this bool val) => new IsAccesskey(val);
        public static IsAccesskey ToIsAccesskey(this string val) => ToIsAccesskey(val.Convert<bool?>());
    }
}

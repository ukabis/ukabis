using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    class IsOpenidAuthenticationAllowNull : IValueObject
    {
        [Key(0)]
        public bool Value { get; }

        public IsOpenidAuthenticationAllowNull(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsOpenidAuthenticationAllowNull me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsOpenidAuthenticationAllowNull me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsOpenidAuthenticationAllowNullExtension
    {
        public static IsOpenidAuthenticationAllowNull ToIsOpenidAuthenticationAllowNull(this bool? val) => val == null ? null : new IsOpenidAuthenticationAllowNull(val.Value);
        public static IsOpenidAuthenticationAllowNull ToIsOpenidAuthenticationAllowNull(this bool val) => new IsOpenidAuthenticationAllowNull(val);
        public static IsOpenidAuthenticationAllowNull ToIsOpenidAuthenticationAllowNull(this string val) => ToIsOpenidAuthenticationAllowNull(val.Convert<bool?>());
    }
}
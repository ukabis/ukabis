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
    internal record IsActive : IValueObject
    {
        [Key(0)]
        public bool Value { get; }

        public IsActive(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsActive me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsActive me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsActiveExtension
    {
        public static IsActive ToIsActive(this bool? flag) => flag == null ? null : new IsActive(flag.Value);
        public static IsActive ToIsActive(this bool flag) => new IsActive(flag);
        public static IsActive ToIsActive(this string str) => ToIsActive(str.Convert<bool?>());
    }
}

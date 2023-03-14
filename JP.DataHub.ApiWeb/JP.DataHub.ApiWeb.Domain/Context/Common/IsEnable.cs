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
    internal record IsEnable : IValueObject
    {
        [Key(0)]
        public bool Value { get; }

        public IsEnable(bool isEnabled)
        {
            this.Value = isEnabled;
        }

        public static bool operator ==(IsEnable me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsEnable me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsEnableExtension
    {
        public static IsEnable ToIsEnable(this bool? val) => val == null ? null : new IsEnable(val.Value);
        public static IsEnable ToIsEnable(this bool val) => new IsEnable(val);
        public static IsEnable ToIsEnable(this string val) => ToIsEnable(val.Convert<bool?>());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsTransparentApi : IValueObject
    {
        public bool Value { get; }

        public IsTransparentApi(bool isEnabled)
        {
            this.Value = isEnabled;
        }

        public static bool operator ==(IsTransparentApi me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsTransparentApi me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsTransparentApiExtension
    {
        public static IsTransparentApi ToIsTransparentApi(this bool? val) => val == null ? null : new IsTransparentApi(val.Value);
        public static IsTransparentApi ToIsTransparentApi(this bool val) => new IsTransparentApi(val);
        public static IsTransparentApi ToIsTransparentApi(this string val) => ToIsTransparentApi(val.Convert<bool?>());
    }
}

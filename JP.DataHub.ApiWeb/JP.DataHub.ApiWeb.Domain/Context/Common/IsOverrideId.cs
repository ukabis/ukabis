using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsOverrideId : IValueObject
    {
        public bool Value { get; }

        public IsOverrideId(bool isEnabled = true)
        {
            this.Value = isEnabled;
        }

        public static bool operator ==(IsOverrideId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsOverrideId me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsOverrideIdExtension
    {
        public static IsOverrideId ToIsOverrideId(this bool? val) => val == null ? null : new IsOverrideId(val.Value);
        public static IsOverrideId ToIsOverrideId(this bool val) => new IsOverrideId(val);
        public static IsOverrideId ToIsOverrideId(this string val) => ToIsOverrideId(val.Convert<bool?>());
    }
}

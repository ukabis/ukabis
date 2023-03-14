using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsAutomaticId : IValueObject
    {
        public bool Value { get; }

        public IsAutomaticId(bool isEnabled)
        {
            this.Value = isEnabled;
        }

        public static bool operator ==(IsAutomaticId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsAutomaticId me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsAutomaticIdExtension
    {
        public static IsAutomaticId ToIsAutomaticId(this bool? val) => val == null ? null : new IsAutomaticId(val.Value);
        public static IsAutomaticId ToIsAutomaticId(this bool val) => new IsAutomaticId(val);
        public static IsAutomaticId ToIsAutomaticId(this string val) => ToIsAutomaticId(val.Convert<bool?>());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ActionTypeVersion : IValueObject
    {
        public int Value { get; }

        public ActionTypeVersion(int value)
        {
            Value = value;
        }

        public static bool operator ==(ActionTypeVersion me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ActionTypeVersion me, object other) => !me?.Equals(other) == true;
    }

    internal static class ActionTypeVersionExtension
    {
        public static ActionTypeVersion ToActionTypeVersion(this string val) => val == null ? null : new ActionTypeVersion(val.To<int>());
        public static ActionTypeVersion ToActionTypeVersion(this int? val) => val == null ? null : new ActionTypeVersion(val.Value);
        public static ActionTypeVersion ToActionTypeVersion(this int val) => new ActionTypeVersion(val);
    }
}

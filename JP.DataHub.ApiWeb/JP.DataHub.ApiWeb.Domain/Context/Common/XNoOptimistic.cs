using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XNoOptimistic : IValueObject
    {
        public bool Value { get; }

        public XNoOptimistic(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(XNoOptimistic me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XNoOptimistic me, object other) => !me?.Equals(other) == true;
    }

    internal static class XNoOptimisticExtension
    {
        public static XNoOptimistic ToXNoOptimistic(this bool? val) => val == null ? null : new XNoOptimistic(val.Value);
        public static XNoOptimistic ToXNoOptimistic(this bool val) => new XNoOptimistic(val);
    }
}

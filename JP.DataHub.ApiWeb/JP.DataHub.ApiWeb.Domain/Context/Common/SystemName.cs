using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    public record SystemName : IValueObject
    {
        public string Value { get; }

        public SystemName(string value)
        {
            Value = value.ToLower();
        }

        public static bool operator ==(SystemName me, object other) => me?.Equals(other) == true;

        public static bool operator !=(SystemName me, object other) => !me?.Equals(other) == true;
    }

    internal static class SystemNameExtension
    {
        public static SystemName ToSystemName(this string val) => val == null ? null : new SystemName(val);
    }
}

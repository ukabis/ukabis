using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject(keyAsPropertyName: true)]
    public record ResourceVersion : IValueObject
    {
        public int Value { get; }

        public ResourceVersion(int value)
        {
            this.Value = value;
        }

        public static bool operator ==(ResourceVersion me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ResourceVersion me, object other) => !me?.Equals(other) == true;
    }

    internal static class ResourceVersionExtension
    {
        public static ResourceVersion ToResourceVersion(this int? val) => val == null ? null : new ResourceVersion(val.Value);
        public static ResourceVersion ToResourceVersion(this int val) => new ResourceVersion(val);
    }
}

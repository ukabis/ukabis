using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsOverPartition : IValueObject
    {
        public bool Value { get; }
        public IsOverPartition(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsOverPartition me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsOverPartition me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsOverPartitionExtension
    {
        public static IsOverPartition ToIsOverPartition(this bool value) => new IsOverPartition(value);
    }
}

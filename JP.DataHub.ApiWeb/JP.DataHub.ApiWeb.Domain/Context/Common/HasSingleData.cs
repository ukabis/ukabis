using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record HasSingleData : IValueObject
    {
        public bool Value { get; }

        public HasSingleData(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(HasSingleData me, object other) => me?.Equals(other) == true;

        public static bool operator !=(HasSingleData me, object other) => !me?.Equals(other) == true;
    }

    internal static class HasSingleDataExtension
    {
        public static HasSingleData ToHasSingleData(this bool val) => new HasSingleData(val);
    }
}

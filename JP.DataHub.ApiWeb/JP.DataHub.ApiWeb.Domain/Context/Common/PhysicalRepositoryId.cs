using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record PhysicalRepositoryId : IValueObject
    {
        public string Value { get; }

        public PhysicalRepositoryId(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(PhysicalRepositoryId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(PhysicalRepositoryId me, object other) => !me?.Equals(other) == true;
    }

    internal static class PhysicalRepositoryIdExtension
    {
        public static PhysicalRepositoryId ToPhysicalRepositoryId(this string val) => val == null ? null : new PhysicalRepositoryId(val);
    }
}
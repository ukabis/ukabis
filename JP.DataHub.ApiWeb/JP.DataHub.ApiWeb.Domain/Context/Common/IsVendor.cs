using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsVendor : IValueObject
    {
        public bool Value { get; }

        public IsVendor(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsVendor me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsVendor me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsVendorExtension
    {
        public static IsVendor ToIsVendor(this bool val) => new IsVendor(val);
    }
}
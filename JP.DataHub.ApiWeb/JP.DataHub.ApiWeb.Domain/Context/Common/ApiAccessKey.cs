using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiAccessKey : IValueObject
    {
        public Guid? Value { get; }

        public bool IsValid { get; internal set; }

        public ApiAccessKey()
        {
        }

        public ApiAccessKey(bool valid)
        {
            IsValid = valid;
        }

        public ApiAccessKey(Guid? value)
        {
            this.Value = value;
            IsValid = true;
        }

        public static bool operator ==(ApiAccessKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiAccessKey me, object other) => !me?.Equals(other) == true;
    }

    internal static class ApiAccessKeyExtension
    {
        public static ApiAccessKey ToApiAccessKey(this bool value) => new ApiAccessKey(value);
        public static ApiAccessKey ToApiAccessKey(this Guid? value) => value == null ? null : new ApiAccessKey(value);
    }
}

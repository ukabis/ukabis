using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsUseBlobCache : IValueObject
    {
        public bool Value { get; }

        public IsUseBlobCache(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsUseBlobCache me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsUseBlobCache me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsUseBlobCacheExtension
    {
        public static IsUseBlobCache ToIsUseBlobCache(this bool? val) => val == null ? null : new IsUseBlobCache(val.Value);
        public static IsUseBlobCache ToIsUseBlobCache(this bool val) => new IsUseBlobCache(val);
    }
}
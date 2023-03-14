using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class IsEnableResourceVersion : IValueObject
    {
        [Required]
        public bool Value { get; }

        public IsEnableResourceVersion(bool value)
        {
            Value = value;
        }

        public static bool operator ==(IsEnableResourceVersion me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsEnableResourceVersion me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsEnableResourceVersionExtension
    {
        public static IsEnableResourceVersion ToIsEnableResourceVersion(this bool? val) => val == null ? null : new IsEnableResourceVersion(val.Value);
        public static IsEnableResourceVersion ToIsEnableResourceVersion(this bool val) => new IsEnableResourceVersion(val);
        public static IsEnableResourceVersion ToIsEnableResourceVersion(this string val) => ToIsEnableResourceVersion(val.Convert<bool?>());
    }
}
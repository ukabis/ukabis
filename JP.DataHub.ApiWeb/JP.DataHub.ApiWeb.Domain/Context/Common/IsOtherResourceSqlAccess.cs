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
    internal record IsOtherResourceSqlAccess : IValueObject
    {
        [Required]
        public bool Value { get; }

        public IsOtherResourceSqlAccess(bool value)
        {
            Value = value;
        }

        public static bool operator ==(IsOtherResourceSqlAccess me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsOtherResourceSqlAccess me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsOtherResourceSqlAccessExtension
    {
        public static IsOtherResourceSqlAccess ToIsOtherResourceSqlAccess(this bool? val) => val == null ? null : new IsOtherResourceSqlAccess(val.Value);
        public static IsOtherResourceSqlAccess ToIsOtherResourceSqlAccess(this bool val) => new IsOtherResourceSqlAccess(val);
        public static IsOtherResourceSqlAccess ToIsOtherResourceSqlAccess(this string val) => ToIsOtherResourceSqlAccess(val.Convert<bool?>());
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsSkipJsonSchemaValidation : IValueObject
    {
        public bool Value { get; }

        public IsSkipJsonSchemaValidation(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsSkipJsonSchemaValidation me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsSkipJsonSchemaValidation me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsSkipJsonSchemaValidationExtension
    {
        public static IsSkipJsonSchemaValidation ToIsSkipJsonSchemaValidation(this bool? val) => val == null ? null : new IsSkipJsonSchemaValidation(val.Value);
        public static IsSkipJsonSchemaValidation ToIsSkipJsonSchemaValidation(this bool val) => new IsSkipJsonSchemaValidation(val);
        public static IsSkipJsonSchemaValidation ToIsSkipJsonSchemaValidation(this string val) => ToIsSkipJsonSchemaValidation(val.Convert<bool?>());
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    internal record IsArray : IValueObject
    {
        [Required]
        public bool Value { get; }

        public IsArray(bool value)
        {
            Value = value;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public IsArray(string value)
        {
            bool result;
            if (bool.TryParse(value, out result) == false)
            {
                throw new ArgumentException($"IsArray constructor value({value}) is invalid.");
            }
            Value = result;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(IsArray me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsArray me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsArrayExtension
    {
        public static IsArray ToIsArray(this bool? val) => val == null ? null : new IsArray(val.Value);
        public static IsArray ToIsArray(this bool val) => new IsArray(val);
        public static IsArray ToIsArray(this string val) => ToIsArray(val.Convert<bool?>());
    }
}
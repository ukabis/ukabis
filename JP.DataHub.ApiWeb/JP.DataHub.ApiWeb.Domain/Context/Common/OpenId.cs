using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Validations;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    public record OpenId : IValueObject
    {
        [Required]
        [Type(typeof(Guid))]
        public string Value { get; }

        public OpenId(string value)
        {
            Value = value?.ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public OpenId(Guid value)
        {
            Value = value.ToString().ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(OpenId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(OpenId me, object other) => !me?.Equals(other) == true;
    }

    internal static class OpenIdExtension
    {
        public static OpenId ToOpenId(this Guid val) => new OpenId(val);
        public static OpenId ToOpenId(this string val) => val == null ? null : new OpenId(val);
    }
}

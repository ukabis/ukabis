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
    public record AccessTokenId : IValueObject
    {
        [Required]
        [Type(typeof(Guid))]
        public string Value { get; }

        public AccessTokenId(string value)
        {
            Value = value;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public AccessTokenId(Guid value)
        {
            Value = value.ToString();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(AccessTokenId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AccessTokenId me, object other) => !me?.Equals(other) == true;
    }
}

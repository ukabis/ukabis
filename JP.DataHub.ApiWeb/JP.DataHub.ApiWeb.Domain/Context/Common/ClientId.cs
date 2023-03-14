using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Validations;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    public record ClientId : IValueObject
    {
        [Type(typeof(Guid))]
        [RequiredEx(typeof(ArgumentNullException))]
        public string Value { get; }

        public ClientId(string value)
        {
            Value = value.ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public ClientId(Guid value)
        {
            Value = value.ToString().ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(ClientId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ClientId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ClientIdExtension
    {
        public static ClientId ToClientId(this Guid val) => new ClientId(val);
        public static ClientId ToClientId(this string val) => val == null ? null : new ClientId(val);
    }
}

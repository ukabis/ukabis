using System.ComponentModel.DataAnnotations;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Validations;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    [MessagePackObject]
    public record SystemId : IValueObject
    {
        [MessagePack.Key(0)]
        [Required]
        [Type(typeof(Guid))]
        public string Value { get; }

        [MessagePack.IgnoreMember]
        public Guid? ToGuid
        {
            get
            {
                Guid tmp;
                return Guid.TryParse(Value, out tmp) ? tmp : Guid.Empty;
            }
        }

        public SystemId(string value)
        {
            Value = value.ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public SystemId(Guid value)
        {
            Value = value.ToString().ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(SystemId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(SystemId me, object other) => !me?.Equals(other) == true;
    }

    internal static class SystemIdExtension
    {
        public static SystemId ToSystemId(this Guid guid) => new SystemId(guid);
        public static SystemId ToSystemId(this string str) => str == null ? null : new SystemId(str);
    }
}

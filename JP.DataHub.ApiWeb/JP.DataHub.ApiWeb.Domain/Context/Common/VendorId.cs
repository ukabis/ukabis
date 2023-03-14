using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Validations;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    [MessagePackObject]
    public record VendorId : IValueObject
    {
        [Required]
        [Type(typeof(Guid))]
        [MessagePack.Key(0)]
        public string Value { get; }

        [IgnoreMember]
        public Guid? ToGuid
        {
            get
            {
                Guid tmp;
                return Guid.TryParse(Value, out tmp) ? tmp : Guid.Empty; 
            }
        }

        [IgnoreMember]
        public bool IsOpenVendor { get => _isOpenVendor();  }

        public VendorId(string value)
        {
            Value = value?.ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        public VendorId(Guid value)
        {
            Value = value.ToString().ToLower();
            ValidatorEx.ExceptionValidateObject(this);
        }

        private bool _isOpenVendor()
            => UnityCore.ResolveOrDefault<string>("DefaultVendorId")?.ToLower() == Value.ToLower();


        public static bool operator ==(VendorId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(VendorId me, object other) => !me?.Equals(other) == true;
    }

    internal static class VendorIdExtension
    {
        public static VendorId ToVendorId(this Guid guid) => new VendorId(guid);
        public static VendorId ToVendorId(this string str) => str == null ? null : new VendorId(str);
    }
}

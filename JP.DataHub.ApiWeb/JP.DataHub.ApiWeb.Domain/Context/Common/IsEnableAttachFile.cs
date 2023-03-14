using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    internal record IsEnableAttachFile : IValueObject
    {
        [Key(0)]
        public bool Value { get; }

        public IsEnableAttachFile(bool isEnableAttachFile)
        {
            this.Value = isEnableAttachFile;
        }

        public static bool operator ==(IsEnableAttachFile me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsEnableAttachFile me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsEnableAttachFileExtension
    {
        public static IsEnableAttachFile ToIsEnableAttachFile(this bool? flag) => flag == null ? null : new IsEnableAttachFile(flag.Value);
        public static IsEnableAttachFile ToIsEnableAttachFile(this bool flag) => new IsEnableAttachFile(flag);
        public static IsEnableAttachFile ToIsEnableAttachFile(this string str) => ToIsEnableAttachFile(str.Convert<bool?>());
    }
}

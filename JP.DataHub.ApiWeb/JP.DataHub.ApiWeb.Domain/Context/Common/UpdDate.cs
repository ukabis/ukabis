using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    internal record UpdDate : IValueObject
    {
        [Key(0)]
        public DateTime Value { get; }

        public UpdDate(DateTime updDate)
        {
            this.Value = updDate;
        }

        public static bool operator ==(UpdDate me, object other) => me?.Equals(other) == true;

        public static bool operator !=(UpdDate me, object other) => !me?.Equals(other) == true;
    }

    internal static class UpdDateExtension
    {
        public static UpdDate ToUpdDate(this DateTime val) => val == null ? null : new UpdDate(val);
    }
}

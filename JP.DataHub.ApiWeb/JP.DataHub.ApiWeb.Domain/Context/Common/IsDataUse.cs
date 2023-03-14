using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    internal class IsDataUse : IValueObject
    {
        [Key(0)]
        public bool Value { get; }

        public IsDataUse(bool isDataUse)
        {
            this.Value = isDataUse;
        }

        public static bool operator ==(IsDataUse me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsDataUse me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsDataUseExtension
    {
        public static IsDataUse ToIsDataUse(this bool val) => new IsDataUse(val);
    }
}

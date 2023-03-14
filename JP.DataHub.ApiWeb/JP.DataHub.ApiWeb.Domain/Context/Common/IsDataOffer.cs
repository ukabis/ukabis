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
    internal record IsDataOffer : IValueObject
    {
        [Key(0)]
        public bool Value { get; }

        public IsDataOffer(bool isDataOffer)
        {
            this.Value = isDataOffer;
        }

        public static bool operator ==(IsDataOffer me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsDataOffer me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsDataOfferExtension
    {
        public static IsDataOffer ToIsDataOffer(this bool val) => new IsDataOffer(val);
    }
}

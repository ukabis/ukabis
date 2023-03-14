using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    [MessagePackObject(keyAsPropertyName: true)]
    public record VendorVO : IValueObject
    {
        public Guid vendor_id { get; private set; }
        public string vendor_name { get; private set; }
        public bool is_data_offer { get; private set; }
        public bool is_data_use { get; private set; }
        public bool is_enable { get; private set; }

        public VendorVO(Guid vendor_id, string vendor_name, bool is_data_offer, bool is_data_use, bool is_enable)
        {
            this.vendor_id = vendor_id;
            this.vendor_name = vendor_name;
            this.is_data_offer = is_data_offer;
            this.is_data_use = is_data_use;
            this.is_enable = is_enable;
        }

        public static bool operator ==(VendorVO me, object other) => me?.Equals(other) == true;

        public static bool operator !=(VendorVO me, object other) => !me?.Equals(other) == true;
    }
}
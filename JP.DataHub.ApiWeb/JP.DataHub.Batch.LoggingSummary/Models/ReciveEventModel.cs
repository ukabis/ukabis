using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingSummary.Models
{
    public class ReciveEventModel
    {
        public string ApiId { get; set; }

        public string ControllerId { get; set; }

        public string ProviderSystemId { get; set; }

        public string ProviderVendorId { get; set; }

        public string SystemId { get; set; }

        public string VendorId { get; set; }

        public DateTime RequestDate { get; set; }

        public bool Equals(ReciveEventModel other)
        {
            if (ApiId == other.ApiId &&
                ControllerId == other.ControllerId &&
                ProviderVendorId == other.ProviderVendorId &&
                ProviderSystemId == other.ProviderSystemId &&
                VendorId == other.VendorId &&
                SystemId == other.SystemId &&
                RequestDate == other.RequestDate
            )

            {
                return true;
            }

            return false;
        }
        public override int GetHashCode()
        {
            var apiIdHash = ApiId.GetHashCode();
            var controllerIdHash = ControllerId.GetHashCode();
            var providerSystemIdHash = ProviderSystemId.GetHashCode();
            var providerVendorIdHash = ProviderVendorId.GetHashCode();
            var systemIdHash = SystemId.GetHashCode();
            var vendorIdHash = VendorId.GetHashCode();
            var requestDateHash = RequestDate.GetHashCode();
            return apiIdHash ^ controllerIdHash ^ providerSystemIdHash ^ providerVendorIdHash ^ systemIdHash ^ vendorIdHash ^ requestDateHash;
        }

        public override bool Equals(object obj)
        {
            return obj is ReciveEventModel ? Equals((ReciveEventModel)obj) : base.Equals(obj);
        }

        public static bool operator ==(ReciveEventModel obj1, ReciveEventModel obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(ReciveEventModel obj1, ReciveEventModel obj2)
        {
            return !(obj1 == obj2);
        }
    }
}

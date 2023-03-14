using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Models
{
    public class SummaryEventModel
    {
        public string ApiId { get; }

        public string ControllerId { get; }

        public string ProviderSystemId { get; }

        public string ProviderVendorId { get; }

        public string SystemId { get; }

        public string VendorId { get; }

        public DateTime RequestDate { get; }

        public SummaryEventModel(string apiId, string controllerId, string providerSystemId, string providerVendorId, string systemId, string vendorId, DateTime requestDate)
        {
            ApiId = apiId;
            ControllerId = controllerId;
            ProviderSystemId = providerSystemId;
            ProviderVendorId = providerVendorId;
            SystemId = systemId;
            VendorId = vendorId;
            RequestDate = requestDate;
        }
    }
}

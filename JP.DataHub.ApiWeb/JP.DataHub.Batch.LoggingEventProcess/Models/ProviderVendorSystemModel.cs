using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Models
{
    public class ProviderVendorSystemModel
    {
        public string ProviderVendorId { get; set; }

        public string ProviderSystemId { get; set; }

        public ProviderVendorSystemModel(string providerVendorId, string providerSystemId)
        {
            ProviderVendorId = providerVendorId;
            ProviderSystemId = providerSystemId;
        }

    }
}
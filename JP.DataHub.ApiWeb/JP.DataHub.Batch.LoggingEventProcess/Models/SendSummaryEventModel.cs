using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Models
{
    public class SendSummaryEventModel
    {
        public string ApiId { get; set; }

        public string ControllerId { get; set; }

        public string ProviderSystemId { get; set; }

        public string ProviderVendorId { get; set; }

        public string SystemId { get; set; }

        public string VendorId { get; set; }

        public DateTime RequestDate { get; set; }

    }
}

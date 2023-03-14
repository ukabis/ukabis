using JP.DataHub.Com.TimeZone;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class SummaryEventModel
    {
        public string ApiId { get; set; }

        public string ControllerId { get; set; }

        public string ProviderSystemId { get; set; }

        public string ProviderVendorId { get; set; }

        public string SystemId { get; set; }

        public string VendorId { get; set; }

        public DateTime RequestDate { get; set; }

        public SummaryEventModel(string apiId, string controllerId, string providerSystemId, string providerVendorId, string systemId, string vendorId, DateTime requestDate)
        {
            ApiId = apiId;
            ControllerId = controllerId;
            ProviderSystemId = providerSystemId;
            ProviderVendorId = providerVendorId;
            SystemId = systemId;
            VendorId = vendorId;
            RequestDate = requestDate.ConvertToJst();
        }

    }
}

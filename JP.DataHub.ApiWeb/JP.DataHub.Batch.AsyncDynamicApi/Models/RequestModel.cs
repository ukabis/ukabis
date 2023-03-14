using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.Batch.AsyncDynamicApi.Models
{
    public class RequestModel
    {
        public string RequestId { get; set; }

        public string Url { get; set; }

        public string MethodType { get; set; }

        public string ActionType { get; set; }
        public string Accept { get; set; }


        public PerRequestDataContainer PerRequestDataContainer { get; set; }

        public string QueryString { get; set; }

        public string RequestBody { get; set; }

    }
}

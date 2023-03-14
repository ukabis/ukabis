using System.Net;
using System.Collections.Generic;

namespace JP.DataHub.Batch.AsyncDynamicApi.Models
{
    public class AsyncResultHeaderModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string MediaType { get; set; }
        public string CharSet { get; set; }
        public IDictionary<string, List<string>> HttpHeaders { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Async
{
    // .NET6
    internal class AsyncResultHeaderModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string MediaType { get; set; }
        public string CharSet { get; set; }
        public IDictionary<string, List<string>> HttpHeaders { get; set; }
    }
}

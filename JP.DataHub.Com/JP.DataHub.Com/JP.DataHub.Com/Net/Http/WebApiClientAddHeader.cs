using System.Collections.Generic;

namespace JP.DataHub.Com.Net.Http
{
    public class WebApiClientAddHeader : IWebApiClientAddHeader
    {
        public Dictionary<string, string[]> Header { get; set; } = new Dictionary<string, string[]>();
    }
}

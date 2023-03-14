using System.Collections.Generic;

namespace JP.DataHub.Com.Net.Http
{
    public interface IWebApiClientAddHeader
    {
        Dictionary<string, string[]> Header { get; set; }
    }
}

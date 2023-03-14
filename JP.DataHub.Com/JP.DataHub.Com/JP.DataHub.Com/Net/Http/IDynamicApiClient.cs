using System.Collections.Generic;
using System.Net.Http;
using Polly.Retry;

namespace JP.DataHub.Com.Net.Http
{
    public interface IDynamicApiClient : IWebApiClient
    {
        WebApiResponseResult GetWebApiResponseResult(WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null);
        WebApiResponseResult GetWebApiResponseResult(WebApiRequestModel req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null);
        WebApiResponseResult<T> GetWebApiResponseResult<T>(WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null) where T : new();
        WebApiResponseResult<T> GetWebApiResponseResult<T>(WebApiRequestModel<T> req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null) where T : new();
    }
}

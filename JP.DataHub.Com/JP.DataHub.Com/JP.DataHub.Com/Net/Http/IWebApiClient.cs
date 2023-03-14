using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly.Retry;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Net.Http
{
    public interface IWebApiClient
    {
        IServerEnvironment ServerEnvironment { get; set; }
        IAuthenticationInfo AuthenticationInfo { get; set; }
        IAuthenticationResult AuthenticationResult { get; set; }

        void SwitchAuthentication(IAuthenticationInfo authenticationInfo);
        void SwitchAuthenticationResult(IAuthenticationResult authenticationResult);
        HttpResponseMessage Request(WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null);
        HttpResponseMessage Request(WebApiRequestModel req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null);
        HttpResponseMessage Request<T>(WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null);
        HttpResponseMessage Request<T>(WebApiRequestModel<T> req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null);
    }
}

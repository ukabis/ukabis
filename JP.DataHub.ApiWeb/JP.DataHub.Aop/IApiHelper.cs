using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Aop
{
    public interface IApiHelper
    {
        //HttpResponseMessage ExecuteApi(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam);
        //T ExecuteApiToObject<T>(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam) where T : class;
        HttpResponseMessage ExecuteGetApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null);
        Task<HttpResponseMessage> ExecuteGetApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null);
        T ExecuteGetApiToObject<T>(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) where T : class;
        HttpResponseMessage ExecutePostApi(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null);
        Task<HttpResponseMessage> ExecutePostApiAsync(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null);
        HttpResponseMessage ExecutePostApi(string url, Stream contents, string querystring = null, Dictionary<string, List<string>> headers = null);
        Task<HttpResponseMessage> ExecutePostApiAsync(string url, Stream contents, string querystring = null, Dictionary<string, List<string>> headers = null);
        HttpResponseMessage ExecutePutApi(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null);
        Task<HttpResponseMessage> ExecutePutApiAsync(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null);
        HttpResponseMessage ExecuteDeleteApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null);
        Task<HttpResponseMessage> ExecuteDeleteApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null);
        HttpResponseMessage ExecutePatchApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null);
        Task<HttpResponseMessage> ExecutePatchApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null);
    }
}

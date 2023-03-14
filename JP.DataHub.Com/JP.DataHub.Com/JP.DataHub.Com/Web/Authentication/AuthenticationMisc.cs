using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    internal static class AuthenticationMisc
    {
        public static HttpResponseResult<T> Post<T>(string requestUri, Stream content, string contentType, Dictionary<string, string> header = null)
        {
            var client = new HttpClient();
            // emptyやnullの場合にExceptionが出るために抑制する
            try
            {
                var response = client.PostAsync(requestUri, CreateContents(content, contentType)).Result;
                return new HttpResponseResult<T>(response);
            }
            catch (Exception e)
            {
                return new HttpResponseResult<T>(e);
            }
        }

        public static StreamContent CreateContents(Stream content, string contentType = null)
        {
            var streamContents = new StreamContent(content);
            streamContents.Headers.Remove("Content-Type");
            streamContents.Headers.TryAddWithoutValidation("Content-Type", contentType);
            return streamContents;
        }
    }
}

using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.MockClass
{
    public class MoqHttpClient : DelegatingHandler
    {
        private string response;

        public MoqHttpClient(string _response)
        {
            this.response = _response;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            var content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(response)));
            var resMsg = new HttpResponseMessage();
            resMsg.Content = content;
            resMsg.StatusCode = HttpStatusCode.OK;
            return resMsg;

        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.MockClass
{
    public class MoqHttpClient : DelegatingHandler
    {
        public int Sequence { get; set; } = 0;
        public MoqResponse[] MoqResponses { get; set; }

        public MoqHttpClient()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            var response = MoqResponses[Sequence];
            Sequence++;

            var content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(response.Content)));
            var resMsg = new HttpResponseMessage();
            resMsg.Content = content;
            resMsg.StatusCode = response.HttpStatusCode;
            await Task.Run(() => Task.CompletedTask);
            return resMsg;
        }
    }

    public class MoqResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Content { get; set; }
    }
}

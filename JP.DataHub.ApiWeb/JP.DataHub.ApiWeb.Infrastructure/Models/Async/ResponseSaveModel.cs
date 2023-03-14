using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Async
{
    internal class ResponseSaveModel
    {
        //https://github.com/aspnet/AspNetWebStack/blob/master/src/System.Net.Http.Formatting/HttpContentMessageExtensions.cs を参考に決定
        private const int DefaultBufferSize = 32 * 1024;

        [JsonIgnore]
        public HttpResponseMessage Message
        {
            get
            {
                if (Base64SeralizedHttpResponseMessage == null)
                {
                    return null;
                }
                var response = new HttpResponseMessage();
                response.Content = new ByteArrayContent(Convert.FromBase64String(Base64SeralizedHttpResponseMessage));
                response.Content.Headers.Add("Content-Type", "application/http;msgtype=response");
                return response;
            }
        }

        public string Base64SeralizedHttpResponseMessage { get; set; }

        public long? ContentLength { get; set; }


        public ResponseSaveModel(HttpResponseMessage message)
        {
            if (message == null)
            {
                Base64SeralizedHttpResponseMessage = null;
                return;
            }
            ContentLength = message.Content?.Headers?.ContentLength ?? 0;
            Base64SeralizedHttpResponseMessage = Convert.ToBase64String(message.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult());
        }

        public ResponseSaveModel(string message, long? contentLength)
        {
            Base64SeralizedHttpResponseMessage = message;
            ContentLength = contentLength;
        }

        public ResponseSaveModel()
        {
        }
    }
}

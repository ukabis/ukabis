using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Exceptions
{
    public class AopResponseException : Exception
    {
        public HttpResponseMessage Response { get; }

        public AopResponseException()
        {
        }

        public AopResponseException(HttpResponseMessage response)
        {
            Response = response;
        }

        public AopResponseException(HttpStatusCode statusCode)
        {
            Response = new HttpResponseMessage(statusCode);
        }

        public AopResponseException(HttpStatusCode statusCode, HttpContent httpContent)
        {
            Response = new HttpResponseMessage(statusCode) { Content = httpContent };
        }

        public AopResponseException(HttpStatusCode statusCode, string content)
        {
            Response = new HttpResponseMessage(statusCode) { Content = content.ToStreamContent() };
        }
    }
}

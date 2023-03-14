using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static WebApiResponseResult<T> ToWebApiResponseResult<T>(this HttpResponseMessage msg) where T : new()
        {
            return new WebApiResponseResult<T>(msg);
        }

        public static WebApiResponseResult ToWebApiResponseResult(this HttpResponseMessage msg, Type type)
        {
            return new WebApiResponseResult(msg, type);
        }

        public static WebApiResponseResult ToWebApiResponseResult(this HttpResponseMessage msg)
        {
            return new WebApiResponseResult(msg);
        }

        public static HttpResponseMessage ToHttpResponseMessage(this HttpStatusCode code, object model = null)
            => new HttpResponseMessage() { StatusCode = code, Content = model.ToJsonString()?.ToStreamContent() };

        public static HttpResponseMessage ToHttpResponseMessage(this object model, HttpStatusCode code = HttpStatusCode.OK)
            => new HttpResponseMessage() { StatusCode = code, Content = model?.ToJsonString()?.ToStreamContent() };
    }
}
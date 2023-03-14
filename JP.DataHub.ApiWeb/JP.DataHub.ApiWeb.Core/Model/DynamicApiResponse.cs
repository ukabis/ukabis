using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Core.Model
{
    [MessagePackObject]
    [Serializable]
    public record DynamicApiResponse : IValueObject, IDisposable
    {
        [MessagePack.Key(0)]
        public HttpStatusCode StatusCode { get; }
        [Required]
        [MessagePack.Key(1)]
        public HttpHeaderModel Headers { get; }
        [MessagePack.Key(2)]
        public DynamicApiReponseContents Contents { get; }
        [MessagePack.Key(3)]
        public string ReasonPhrase { get; }

        public DynamicApiResponse(HttpResponseMessage message)
        {
            StatusCode = message.StatusCode;
            Headers = message.Headers.ToDomainHttpHeader();
            Contents = message.Content.Headers.ContentType is not null
                ? new DynamicApiReponseContents(message.Content, message.Headers.TransferEncoding.Any())
                : null;
            ReasonPhrase = message.ReasonPhrase;
            ValidatorEx.ExceptionValidateObject(this);//System.Net.Http.EmptyContent
        }

        public DynamicApiResponse(HttpStatusCode statusCode, DynamicApiReponseContents contents, Dictionary<string, List<string>> headers, string reasonPhrase = null)
        {
            StatusCode = statusCode;
            Headers = new HttpHeaderModel(headers);
            Contents = contents;
            ReasonPhrase = reasonPhrase;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public DynamicApiResponse(HttpStatusCode statusCode = HttpStatusCode.OK, DynamicApiReponseContents contents = null, HttpHeaderModel headers = null, string reasonPhrase = null)
        {
            StatusCode = statusCode;
            Headers = headers ?? new HttpHeaderModel();
            Contents = contents;
            ReasonPhrase = reasonPhrase;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public HttpResponseMessage ToHttpResponseMessage()
        {
            var res = new HttpResponseMessage() { StatusCode = StatusCode, ReasonPhrase = ReasonPhrase, Content = Contents?.ToSystemHttpContent() };
            Headers.ToList().ForEach(x => res.Headers.Add(x.Key, x.Value));
            return res;
        }



        public static bool operator ==(DynamicApiResponse me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DynamicApiResponse me, object other) => !me?.Equals(other) == true;

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Contents?.Dispose();
            }
        }

        //~DynamicApiResponse()
        //{
        //    Dispose(false);
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Core.Model;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    [MessagePackObject]
    public record HttpHeader : IValueObject, ISerializable
    {
        public Dictionary<string, List<string>> Dic { get; internal set; }

        public HttpHeader()
            : base()
        {
        }

        public HttpHeader(IDictionary<string, List<string>> keyValuePairs)
        {
            Dic = new Dictionary<string,List<string>>(keyValuePairs);
        }

        public HttpHeader(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Dic.GetObjectData(info, context);
        }

        public override int GetHashCode() => this.PropertiesGetHashCode();

        public override string ToString() => Dic.Count < 1 ? "" : string.Join(':', Dic.Select(x => $"Key:{x.Key} Value:{string.Join(',', x.Value)}"));

        public static bool operator ==(HttpHeader me, object other) => me?.Equals(other) == true;

        public static bool operator !=(HttpHeader me, object other) => !me?.Equals(other) == true;
    }

    internal static class HttpHeaderExtensions
    {
        public static HttpHeader ToHttpHeaderValueObject(this IHeaderDictionary httpHeader)
        {
            return new HttpHeader(httpHeader.ToDictionary(x => x.Key, y => y.Value.ToList()));
        }

        public static HttpHeader ToHttpHeaderValueObject(this HttpHeaderModel httpHeader)
        {
            return new HttpHeader(httpHeader?.ToDictionary(x => x.Key, y => y.Value.ToList()) ?? new Dictionary<string, List<string>>());
        }
    }
}

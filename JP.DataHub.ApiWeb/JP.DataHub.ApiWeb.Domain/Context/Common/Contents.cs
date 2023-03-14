using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Converter;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class Contents : IDisposable, IValueObject
    {

        public Stream Value { get; }
        public string ContentType { get; }
        public Encoding TextEncoding { get; }

        private string ReadString;
        private bool IsReadCompleted = false;

        public Contents(Stream value, string contentType = null, string encode = null, Func<long, Stream, long> callback = null)
        {
            this.Value = value;
            this.ContentType = contentType;
            TextEncoding = string.IsNullOrWhiteSpace(encode) ? Encoding.UTF8 : Encoding.GetEncoding(encode);
        }

        public Contents(string value)
        {
            TextEncoding = Encoding.UTF8;
            this.Value = string.IsNullOrEmpty(value) ? Stream.Null : new MemoryStream(TextEncoding.GetBytes(value));
        }

        public string ReadToString() => ReadToStringAsync().Result;

        public async ValueTask<string> ReadToStringAsync()
        {
            if (IsReadCompleted)
            {
                return ReadString;
            }
            else
            {
                if (Value == null || !Value.CanRead)
                {
                    return "";
                }
                using (StreamReader reader = new StreamReader(Value))
                {
                    var result = await reader.ReadToEndAsync();
                    IsReadCompleted = true;
                    ReadString = result;
                    return result;
                }
            }
        }

        public Tuple<bool, Contents> ConvertContents(MediaType mediaType, DataSchema requestSchema, IsArray isArray)
        {
            var retContent = this;
            if (mediaType.IsXml)
            {
                var stringContents = ReadToString();
                if (string.IsNullOrEmpty(stringContents) == false)
                {
                    var converter = new JsonXmlConverter();
                    var json = converter.XmlToJson(stringContents,UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:XmlNamespace"));
                    retContent = new Contents(new MemoryStream(Encoding.UTF8.GetBytes(json)));
                }
            }
            else if (mediaType.IsCsv)
            {
                var stringContents = ReadToString();
                if (string.IsNullOrEmpty(stringContents) == false)
                {
                    var converter = new JsonCsvConverter();
                    var json = converter.CsvToJson(stringContents, requestSchema?.ToJSchema(), isArray.Value);
                    retContent = new Contents(new MemoryStream(Encoding.UTF8.GetBytes(json)));
                }
            }

            return Tuple.Create<bool, Contents>(mediaType.IsXml, retContent);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Streamが閉じられていた場合はDisposeが行えないのでExceptionが発生する
                try
                {
                    Value?.Dispose();
                }
                catch
                {
                }
            }
        }

        ~Contents()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

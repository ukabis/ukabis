using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MessagePack;
using JP.DataHub.Com.Validations;
using JP.DataHub.Com.Validations.Annotations;
using JP.DataHub.Com.Validations.Attributes;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Core.Model
{
    [MessagePackObject]
    [Serializable]
    public class DynamicApiReponseContents : IDisposable, ISerializable
    {
        [MessagePack.Key(0)]
        public string StringContents { get; set; }

        public Stream Stream { get; set; }

        public bool IsStreamContents { get => (Stream != null && string.IsNullOrEmpty(StringContents)); }

        [Required]
        public string ContentType { get; }
        public long? ContentLength { get; }
        public string ContentCharset { get; }
        public IDictionary<string, List<string>> HttpHeader { get; private set; }

        public string MediaType => $"{ContentType};{ContentCharset}";


        public DynamicApiReponseContents(string stringContents, string contentType, long? contentLength, string contentCharset, IDictionary<string, List<string>> httpHeader)
        {
            StringContents = stringContents;
            ContentType = contentType;
            ContentLength = contentLength;
            ContentCharset = contentCharset;
            HttpHeader = httpHeader;
        }

        public DynamicApiReponseContents(string contents, string contentType, Encoding charset = null)
        {
            StringContents = contents ?? throw new ArgumentNullException(nameof(contents));
            ContentType = contentType;
            ContentCharset = (charset ?? Encoding.UTF8).WebName;
            using (var stream = new MemoryStream((charset ?? Encoding.UTF8).GetBytes(StringContents)))
            {
                ContentLength = stream.Length;
            }
            ValidatorEx.ExceptionValidateObject(this);
        }

        public DynamicApiReponseContents(Stream streamContents, string contentType = null, Encoding charset = null,
            bool isChunk = false)
        {
            Stream = streamContents;
            ContentLength = isChunk ? null : streamContents.Length;
            ContentType = contentType ?? MediaTypeConst.ApplicationJson;
            ContentCharset = (charset ?? Encoding.UTF8).WebName;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public DynamicApiReponseContents(HttpContent content, bool isChunk = false)
        {
            Stream = content.ReadAsStreamAsync().Result;
            ContentType = content.Headers?.ContentType?.MediaType ?? MediaTypeConst.ApplicationJson;
            ContentCharset = content.Headers?.ContentType?.CharSet ?? Encoding.UTF8.WebName;
            ContentLength = isChunk ? null : content.Headers?.ContentLength;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public string ReadAsString()
        {
            return IsStreamContents ? Stream.ReadToEnd() : StringContents;
        }

        public Stream ReadAsStream()
        {
            return IsStreamContents ? Stream : new MemoryStream(Encoding.UTF8.GetBytes(StringContents));
        }

        public HttpContent ToSystemHttpContent()
        {
            var content = new StreamContent(ReadAsStream());
            content.Headers.Clear();
            SystemHttpHeader().ToList().ForEach(x => content.Headers.Add(x.Key, x.Value));

            return content;
        }

        public IDictionary<string, List<string>> SystemHttpHeader()
        {
            var result = HttpHeader ?? new Dictionary<string, List<string>>();
            if (ContentLength != null)
            {
                result.Merge(new KeyValuePair<string, List<string>>(HeaderConst.ContentLength, new List<string>() { ContentLength.ToString() }));
            }

            result.Merge(new KeyValuePair<string, List<string>>(HeaderConst.ContentType, new List<string>() { MediaType }));
            return result;
        }


        #region ISerializable Implementation

        public DynamicApiReponseContents(SerializationInfo info, StreamingContext context)
        {
            StringContents = info.GetString(nameof(StringContents));
            Stream = new MemoryStream(info.GetValue(nameof(Stream), typeof(byte[])) as byte[]);
            ContentType = info.GetString(nameof(ContentType));
            ContentLength = info.GetValue(nameof(ContentLength), typeof(long?)) as long?;
            ContentCharset = info.GetString(nameof(ContentCharset));
            HttpHeader = info.GetValue(nameof(HttpHeader), typeof(IDictionary<string, List<string>>)) as IDictionary<string, List<string>>;
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(StringContents), StringContents);
            info.AddValue(nameof(Stream), Stream.ToByteArray());
            info.AddValue(nameof(ContentType), ContentType);
            info.AddValue(nameof(ContentLength), ContentLength);
            info.AddValue(nameof(ContentCharset), ContentCharset);
            info.AddValue(nameof(HttpHeader), HttpHeader);
        }

        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Stream?.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

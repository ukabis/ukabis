using System.Net;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Streams;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    [MessagePackFormatter(typeof(GateWayResponseFormatter))]
    class GatewayResponse : IValueObject
    {
        private static readonly int s_axSaveApiResponseCacheSize = UnityCore.Resolve<int>("MaxSaveApiResponseCacheSize");

        public HttpResponseMessage Message
        {
            get
            {
                if (_maked != null)
                {
                    return _maked;
                }

                if (_isCache)
                {
                    if (Base64SeralizedHttpResponseMessage == null && _bufferStream == null)
                    {
                        return null;
                    }

                    var response = new HttpResponseMessage();
                    if (_bufferStream != null)
                    {
                        response.Content = new StreamContent(_bufferStream);
                        CopyHeaders(_sourceMessage, response);
                        response.StatusCode = _sourceMessage.StatusCode;
                    }
                    else
                    {
                        response.StatusCode = StatusCode;
                        foreach (var h in Headers)
                        {
                            response.Headers.TryAddWithoutValidation(h.Key, h.Value);
                        }
                        response.Content = new ByteArrayContent(Convert.FromBase64String(Base64SeralizedHttpResponseMessage));
                        response.Content.Headers.Add("Content-Type", "application/http;msgtype=response");
                    }

                    _maked = response;
                    return response;
                }
                else
                {
                    return _message;
                }
            }
        }

        public string Base64SeralizedHttpResponseMessage { get; }

        public HttpStatusCode StatusCode { get; }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; }

        public long? ContentLength { get; }


        private Stream _bufferStream;
        private bool _isCache = false;
        private HttpResponseMessage _message;
        private HttpResponseMessage _sourceMessage;
        private HttpResponseMessage _maked;


        public GatewayResponse(HttpResponseMessage message, bool isCache = false)
        {
            _sourceMessage = message;
            _isCache = isCache;
            if (message == null)
            {
                Base64SeralizedHttpResponseMessage = null;
                return;
            }
            if (isCache)
            {
                //チャンクの場合ContentLengthは入っていない
                ContentLength = message.Content?.Headers?.ContentLength ?? 0;
                // 最初にMaxCacheSaveSize(既定1MB)読んでみる。読めるということはサイズがでかい可能性があるため
                // 後続のデータは読まない（ストリームのままとする）
                // 最初に読んだものをStreamに詰め、残りのストリームと合わせて、DoubleStreamを作り、HttpResponseMessageを作る際に
                // ２つのStreamから合成する
                var stream = message.Content.ReadAsStreamAsync().Result;
                byte[] data = new byte[s_axSaveApiResponseCacheSize];
                var ms = new MemoryStream();
                int totalBytesRead = 0;
                bool isCacheSizeOver = true;
                while (totalBytesRead < data.Length)
                {
                    int bytesRead = stream.Read(data, totalBytesRead, data.Length - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        isCacheSizeOver = false;
                        break;
                    }
                    totalBytesRead += bytesRead;
                }
                if (isCacheSizeOver)
                {
                    ms.Write(data, 0, s_axSaveApiResponseCacheSize);
                    ms.Seek(0, SeekOrigin.Begin);
                    _bufferStream = new DoubleSteam(ms, stream);
                }
                else
                {
                    StatusCode = message.StatusCode;
                    Headers = message.Headers;

                    HttpResponseMessage tmpHttpResponseMessage = new HttpResponseMessage();
                    ms.Write(data, 0, totalBytesRead);
                    ms.Seek(0, SeekOrigin.Begin);
                    tmpHttpResponseMessage.Content = new StreamContent(ms);
                    tmpHttpResponseMessage.Content.Headers.ContentLength = totalBytesRead;
                    CopyHeaders(message, tmpHttpResponseMessage);
                    ContentLength = totalBytesRead;
                    Base64SeralizedHttpResponseMessage = Convert.ToBase64String(tmpHttpResponseMessage.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult());
                }
            }
            else
            {
                _message = message;
            }
        }

        public GatewayResponse(HttpStatusCode statusCode, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, string message, long? contentLength)
        {
            _isCache = true;
            StatusCode = statusCode;
            Headers = headers;
            Base64SeralizedHttpResponseMessage = message;
            ContentLength = contentLength;
        }

        public bool IsSaveCache()
        {
            return _isCache && _bufferStream == null;
        }

        private void CopyHeaders(HttpResponseMessage source, HttpResponseMessage dest)
        {
            foreach (var h in source.Headers)
            {
                dest.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
            foreach (var h in source.Content.Headers)
            {
                dest.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
        }
    }

    class GateWayResponseFormatter : IMessagePackFormatter<GatewayResponse>
    {
        public GatewayResponse Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            var statusCode = (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), reader.ReadInt32());
            var headers = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, IEnumerable<string>>>>(reader.ReadString());
            var message = reader.ReadString();
            long? contentLength = null;
            if (!reader.TryReadNil())
            {
                contentLength = reader.ReadInt64();
            }

            if (message == "null")
            {
                return new GatewayResponse(statusCode, headers, null, contentLength);
            }
            else
            {
                return new GatewayResponse(statusCode, headers, message, contentLength);
            }
        }

        public void Serialize(ref MessagePackWriter writer, GatewayResponse value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteInt32((int)value.StatusCode);
            writer.WriteString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value.Headers)));
            writer.WriteString(Encoding.UTF8.GetBytes(value.Base64SeralizedHttpResponseMessage?.ToString() ?? "null"));
            if (value.ContentLength.HasValue)
            {
                writer.WriteInt64(value.ContentLength.Value);
            }
            else
            {
                writer.WriteNil();
            }
        }
    }
}

using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using MessagePack;
using MessagePack.Formatters;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Log;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    [MessagePackObject]
    [MessagePackFormatter(typeof(JsonSearchResultFormatter))]

    internal class JsonSearchResult : IEntity, IDisposable
    {
        private readonly object streamReadToEndLock = new object();
        [Key(0)]
        public int Count { get; private set; }

        [Key(1)]
        public string Value
        {
            get
            {
                if (!isClose)
                {
                    throw new InvalidOperationException("Data not Close");
                }
                if (_valueCache == null)
                {
                    lock (streamReadToEndLock)
                    {
                        _valueCache = StreamReadToEnd();
                    }
                }
                return _valueCache;
            }
        }

        private string StreamReadToEnd()
        {
            stream.Position = 0;
            return reader.ReadToEnd();
        }

        [IgnoreMember]
        public JToken JToken
        {
            get
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    return JToken.Parse(Value);
                }
                return null;
            }

        }

        [IgnoreMember]
        public Stream Stream
        {
            get
            {
                stream.Position = 0;
                return stream;
            }
        }

        [IgnoreMember]
        private StreamWriter writer;
        [IgnoreMember]
        private MemoryStream stream = new MemoryStream();
        [IgnoreMember]
        private StreamReader reader;
        [IgnoreMember]
        private bool isClose = false;
        [IgnoreMember]
        private bool isArray = false;
        [IgnoreMember]
        private bool isQuery = false;
        [IgnoreMember]
        private bool arrayWrite = false;
        [IgnoreMember]
        private StringBuilder firstBufferBuilder = new StringBuilder();

        [IgnoreMember]
        public bool IsQuery { get => isQuery; }
        [IgnoreMember]
        public bool IsArray { get => isArray; }

        [IgnoreMember]
        public string _valueCache;

        public JsonSearchResult(JsonSearchResult src)
        {
            isQuery = src.isQuery;
            isArray = src.isArray;
        }

        public JsonSearchResult(ApiQuery query, PostDataType postDataType, ActionTypeVO actionType)
        {
            if ((postDataType?.Value ?? "").ToLower() == "array" || actionType.Value == ActionType.DeleteData)
            {
                //DeleteActionの場合はpostTypeを持っていないためarrayで返してあげる
                isArray = true;
            }

            if (!string.IsNullOrEmpty(query?.Value ?? ""))
            {
                isQuery = true;
            }
        }

        public JsonSearchResult(string json, int count)
        {
            isClose = true;
            Count = count;
            writer = new StreamWriter(stream, new UTF8Encoding(false));
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;
            reader = new StreamReader(stream);
        }

        public void BeginData()
        {
            writer = new StreamWriter(stream, new UTF8Encoding(false));
            if (isArray)
            {
                writer.Write("[");
                arrayWrite = true;
            }
        }

        public void AddString(string value)
        {
            if (isClose)
            {
                throw new InvalidOperationException("Data Already Close Not Add");
            }

            Count++;
            if (Count == 1)
            {
                firstBufferBuilder.Append(value);
            }

            if (Count == 2)
            {
                if (isQuery && !isArray)
                {
                    writer.Write("[");
                    arrayWrite = true;
                }
                writer.Write(firstBufferBuilder.ToString());
                firstBufferBuilder.Clear();
                writer.Write(",");
                writer.Write(value);
                //ここに来たということはデータは配列になっている
                isArray = true;
            }
            if (Count > 2)
            {
                writer.Write(",");
                writer.Write(value);
            }
        }


        public void EndData()
        {
            if (isClose)
            {
                throw new InvalidOperationException("Data Already Close");
            }

            if (firstBufferBuilder.Length > 0)
            {
                writer.Write(firstBufferBuilder.ToString());
                firstBufferBuilder.Clear();
            }
            if (arrayWrite)
            {
                writer.Write("]");
            }
            writer.Flush();
            isClose = true;
            stream.Position = 0;
            reader = new StreamReader(stream);
        }


        [Key(12)]
        private bool disposedValue = false;

        /// <summary>
        /// 使用中のフラグを変更する
        /// </summary>
        /// <param name="isUse">true:使用中、false:使用済み</param>
        public void InUse(bool isUse)
        {
            lock (usingCounterLock)
            {
                if (isUse)
                {
                    usingCounter++;
                }
                else
                {
                    usingCounter--;
                }
            }
        }

        /// <summary>
        /// 使用中カウンター。
        /// このカウンターが1以上の場合はDisposeされない。
        /// 非同期処理等で使用する場合に+1する。終わったら-1するのを忘れないように。
        /// </summary>
        private int usingCounter = 0;
        private readonly object usingCounterLock = new object();

        protected virtual async Task Dispose(bool disposing)
        {
            while (usingCounter > 0)
            {
                await Task.Delay(1000);
            }

            var log = new JPDataHubLogger(this.GetType());

            if (!disposedValue)
            {
                if (stream != null)
                {
                    //Streamが閉じられていた場合はDisposeが行えないのでExceptionが発生する
                    try
                    {
                        stream.Dispose();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    stream = null;
                }

                if (reader != null)
                {
                    try
                    {
                        reader.Dispose();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    reader = null;
                }

                if (writer != null)
                {
                    try
                    {
                        writer.Dispose();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    writer = null;
                }

                disposedValue = true;
            }
        }

        // 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        ~JsonSearchResult()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            GC.SuppressFinalize(this);
        }
    }


    class JsonSearchResultFormatter : IMessagePackFormatter<JsonSearchResult>
    {
        public JsonSearchResult Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            var value = reader.ReadString();
            if (value == AbstractCache.NullValue && reader.End)
            {
                return null;
            }
            var count = 1;
            if (value.TrimStart().StartsWith("["))
            {
                var json = JArray.Parse(value);
                count = json.Count;
            }

            return new JsonSearchResult(value, count);
        }

        public void Serialize(ref MessagePackWriter writer, JsonSearchResult value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteString(Encoding.UTF8.GetBytes(value.Value));
            writer.WriteInt32(value.Count);
        }
    }
}
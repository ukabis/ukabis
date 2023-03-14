using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Converter;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.ParallelOptions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using Microsoft.Extensions.Configuration;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    [CacheKey]
    internal abstract class AbstractDynamicApiAction : IDynamicApiAction
    {
        protected JPDataHubLogger Logger = new JPDataHubLogger(typeof(AbstractDynamicApiAction));
        protected int MaxRegisterContentLength => UnityCore.Resolve<int>("MaxRegisterContentLength");
        protected int MaxBase64AttachFileContentLength => UnityCore.Resolve<int>("MaxBase64AttachFileContentLength");
        protected int MaxSaveApiResponseCacheSize => UnityCore.Resolve<int>("MaxSaveApiResponseCacheSize");
        protected int ThresholdJsonSchemaValidaitonParallelize => UnityCore.Resolve<int>("ThresholdJsonSchemaValidaitonParallelize");
        protected bool enableThreadingOfReference = UnityCore.Resolve<bool>("EnableThreadingOfReference");

        public static string INTERNAL_UPDATE = "**InternalUpdate**";

        public static string DOCUMENTHISTORY_HEADERNAME = "X-DocumentHistory";

        protected string NULL_KEYWORD = "JP.DATAHUB.NULL_KEYWORD";

        protected string SPACE_HEX = "0x20";

        [CacheKey(CacheKeyType.Id, "vendor_id", "system_id", "controller_id", "api_id", "action_key")]
        public static string CACHE_KEY_API_ACTION = "DynamicApiAction";

        [Dependency]
        public ICache Cache { get; set; }

        protected ICache BlobCache => _lazyBlobCache.Value;
        private Lazy<ICache> _lazyBlobCache = new Lazy<ICache>(() =>
        {
            return UnityCore.Resolve<ICache>("DynamicApiBlobCache");
        });

        protected IPerRequestDataContainer PerRequestDataContainer { get => _lazyPerRequestDtaContainer.Value; }
        private Lazy<IPerRequestDataContainer> _lazyPerRequestDtaContainer = new Lazy<IPerRequestDataContainer>(() =>
        {
            return UnityCore.Resolve<IPerRequestDataContainer>();
        });

        protected bool ReturnJsonValidatorErrorDetail { get => _returnJsonValidatorErrorDetail.Value; }
        protected Lazy<bool> _returnJsonValidatorErrorDetail = new Lazy<bool>(() => UnityCore.Resolve<bool>("Return.JsonValidator.ErrorDetail"));

        protected const string MEDIATYPE_JSON = "application/json";
        protected const string MEDIATYPE_XML = "application/xml";
        protected const string MEDIATYPE_GEO_JSON = "application/geo+json";
        protected const string MEDIATYPE_VND_GEO_JSON = "application/vnd.geo+json";
        protected const string MEDIATYPE_TEXTXML = "text/xml";
        protected const string MEDIATYPE_CSV = "text/csv";
        protected const string MEDIATYPE_ProblemJson = "application/problem+json";
        protected const string MEDIATYPE_ProblemXml = "application/problem+xml";
        protected const string SEPARATOR = "~";

        protected const string TYPE = "_Type";
        protected const string VENDORID = "_Vendor_Id";
        protected const string SYSTEMID = "_System_Id";
        protected const string REGUSERID = "_Reguser_Id";
        protected const string REGDATE = "_Regdate";
        protected const string UPDUSERID = "_Upduser_Id";
        protected const string UPDDATE = "_Upddate";
        protected const string VERSION_COLNAME = "_Version";
        protected const string PARTITIONKEY = "_partitionkey";
        protected const string VERSION_VALUE = "version";
        protected const string OWNERID = "_Owner_Id";
        protected const string ID = "id";
        protected const string ETAG = "_etag";

        protected const string Base64EncordString = "$Base64";
        protected const string Base64DecordFilePath = "$Base64Reference";
        protected const string Base64BlobPrefix = "attachfilebase64";


        public ActionId ActionId { get; set; }

        public ApiId ApiId { get; set; }

        public ControllerId ControllerId { get; set; }

        public ControllerUrl ControllerRelativeUrl { get; set; }

        public HttpMethodType MethodType { get; set; }

        public DataSchema RequestSchema { get; set; }

        public DataSchema UriSchema { get; set; }


        public DataSchema ControllerSchema { get; set; }

        public DataSchema ResponseSchema { get; set; }

        public ReadOnlyCollection<RepositoryInfo> RepositoryInfo { get; set; }

        public IsOpenIdAuthentication IsOpenIdAuthentication { get; set; }

        public RepositoryKey RepositoryKey { get; set; }

        public RepositoryKey ControllerRepositoryKey { get; set; }

        public UrlParameter KeyValue { get; set; }

        public QueryStringVO Query { get; set; }

        public QueryType QueryType { get; set; }

        public PostDataType PostDataType { get; set; }

        public Contents Contents { get; set; }

        public RelativeUri RelativeUri { get; set; }

        public ApiUri ApiUri { get; set; }

        public ApiQuery ApiQuery { get; set; }

        public IsVendor IsVendor { get; set; }

        public IsPerson IsPerson { get; set; }

        public VendorId ProviderVendorId { get; set; }
        public SystemId ProviderSystemId { get; set; }
        public VendorId VendorId { get; set; }

        public SystemId SystemId { get; set; }

        public OpenId OpenId { get; set; }

        public XAdmin Xadmin { get; set; }

        public IsOverPartition IsOverPartition { get; set; }

        public ActionTypeVO ActionType { get; set; }
        public ActionTypeVO AsyncOriginalActionType { get; set; }

        public CacheInfo CacheInfo { get; set; }

        public IsAutomaticId IsAutomaticId { get; set; }

        public ActionTypeVersion ActionTypeVersion { get; set; }

        public ActionInjectorHandler ActionInjectorHandler { get; set; }

        public PartitionKey PartitionKey { get; set; }

        public ApiResourceSharing ApiResourceSharing { get; set; }

        public XResourceSharingPerson XResourceSharingPerson { get; set; }

        public List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; set; }

        public XVersion Xversion { get; set; }

        public XGetInnerField XGetInnerAllField { get; set; }

        public XRequestContinuation XRequestContinuation { get; set; }

        public ReadOnlyCollection<INewDynamicApiDataStoreRepository> DynamicApiDataStoreRepository { get; set; }

        public MediaType MediaType { get; set; }

        public IsOverrideId IsOverrideId { get; set; } = new IsOverrideId(true);
        public Accept Accept { get; set; } = new Accept(null);

        public HasMailTemplate HasMailTemplate { get; set; }

        public HasWebhook HasWebhook { get; set; }

        public IsEnableAttachFile IsEnableAttachFile { get; set; }
        public RepositoryInfo AttachFileBlobRepositoryInfo { get; set; }
        public RepositoryGroupId AttachFileBlobRepositoryGroupId { get; set; }
        public IDynamicApiAttachFileRepository AttachFileDynamicApiDataStoreRepository { get; set; }

        protected IBlockchainEventHubStoreRepository BlockchainEventHubStoreRepository { get => this.IsEnableBlockchain?.Value == true ? UnityCore.Resolve<IBlockchainEventHubStoreRepository>() : null; }

        public IsUseBlobCache IsUseBlobCache { get; set; }

        public IsSkipJsonSchemaValidation IsSkipJsonSchemaValidation { get; set; }

        public IsOptimisticConcurrency IsOptimisticConcurrency { get; set; }
        public XNoOptimistic XNoOptimistic { get; set; }

        public IsContainerDynamicSeparation IsContainerDynamicSeparation { get; set; }

        public IsOtherResourceSqlAccess IsOtherResourceSqlAccess { get; set; }

        public virtual OperationInfo OperationInfo { get; set; }

        public ContentRange ContentRange { get; set; }
        public IsEnableBlockchain IsEnableBlockchain { get; set; }
        public IsDocumentHistory IsDocumentHistory { get; set; }
        public INewDynamicApiDataStoreRepository HistoryEvacuationDataStoreRepository { get; set; }
        public RepositoryInfo BlockchainRepositoryInfo { get; set; }
        protected HttpContext TmpHttpContext { get; set; } // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
        protected IPerRequestDataContainer TmpPerRequestDataContainer { get; set; } // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
        protected HttpContext ForeignKeyHttpContext { get; set; } // JsonSchemaのForeignKeyチェック用
        protected IPerRequestDataContainer ForeignKeyPerRequestDataContainer { get; set; } // JsonSchemaのForeignKeyチェック用
        private Lazy<bool> _enableJsonDocumentHistory = new Lazy<bool>(() => UnityCore.Resolve<bool>("EnableJsonDocumentHistory"));
        protected bool EnableJsonDocumentHistory { get => _enableJsonDocumentHistory.Value; }
        private Lazy<bool> _enableJsonDocumentReference = new Lazy<bool>(() => UnityCore.Resolve<bool>("EnableJsonDocumentReference"));
        protected bool EnableJsonDocumentReference { get => _enableJsonDocumentReference.Value; }
        private Lazy<bool> _enableJsonDocumentKeepRegDate = new Lazy<bool>(() => UnityCore.Resolve<bool>("EnableJsonDocumentKeepRegDate"));
        protected bool EnableJsonDocumentKeepRegDate { get => _enableJsonDocumentKeepRegDate.Value; }

        abstract public HttpResponseMessage ExecuteAction();

        private Lazy<bool> _isEnableUploadContentCheck = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsEnableUploadContentCheck"));
        private Lazy<List<string>> _uploadOK_ContentTypeList = new Lazy<List<string>>(() => UnityCore.Resolve<List<string>>("UploadOK_ContentTypeList"));
        private Lazy<List<string>> _uploadOK_ExtensionList = new Lazy<List<string>>(() => UnityCore.Resolve<List<string>>("UploadOK_ExtensionList"));
        private Lazy<List<string>> _blockContentTypeList = new Lazy<List<string>>(() => UnityCore.Resolve<List<string>>("BlockContentTypeList"));
        private Lazy<List<string>> _blockExtensionList = new Lazy<List<string>>(() => UnityCore.Resolve<List<string>>("BlockExtensionList"));
        private Lazy<bool> _isPriorityHigh_OKList = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsPriorityHigh_OKList"));
        private Lazy<bool> _isUploadOk_NoExtensionFile = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsUploadOk_NoExtensionFile"));

        public bool IsEnableUploadContentCheck { get => _isEnableUploadContentCheck.Value; }
        protected List<string> UploadOK_ContentTypeList { get => _uploadOK_ContentTypeList.Value; }
        protected List<string> UploadOK_ExtensionList { get => _uploadOK_ExtensionList.Value; }
        protected List<string> BlockContentTypeList { get => _blockContentTypeList.Value; }
        protected List<string> BlockExtensionList { get => _blockExtensionList.Value; }
        protected bool IsPriorityHigh_OKList { get => _isPriorityHigh_OKList.Value; }
        protected bool IsUploadOk_NoExtensionFile { get => _isUploadOk_NoExtensionFile.Value; }
        public XUserResourceSharing XUserResourceSharing { get; set; }

        /// <summary>
        /// 初期化のため（派生クラスでオーバーライドする場合は、基底クラスの初期化は呼び出すこと）
        /// </summary>
        public virtual void Initialize()
        {
            if (EnableJsonDocumentReference == false)
            {
                PerRequestDataContainer.IsSkipJsonFormatProtect = true;
            }
        }

        /// <summary>
        /// キャッシュのキーを作成する
        /// </summary>
        protected string CreateCacheKey(INewDynamicApiDataStoreRepository repository, QueryParam queryParam)
        {
            if (CacheInfo.IsCache == false) return null;
            return CacheManager.CreateKey(
                 CreateResourceCacheKey(),
                 this.ApiId.Value,
                 GetCacheKey(repository, queryParam));
        }

        /// <summary>
        /// リソースのキャッシュキーを作成する
        /// </summary>
        protected string CreateResourceCacheKey()
        {
            return CacheManager.CreateKey(CACHE_KEY_API_ACTION, this.ProviderVendorId.Value, this.ProviderSystemId.Value, this.ControllerId.Value);
        }

        /// <summary>
        /// 自身のリソースのキャッシュを削除する
        /// キャッシュサイズが大きい（多い）とタイムアウトになる可能性があるため、処理は非同期で実行
        /// </summary>
        /// <param name="keyCache"></param>
        protected Task RefreshApiResourceCache(string keyCache)
        {
            return Task.Run(() => Cache.RemoveFirstMatch(keyCache));
        }

        protected virtual string GetCacheKey(INewDynamicApiDataStoreRepository repository, QueryParam queryParam) =>
            repository.KeyManagement.GetCacheKey(queryParam, repository.PerRequestDataContainer, repository.ResourceVersionRepository);

        protected HttpResponseMessage TupleToHttpResponseMessage((HttpStatusCode statusCode, JsonSearchResult result) tuple)
        {
            var mediaTypes = Accept.GetResponseMediaType(this.MediaType);
            HttpResponseMessage ret = null;
            foreach (var mediaType in mediaTypes)
            {
                if (mediaType.Value == MEDIATYPE_XML && tuple.result.Stream.Length > 0)
                {
                    var writeStream = new MemoryStream();
                    WriteXml(tuple.result.Stream, writeStream);
                    writeStream.Position = 0;
                    ret = new HttpResponseMessage(tuple.statusCode /*HttpStatusCode*/)
                    { Content = new StreamContent(writeStream) };
                    ret.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType.Value);
                    break;
                }
                else if (mediaType.Value == MEDIATYPE_CSV && tuple.result.Stream.Length > 0)
                {
                    var writeStream = new MemoryStream();
                    if (!WriteCsv(tuple.result.Stream, writeStream))
                    {
                        if (mediaTypes.Count > 1)
                        {
                            continue;
                        }
                        else if (!new HttpResponseMessage(tuple.statusCode).IsSuccessStatusCode)
                        {
                            //エラーメッセージのはずなのでそのまま返す
                            writeStream.Position = 0;
                            ret = new HttpResponseMessage(tuple.statusCode /*HttpStatusCode*/)
                            { Content = new StreamContent(writeStream) };
                            ret.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType.Value);
                            break;
                        }
                        else
                        {
                            throw new NotParseCsvException(DynamicApiMessages.CsvParseFailed);
                        }
                    }
                    writeStream.Position = 0;
                    ret = new HttpResponseMessage(tuple.statusCode /*HttpStatusCode*/)
                    { Content = new StreamContent(writeStream) };
                    ret.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType.Value);
                    break;
                }
                else if ((mediaType.Value == MEDIATYPE_GEO_JSON || mediaType.Value == MEDIATYPE_VND_GEO_JSON) && tuple.result.Stream.Length > 0)
                {
                    var writeStream = new MemoryStream();
                    if (!WriteGeoJson(tuple.result.Stream, writeStream))
                    {
                        if (mediaTypes.Count > 1)
                        {
                            continue;
                        }
                        else
                        {
                            // そのまま返す
                            writeStream.Position = 0;
                            ret = new HttpResponseMessage(tuple.statusCode /*HttpStatusCode*/)
                            { Content = new StreamContent(writeStream) };
                            ret.Content.Headers.ContentType = new MediaTypeHeaderValue(MEDIATYPE_JSON);
                            break;
                        }
                    }
                    writeStream.Position = 0;
                    ret = new HttpResponseMessage(tuple.statusCode /*HttpStatusCode*/)
                    { Content = new StreamContent(writeStream) };
                    ret.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType.Value);
                    break;
                }
                else
                {
                    var tempStream = new MemoryStream();
                    tuple.result.Stream.CopyTo(tempStream);
                    tempStream.Position = 0;
                    ret = new HttpResponseMessage(tuple.statusCode /*HttpStatusCode*/)
                    { Content = new StreamContent(tempStream) };
                    ret.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType.Value);
                    break;
                }
            }

            tuple.result.Dispose();
            ret.Headers.Add("_CanConvert", "true");
            return ret;
        }

        private HttpResponseMessage TupleToHttpResponseMessage(HttpStatusCode Item1, string Item2, Dictionary<string, string> Item3)
        {
            var mediaTypes = Accept.GetResponseMediaType(this.MediaType);
            string responseContent = Item2 ?? "";
            var ret = new HttpResponseMessage();
            foreach (var mediaType in mediaTypes)
            {
                if (mediaType.Value == MEDIATYPE_XML && !string.IsNullOrEmpty(responseContent))
                {
                    using (var writeStream = new MemoryStream())
                    {
                        WriteXml(new MemoryStream(Encoding.UTF8.GetBytes(responseContent)), writeStream);
                        responseContent = Encoding.UTF8.GetString(writeStream.ToArray());
                        if (new HttpResponseMessage(Item1).IsSuccessStatusCode) ret = new HttpResponseMessage(Item1/*HttpStatusCode*/) { Content = new StringContent(responseContent, Encoding.UTF8, mediaType.Value) };
                        else ret = new HttpResponseMessage(Item1/*HttpStatusCode*/) { Content = new StringContent(responseContent, Encoding.UTF8, MEDIATYPE_ProblemXml) };
                        break;
                    }
                }
                else if (mediaType.Value == MEDIATYPE_CSV && !string.IsNullOrEmpty(responseContent))
                {
                    using (var writeStream = new MemoryStream())
                    {
                        if (!WriteCsv(new MemoryStream(Encoding.UTF8.GetBytes(responseContent)), writeStream))
                        {
                            if (mediaTypes.Count > 1)
                            {
                                continue;
                            }
                            else if (!new HttpResponseMessage(Item1).IsSuccessStatusCode)
                            {
                                //エラーメッセージのはずなのでそのまま返す
                                ret = new HttpResponseMessage(Item1/*HttpStatusCode*/) { Content = new StringContent(responseContent, Encoding.UTF8, mediaType.Value) };
                                break;
                            }
                            else
                            {
                                throw new NotParseCsvException(DynamicApiMessages.CsvParseFailed);
                            }
                        }
                        responseContent = Encoding.UTF8.GetString(writeStream.ToArray());
                        ret = new HttpResponseMessage(Item1/*HttpStatusCode*/) { Content = new StringContent(responseContent, Encoding.UTF8, mediaType.Value) };
                        break;
                    }
                }
                else if ((mediaType.Value == MEDIATYPE_GEO_JSON || mediaType.Value == MEDIATYPE_VND_GEO_JSON) && !string.IsNullOrEmpty(responseContent))
                {
                    using (var writeStream = new MemoryStream())
                    {
                        if (!WriteGeoJson(new MemoryStream(Encoding.UTF8.GetBytes(responseContent)), writeStream))
                        {
                            if (mediaTypes.Count > 1)
                            {
                                continue;
                            }
                            else
                            {
                                // そのまま返す
                                ret = new HttpResponseMessage(Item1/*HttpStatusCode*/) { Content = new StringContent(responseContent, Encoding.UTF8, MEDIATYPE_JSON) };
                                break;
                            }
                        }
                        responseContent = Encoding.UTF8.GetString(writeStream.ToArray());
                        ret = new HttpResponseMessage(Item1/*HttpStatusCode*/) { Content = new StringContent(responseContent, Encoding.UTF8, mediaType.Value) };
                        break;
                    }
                }

                ret = new HttpResponseMessage(Item1/*HttpStatusCode*/) { Content = new StringContent(responseContent, Encoding.UTF8, mediaType.Value) };
                break;
            }

            if (Item3 != null)
            {
                foreach (var header in Item3)
                {
                    ret.Headers.Add(header.Key, header.Value);
                }
            }
            ret.Headers.Add("_CanConvert", "true");
            return ret;
        }

        public HttpResponseMessage TupleToHttpResponseMessage(Tuple<HttpStatusCode, string> tuple) => TupleToHttpResponseMessage(tuple.Item1, tuple.Item2, null);

        public HttpResponseMessage TupleToHttpResponseMessage(Tuple<HttpStatusCode, string, Dictionary<string, string>> tuple) => TupleToHttpResponseMessage(tuple.Item1, tuple.Item2, tuple.Item3);

        /// <summary>
        /// 入力ストリームのJSONデータをXMLに変換して出力ストリームに書き込みます。
        /// </summary>
        /// <param name="readStream">入力ストリーム</param>
        /// <param name="writeStream">出力ストリーム</param>
        private void WriteXml(Stream readStream, Stream writeStream)
        {
            using (var reader = new StreamReader(readStream))
            {
                try
                {
                    var converter = new JsonXmlConverter();
                    var xdoc = converter.JsonToXml(reader.ReadToEnd(), UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:XmlNamespace"));
                    var settings = new XmlWriterSettings()
                    {
                        Encoding = new UTF8Encoding(false)
                    };

                    using (XmlWriter xmlWriter = XmlWriter.Create(writeStream, settings))
                    {
                        xdoc.WriteTo(xmlWriter);
                    }
                }
                catch (JsonReaderException)
                {
                    readStream.Seek(0, SeekOrigin.Begin);
                    readStream.CopyTo(writeStream);
                }
            }
        }

        /// <summary>
        /// 入力ストリームのJSONデータをCSVに変換して出力ストリームに書き込みます。
        /// </summary>
        /// <param name="readStream">入力ストリーム</param>
        /// <param name="writeStream">出力ストリーム</param>
        private bool WriteCsv(Stream readStream, Stream writeStream)
        {
            var tempStream = new MemoryStream();
            readStream.CopyTo(tempStream);
            tempStream.Position = 0;
            using (var reader = new StreamReader(tempStream))
            {
                try
                {
                    var converter = new JsonCsvConverter();
                    if (!converter.JsonToCsv(reader.ReadToEnd(), out var csv))
                    {
                        return false;
                    }
                    new MemoryStream(Encoding.UTF8.GetBytes(csv)).WriteTo(writeStream);
                    return true;
                }
                catch (JsonReaderException)
                {
                    readStream.Seek(0, SeekOrigin.Begin);
                    readStream.CopyTo(writeStream);
                    return false;
                }
            }
        }

        /// <summary>
        /// 入力ストリームのJSONデータをGeoJsonに変換して出力ストリームに書き込みます。
        /// </summary>
        /// <param name="readStream">入力ストリーム</param>
        /// <param name="writeStream">出力ストリーム</param>
        private bool WriteGeoJson(Stream readStream, Stream writeStream)
        {
            using (var reader = new StreamReader(readStream))
            {
                try
                {
                    var converter = new JsonGeoJsonConverter();
                    if (!converter.JsonToGeoJson(reader.ReadToEnd(), out var geoJson))
                    {
                        readStream.Seek(0, SeekOrigin.Begin);
                        readStream.CopyTo(writeStream);
                        return false;
                    }
                    new MemoryStream(Encoding.UTF8.GetBytes(geoJson)).WriteTo(writeStream);
                    return true;
                }
                catch (JsonReaderException)
                {
                    readStream.Seek(0, SeekOrigin.Begin);
                    readStream.CopyTo(writeStream);
                    return false;
                }
            }
        }

        protected HttpResponseMessage AddResponseContinuation(HttpResponseMessage message, XResponseContinuation responseContinuation)
        {
            var ret = message;
            ret.Headers.Add("X-ResponseContinuation", responseContinuation.ContinuationString);
            return ret;
        }

        protected JToken CloneToken(JToken token)
        {
            if (XGetInnerAllField.Value)
            {
                return RemoveFields(token, new string[] { "_rid", "_self", "_etag", "_attachments", "_ts" });
            }
            else
            {
                return RemoveFields(token, new string[] { "id", TYPE, "_rid", "_self", "_etag", "_attachments", "_ts", REGUSERID, VENDORID, SYSTEMID, PARTITIONKEY, REGDATE, VERSION_COLNAME });
            }
        }

        protected JToken RemoveFields(JToken token, string[] fields)
        {
            JContainer container = token as JContainer;
            if (container == null) return token;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
            }

            foreach (JToken el in removeList)
            {
                el.Remove();
            }

            return token;
        }

        protected bool IsValidUrlModelSchema(out HttpResponseMessage responseMessage)
        {
            responseMessage = null;
            // スキーマのチェック
            if (string.IsNullOrEmpty(UriSchema?.Value) == false)
            {
                var schema = UriSchema.ToJSchema();

                var targetJson = new JObject();

                // KeyValueとQueryのKeyの重複は登録時のチェックでないはず
                KeyValue?.Dic.ToList().ForEach(item =>
                {
                    targetJson[item.Key.Value] = GetJsonValue(item.Key.Value, item.Value.Value, schema);
                });
                Query?.Dic.ToList().ForEach(item =>
                {
                    targetJson[item.Key.Value] = GetJsonValue(item.Key.Value, item.Value.Value, schema);
                });

                IList<ValidationError> errors;
                if (targetJson.IsValid(schema, out errors) == false)
                {
                    responseMessage = TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, EditUrlModelJsonSchemaErrorMessage(errors)));
                    return false;
                }
            }

            return true;
        }

        protected JToken GetJsonValue(string key, string value, JSchema schema)
        {
            JSchema propertySchema = null;
            var getSchemaResult = schema?.Properties.TryGetValue(key, out propertySchema) ?? false;
            var target = (!getSchemaResult || propertySchema?.Type == JSchemaType.String)
                ? $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\""
                : value;
            try
            {
                return JToken.Parse(target);
            }
            catch (Exception)
            {
                // パースできなければstringとして扱う
                return JToken.Parse($"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"");
            }
        }

        protected string EditUrlModelJsonSchemaErrorMessage(IList<ValidationError> errors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in errors)
            {
                sb.Append($"UrlModel Path={error.Path} ErrorMessage={error.Message} ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Base64Blobのパスをレスポンスメッセージのヘッダーにカンマ区切りで追加する。
        /// ヘッダーはXgetInternalAllFieldが指定されている場合のみ追加される。
        /// </summary>
        protected void AddHeaderBase64BlobPath(HttpResponseMessage httpResponseMessage, List<string> base64BlobPathList)
        {
            if (base64BlobPathList == null)
            {
                return;
            }
            if (base64BlobPathList.Any())
            {
                var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                if (perRequestDataContainer.XgetInternalAllField)
                {
                    httpResponseMessage.Headers.Add("X-Base64BlobPath", String.Join(",", base64BlobPathList));
                }
            }
        }

        /// <summary>
        /// JTokenの置き換え
        /// <param name="jToken">置き換え対象Jtoken</param>
        /// <param name="replaceBase64PathList">PathとBase64StringのDictionary</param>
        /// <param name="keyPrefix">PathのPerfix</param>
        /// <param name="func">Base64マッチ後の処理</param>
        /// </summary>
        protected JToken ReplaceJtoken(JToken jToken, Dictionary<string, string> replaceBase64PathList, string keyPrefix, Func<JToken, Dictionary<string, string>, string, JToken> func)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    foreach (var child in jToken.Children<JProperty>())
                        ReplaceJtoken(child, replaceBase64PathList, keyPrefix, func);
                    break;
                case JTokenType.Array:
                    foreach (var child in jToken.Children())
                        ReplaceJtoken(child, replaceBase64PathList, keyPrefix, func);
                    break;
                case JTokenType.Property:
                    ReplaceJtoken(((JProperty)jToken).Value, replaceBase64PathList, keyPrefix, func);
                    break;
                default:
                    return func(jToken, replaceBase64PathList, keyPrefix);
            }
            return jToken;
        }
        /// <summary>
        /// PathをBase64Stringに置き換える
        /// </summary>
        protected JToken ReplaceJtokenPathToBase64(JToken jToken, Dictionary<string, string> base64files, string keyPrefix = null)
        {
            if (((JValue)jToken)?.Value == null)
            {
                return jToken;
            }

            string pattern = $@"(^\{Base64DecordFilePath}\()(?<val>.+?)(\))";
            string base64 = Regex.Match(((JValue)jToken).Value.ToString(), pattern).Groups["val"].Value;
            if (!string.IsNullOrEmpty(base64))
            {
                var base64String = BlobPathToBase64String(base64);
                ((JValue)jToken).Value = $"{Base64EncordString}({base64String})";
                base64files.Add(base64, base64String);
            }
            return jToken;
        }
        /// <summary>
        /// Base64StringをPathに置き換える
        /// </summary>
        protected JToken ReplaceJtokenBase64ToPath(JToken jToken, Dictionary<string, string> base64files, string keyPrefix)
        {
            if (((JValue)jToken)?.Value == null)
            {
                return jToken;
            }
            string pattern = $@"(^\{Base64EncordString}\()(?<val>.+?)(\))";
            string base64 = Regex.Match(((JValue)jToken).Value.ToString(), pattern).Groups["val"].Value;
            if (!string.IsNullOrEmpty(base64))
            {
                string path = $"{Base64BlobPrefix}/{keyPrefix}/{jToken.Path}";
                string endpoint = (this.AttachFileBlobRepositoryInfo?.Endpoint == null) ? "" : this.AttachFileBlobRepositoryInfo.Endpoint;
                ((JValue)jToken).Value = $"{Base64DecordFilePath}({endpoint}{VendorId.Value}/{path})";
                base64files.Add(path, base64);
            }
            return jToken;
        }

        /// <summary>
        /// Base64Stringを含むかどうか
        /// </summary>
        /// <returns></returns>
        protected bool ContainsBase64(JToken jToken)
        {
            var pattern = $@"(^\{Base64EncordString}\()(?<val>.+?)(\))";

            switch (jToken.Type)
            {
                case JTokenType.Object:
                    return jToken.Children<JProperty>().Any(x => ContainsBase64(x));
                case JTokenType.Array:
                    return jToken.Children().Any(x => ContainsBase64(x));
                case JTokenType.Property:
                    return ContainsBase64(((JProperty)jToken).Value);
                default:
                    var value = ((JValue)jToken).Value?.ToString();
                    return string.IsNullOrEmpty(value)
                        ? false
                        : Regex.IsMatch(((JValue)jToken).Value.ToString(), pattern);
            }
        }

        private static readonly Regex _base64rx = new Regex(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}[AEIMQUYcgkosw048]=|[A-Za-z0-9+/][AQgw]==)?$", RegexOptions.Compiled);
        /// <summary>
        /// Base64文字列として変換できるか
        /// Base64と判定できるかはBase64の変換対象64文字(パディング含めると正確には65文字)で構成されているかつ4で割り切れること(Base64のLengthは4文字ずつ変換するためからなず4の倍数となる。)
        /// </summary>
        protected bool TryBase64(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }

            if ((val.Length % 4 == 0) && _base64rx.IsMatch(val))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// blobをBase64Stringに変換
        /// </summary>
        protected string BlobPathToBase64String(string blobPath)
        {
            return AttachFileDynamicApiDataStoreRepository.GetFiletoBase64String(blobPath);
        }

        /// <summary>
        /// blobにBase64AttachFileをUpload
        /// </summary>
        protected void UploadBase64AttachFile(string blobName, string base64String)
        {
            AttachFileDynamicApiDataStoreRepository.UploadBase64ToFile(VendorId, base64String, blobName);
        }
        protected void DeleteBase64AttachFiles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            if (this.IsEnableAttachFile == null || !this.IsEnableAttachFile.Value)
            {
                return;
            }
            AttachFileDynamicApiDataStoreRepository.DeleteFilestoBase64(VendorId, $"{Base64BlobPrefix}/{id}");
        }

        protected class RegisterData
        {
            public RegisterData(JToken token, int index = 0)
            {
                jToken = token;
                Index = index;
            }

            public int Index { get; set; }
            public string id { get; set; }
            public JToken jToken { get; set; }
            //Base64のPathとBase64文字列
            public Dictionary<string, string> Base64AttachFiles { get; set; } = new Dictionary<string, string>();
        }

        /// <summary>
        /// 登録するJsonにBase64文字列があったら保存先のURLに置き換える。
        /// </summary>
        protected Dictionary<string, string> ReplaceBase64AttachFileJson(JToken jToken, string keyPrefix)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            jToken = ReplaceJtoken(jToken, files, keyPrefix, ReplaceJtokenBase64ToPath);
            return files;
        }

        protected JToken ToJson(string str) => string.IsNullOrEmpty(str) ? null : JToken.FromObject(JsonConvert.DeserializeObject(str, new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal }));

        protected IEnumerable<JToken> EnumerableJsonArray(bool isArray, JToken json)
        {
            if (isArray == true)
            {
                for (int i = 0; i < json.Count(); i++)
                {
                    string sentence = json[i].ToString();
                    var tmp = JToken.Parse(sentence);
                    yield return tmp;
                }
            }
            else
            {
                yield return json;
            }
        }

        protected JToken MergeJsonArray(bool isArray, IList<JToken> jsons)
        {
            if (isArray == false)
            {
                return (jsons == null && jsons.Count == 0) ? null : jsons[0];
            }
            else
            {
                JArray result = new JArray();
                jsons.ForEach(x => result.Add(x));
                return result;
            }
        }

        protected (DocumentHistoryReferenceHeader, bool) GetDataFromHeader(string referenceData)
        {
            DocumentHistoryReferenceHeader token = null;
            bool parseErrFlg = false;
            try
            {
                token = JsonConvert.DeserializeObject<DocumentHistoryReferenceHeader>(referenceData);
            }
            catch
            {
                parseErrFlg = true;
            }

            if (parseErrFlg)
            {
                //reference がnull の場合は、通常処理をしたいのでチェックした上で返す
                JToken data = null;
                try
                {
                    data = referenceData.ToJson();
                }
                catch
                {
                    //Json変換エラーはもう処理できないので、badrequest
                    return (null, true);
                }

                if (data["reference"].Type == JTokenType.Null)
                {
                    //通常処理
                    return (null, false);
                }
                else
                {
                    var b = false;
                    if (bool.TryParse(data["reference"].ToString(), out b))
                    {
                        //通常処理 or snaspshot
                        return (new DocumentHistoryReferenceHeader(b, null), false);
                    }
                    else
                    {
                        //通常処理
                        return (null, false);
                    }
                }
            }
            else
            {
                //refrence=true/false のみ指定はsnapshot使うか通常処理なので、リターン
                if (token.refhistinfo == null)
                {
                    return (token, false);
                }

                foreach (var t in token.refhistinfo)
                {
                    //resourePath とdocumentkey は無いと処理できないので、badrequest
                    if (string.IsNullOrEmpty(t.resourcePath) || string.IsNullOrEmpty(t.documentKey))
                    {
                        return (token, true);
                    }
                    else
                    {
                        //resourcePath がurlの形式で無い場合は、badrequest
                        if (!t.resourcePath.Contains("/"))
                        {
                            return (token, true);
                        }
                        //versionkey は任意項目だが、あればGuidチェック
                        if (!string.IsNullOrEmpty(t.versionKey))
                        {
                            //versionkey がguid でない場合は、badrequest
                            try
                            {
                                Guid.Parse(t.versionKey);
                            }
                            catch
                            {
                                return (token, true);
                            }
                        }
                    }
                }
                return (token, false);
            }
        }

        /// <summary>
        /// 指定のバージョンでReference先のデータを取得し、返却する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected Tuple<HttpStatusCode, JToken> GetHistoryReference(string url)
        {
            // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
            // Resolveに失敗するため、httpContextを偽装
            ReplaceHttpContextCurrent();

            var retTuple = new Tuple<HttpStatusCode, JToken>(HttpStatusCode.NotFound, null);

            (var token, var iserror) = GetDataFromHeader(PerRequestDataContainer.XReferenceHistory);
            var info = GetOtherResource(url, true);

            //指定のresourePath、VersionKey で履歴データリストを作る
            foreach (var h1 in token.refhistinfo) // NOTE Headerに記載された順番で解決したいので、Paralell化しない
            {
                if (!url.Contains(h1.resourcePath)) continue;
                if (!_targetHistInfo.Exists(x =>
                    x.url == url && x.dockey == h1.documentKey && x.versionkey == h1.versionKey))
                {
                    var requestUrl = h1.resourcePath + "/GetDocumentHistory?id=" + h1.documentKey + "&version=" +
                                     h1.versionKey;
                    Tuple<HttpStatusCode, JToken> dochist;
                    try
                    {
                        dochist = new ApiHelper().ExecuteGetApiToJTokenAndHttpStatusCode(requestUrl);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"GetHistoryReference. ApiHelper().ExecuteGetApiToJTokenAndHttpStatusCode. requestUrl={requestUrl}");
                        Logger.Error(ex.Message, ex);
                        if (ex.InnerException != null)
                        {
                            Logger.Error(ex.InnerException.Message, ex.InnerException);
                        }
                        throw;
                    }
                    (string dockey, string versionkey, string url, JToken histInfo) hist =
                        (h1.documentKey, h1.versionKey, url, dochist.Item1 == HttpStatusCode.OK ? dochist.Item2 : null);

                    lock (_targetHistInfo) // _targetHistInfoへの並列アクセスを禁止
                    {
                        _targetHistInfo.Add(hist);
                    }
                }
            }

            //単一項目参照の場合は、1つのみ(item3はODataかどうかのフラグ)
            if (!info.Item3)
            {
                JToken retJson;
                lock (_targetHistInfo) // _targetHistInfoへの並列アクセスを禁止
                {
                    retJson = _targetHistInfo.FirstOrDefault(x => x.url == url).histInfo;
                }
                if (retJson != null)
                {
                    return new Tuple<HttpStatusCode, JToken>(HttpStatusCode.OK, retJson);
                }
            }
            else
            {
                JArray retArray = new JArray();
                List<(string dockey, string versionkey, string url, JToken histInfo)> data;
                lock (_targetHistInfo) // _targetHistInfoへの並列アクセスを禁止
                {
                    data = _targetHistInfo.Where(x => x.url == url).ToList();
                }

                data.ForEach(x => {
                    if (x.histInfo != null)
                        retArray.Add(x.histInfo);
                });
                if (retArray.Count != 0)
                {
                    // orderbyが指定されている場合は配列要素を並び替える
                    var OdataOrderByKeyword = "$orderby=";
                    if (url.Contains(OdataOrderByKeyword))
                    {
                        var SymbolComma = ',';
                        var SymbolSpace = ' ';
                        var RequestParamsAggregator = '&';
                        var KeywordDescending = "desc";

                        // urlからorderby句を抽出
                        // "$orderby=col1,col2 DESC,col3 ASC&$select=col1" -> "col1,col2 DESC,col3 ASC&$select=col1"
                        var orderByString = url.Substring(url.IndexOf(OdataOrderByKeyword) + OdataOrderByKeyword.Length);
                        if (orderByString.Contains(RequestParamsAggregator))
                        {
                            // "col1,col2 DESC,col3 ASC&$select=col1" -> "col1,col2 DESC,col3 ASC"
                            orderByString = orderByString.Remove(orderByString.IndexOf(RequestParamsAggregator));
                        }

                        // NOTE: ThenByを利用したいのでO(1)ソートを実行してJArrayをIOrderedEnumerableにキャストする
                        IOrderedEnumerable<JToken> tempArray = retArray.OrderBy(x => 1);
                        foreach (var orderByClause in orderByString.Split(SymbolComma))
                        {
                            var orderBy = orderByClause.Split(SymbolSpace);
                            var sortKey = orderBy[0];
                            var isDescending = orderBy.Length == 2 ? orderBy[1].ToLower() == KeywordDescending : false;
                            tempArray = isDescending ?
                                  tempArray.ThenByDescending(x => (object)x[sortKey]) : tempArray.ThenBy(x => (object)x[sortKey]);
                        }

                        retArray = new JArray(tempArray);
                    }

                    return new Tuple<HttpStatusCode, JToken>(HttpStatusCode.OK, retArray);
                }
            }
            //NotFound
            return retTuple;
        }

        /// <summary>
        /// HACK
        /// Reference, Notify系の処理で非同期通信する際、ResolveがNullReferenceになってしまう問題の回避用
        /// </summary>
        /// <returns></returns>
        private void ReplaceHttpContextCurrent()
        {

        }

        private Dictionary<string, Task<Tuple<HttpStatusCode, JToken, bool, string, List<string>>>> _dic =
            new Dictionary<string, Task<Tuple<HttpStatusCode, JToken, bool, string, List<string>>>>();

        private ConcurrentBag<ResponseHeader> _resHeaders = new ConcurrentBag<ResponseHeader>();

        private List<(string dockey, string versionkey, string url, JToken histInfo)> _targetHistInfo =
            new List<(string dockey, string versionkey, string url, JToken resourcePath)>();

        protected static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
        }).CreateMapper();

        /// <summary>
        /// Reference属性が指定されているプロパティがあったら、データの$Referenceを参照先の値で戻す
        /// </summary>
        /// <param name="str"></param>
        /// <param name="GetHistoryReference"></param>
        /// <param name="isGetOtherResourceNull"></param>
        /// <returns></returns>
        protected JToken RecoveryReferenceAttribute(string str, Func<string, Tuple<HttpStatusCode, JToken>> GetHistoryReference = null, bool isGetOtherResourceNull = false)
        {
            _dic = new Dictionary<string, Task<Tuple<HttpStatusCode, JToken, bool, string, List<string>>>>();
            _targetHistInfo = new List<(string dockey, string versionkey, string url, JToken histInfo)>();
            TmpHttpContext = UnityCore.Resolve<IHttpContextAccessor>().HttpContext; // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
            TmpPerRequestDataContainer = PerRequestDataContainer?.DeepCopy(); // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用

            var json = ToJson(str);
            if (TmpPerRequestDataContainer.IsSkipJsonFormatProtect == false)
            {
                bool isArray = json.Type == JTokenType.Array;
                var data = EnumerableJsonArray(isArray, json).ToList();
                var schema = ResponseSchema?.ToJSchema() == null ? ControllerSchema?.ToJSchema() : ResponseSchema?.ToJSchema();
                Parallel.ForEach(data, new ComParallelOptions(), x =>
                {
                    if (isGetOtherResourceNull)
                    {
                        new JsonPropertyFormatProtect(false) { Schema = schema, Update = x, GetHistoryReference = GetHistoryReference }.Recovery();
                    }
                    else
                    {
                        new JsonPropertyFormatProtect(false) { Schema = schema, Update = x, GetOtherResource = GetOtherResource }.Recovery();
                    }
                });
                return MergeJsonArray(isArray, data);
            }
            else
            {
                return json;
            }
        }

        protected (JToken, List<ResponseHeader>) RecoveryReferenceAttributeWithHeader(string str)
        {
            _resHeaders = new ConcurrentBag<ResponseHeader>();
            var token = RecoveryReferenceAttribute(str);
            return (token, _resHeaders.ToList());
        }

        protected Tuple<HttpStatusCode, JToken, bool, string, List<string>> GetOtherResource(string url,
            bool isGetResourceForNotify)
        {
            Task<Tuple<HttpStatusCode, JToken, bool, string, List<string>>> task;
            var dickey = url + "__" + isGetResourceForNotify + "__";
            var innerGetOtherResourceAction = new Func<Task<Tuple<HttpStatusCode, JToken, bool, string, List<string>>>>(() =>
            {
                if (_dic.Keys.Contains(dickey))
                {
                    task = _dic[dickey];
                }
                else
                {
                    task = Task.Run(() =>
                    {
                        // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
                        // Resolveに失敗するため、httpContextを偽装
                        ReplaceHttpContextCurrent();

                        var queryUrl = string.Empty;
                        var repositoryKeys = new List<string>();
                        IMethod findApi = null;

                        //isGetResourceForNotifyがtrueの場合は、Notify用にデータを取得するURLを変える
                        if (isGetResourceForNotify)
                        {
                            var dynamicApiRepository = UnityCore.Resolve<IDynamicApiRepository>();
                            var uri = new Uri("https://localhost" + url);
                            try
                            {
                                //RelativeUrl が欲しいので、FindApiする
                                findApi = dynamicApiRepository.FindApi(
                                    new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                                    new RequestRelativeUri(uri.AbsolutePath),
                                    new GetQuery(uri.Query));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"GetOtherResource. dynamicApiRepository.FindApi. url.AbsolutePath={uri.AbsolutePath}, url.Query={uri.Query}");
                                Logger.Error(ex.Message, ex);
                                if (ex.InnerException != null)
                                {
                                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                                }
                                throw;
                            }

                            //findApiして結果が貰えないのは、Notimple
                            if (findApi == null)
                            {
                                return new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(
                                    HttpStatusCode.NotImplemented, null,
                                    false, null, null);
                            }

                            var tmpReposKeys = findApi.ControllerRepositoryKey.IsExsitsLogicalKey
                                ? findApi.ControllerRepositoryKey.LogicalKeys.ToList()
                                : null;
                            if (tmpReposKeys != null)
                            {
                                foreach (var t in tmpReposKeys)
                                {
                                    repositoryKeys.Add(t);
                                }
                            }

                            //ODataの場合は、スキーマに設定されている$filter を使ってODataで取得
                            if (findApi.ActionType?.Value == Context.DynamicApi.ActionType.OData)
                            {
                                var dollarFilter = findApi.Query?.GetValue("$filter");
                                if (string.IsNullOrEmpty(dollarFilter))
                                {
                                    queryUrl = findApi.ControllerRelativeUrl.Value + "/OData";
                                }
                                else
                                {
                                    queryUrl = findApi.ControllerRelativeUrl.Value + "/OData?$filter=" + dollarFilter;
                                }
                            }
                            //単一項目参照の場合は、urlのQueryから、ODataで同一ドキュメントが取得できるように組み立て
                            else
                            {
                                var query = string.Empty;
                                if (findApi.Query != null)
                                {
                                    //RefSourceから ODataを使ってUpdate対象のIDを取得する
                                    query = "?$filter=";
                                    var i = 0;
                                    foreach (var q in findApi.Query.Dic)
                                    {
                                        query += q.Key.Value + " eq '" + q.Value.Value + "'";
                                        if (i + 1 != findApi.Query.Dic.Count)
                                        {
                                            query += " and ";
                                        }

                                        i++;
                                    }
                                }

                                queryUrl = findApi.ControllerRelativeUrl.Value + "/OData" + query;
                            }
                        }
                        else
                        {
                            queryUrl = url;
                        }

                        HttpResponseMessage executeGetApiResult;
                        string msg;
                        Tuple<HttpStatusCode, JToken, bool, string, List<string>> res;
                        try
                        {
                            executeGetApiResult = new ApiHelper().ExecuteGetApi(queryUrl);
                            msg = executeGetApiResult.Content?.ReadAsStringAsync().Result;
                            res = new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(
                                executeGetApiResult.StatusCode,
                                string.IsNullOrEmpty(msg) ? null : msg.ToJson(),
                                findApi?.ActionType?.Value == Context.DynamicApi.ActionType.OData, findApi?.ControllerRelativeUrl?.Value,
                                repositoryKeys);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"GetOtherResource. ApiHelper().ExecuteGetApi(). queryUrl={queryUrl}");
                            Logger.Error(ex.Message, ex);
                            if (ex.InnerException != null)
                            {
                                Logger.Error(ex.InnerException.Message, ex.InnerException);
                            }
                            throw;
                        }

                        // Reference先の履歴が有効の場合はヘッダが返ってくるので、クライアントまで返却するために、ヘッダを中継するための処理
                        if (executeGetApiResult.Headers.Count() != 0)
                        {
                            executeGetApiResult.Headers.ToList().ForEach(x =>
                            {
                                if (x.Key == DOCUMENTHISTORY_HEADERNAME)
                                {
                                    var tval = x.Value.ToList();
                                    lock (_resHeaders) // _resHeadersへの並列アクセスを禁止
                                    {
                                        foreach (var v in tval)
                                        {
                                            _resHeaders.Add(new ResponseHeader(x.Key, v.ToJson()));
                                        }
                                    }
                                }
                            });
                        }

                        return res;
                    });

                    _dic.Add(dickey, task); // taskの参照だけを入れる
                }
                return task;
            });
            if (enableThreadingOfReference)
            {
                lock (_dic) // _dicへの並列アクセスを禁止
                {
                    task = innerGetOtherResourceAction();
                }
            }
            else
            {
                task = innerGetOtherResourceAction();
            }

            var result = task.Result;
            return result;
        }

        protected JToken InternalMerge(JToken source, JToken target)
        {
            var result = new JObject();
            foreach (var child in source)
            {
                var prop = child as JProperty;
                if (prop != null && prop.Name != INTERNAL_UPDATE)
                {
                    result.AddFirst(new JProperty(prop.Name, target.GetPropertyValue(prop.Name)));
                    target.SetPropertyValue(prop.Name, prop.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// NotifyによるJsonデータか、jsonの入れ子でないシンプルなjsonか？
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        protected bool IsInernalMerge(JToken json)
        {
            bool isChild = false;
            foreach (var p in json)
            {
                var child = p as JProperty;
                if (child?.Name == INTERNAL_UPDATE)
                {
                    return true;
                }
                else if (child.Value?.Type == JTokenType.Object)
                {
                    isChild = true;
                }
            }
            //return !isChild;
            return false;
        }

        /// <summary>
        /// $ReferenceとNotify属性による他リソースへの更新処理
        /// </summary>
        /// <param name="pos">配列の場合はその要素番号。配列でない場合はnull</param>
        /// <param name="update">$ReferenceやNotifyによる他リソースの更新情報</param>
        /// <param name="refErrors">エラー情報</param>
        /// <param name="refSourceHeaders">Reference先の応答ヘッダ</param>
        /// <param name="refJsonValidationErrorMsgs">Reference先のバリデーションエラー</param>
        /// <returns>成功失敗の有無</returns>
        protected bool OtherResourceUpdate(int? pos, UpdateValueCollection update, ref List<string> refErrors, ref List<ResponseHeader> refSourceHeaders, ref List<RFC7807ProblemDetailExtendErrors> refJsonValidationErrorMsgs, ref bool isDataNotifed)
        {
            var results = new ConcurrentBag<bool>(); // result返却用
            if (update?.Any() == true)
            {
                var api = new ApiHelper();
                var merged = update.Merge().ToList();
                if (merged.Count != 0)
                {
                    //データは、Notifyするデータ
                    isDataNotifed = true;

                    // Parallel処理内で書き換えるのでlistをthreadSafeに
                    var errors = new ConcurrentBag<string>(refErrors);
                    var sourceHeaders = new ConcurrentBag<ResponseHeader>(refSourceHeaders);
                    var jsonValidationErrorMsgs = new ConcurrentBag<RFC7807ProblemDetailExtendErrors>(refJsonValidationErrorMsgs);

                    //ヘッダのKeyValueリスト
                    var headervalues = Enumerable.Range(0, 0).Select(x => new { heardername = "", val = "" }).ToList();

                    TmpHttpContext = UnityCore.Resolve<IHttpContextAccessor>().HttpContext; // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
                    TmpPerRequestDataContainer = PerRequestDataContainer?.DeepCopy(); // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用

                    Parallel.ForEach(merged, new ComParallelOptions(), u =>
                    {
                        // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
                        // Resolveに失敗するため、httpContextを偽装
                        ReplaceHttpContextCurrent();

                        var json = u.Json;
                        HttpResponseMessage res = null;
                        try
                        {
                            if (u.targetHttpMethod.Method == HttpMethod.Post.Method)
                            {
                                res = api.ExecutePostApi(u.Url, json.ToString());
                            }
                            else if (u.targetHttpMethod.Method == HttpMethod.Delete.Method)
                            {
                                res = api.ExecuteDeleteApi(u.Url);
                            }
                            else
                            {
                                json.FirstOrDefault().AddAfterSelf(new JProperty(INTERNAL_UPDATE, INTERNAL_UPDATE));
                                res = api.ExecutePatchApi(u.Url, json.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"OtherResourceUpdate. ExecuteApi. Method={u.targetHttpMethod.Method}. url={u.Url}");
                            Logger.Error(ex.Message, ex);
                            if (ex.InnerException != null)
                            {
                                Logger.Error(ex.InnerException.Message, ex.InnerException);
                            }
                            throw;
                        }

                        if (res.StatusCode == HttpStatusCode.NoContent || res.StatusCode == HttpStatusCode.OK ||
                            res.StatusCode == HttpStatusCode.Created)
                        {
                            if (res.Headers.Any())
                            {
                                //レスポンスヘッダがあれば、取っておく
                                bool hasResponseHeader = false;
                                foreach (var h in res.Headers)
                                {
                                    if (h.Key != DOCUMENTHISTORY_HEADERNAME) continue;
                                    var tval = h.Value.ToList();
                                    foreach (var v in tval)
                                    {
                                        sourceHeaders.Add(new ResponseHeader(h.Key, v.ToJson()));
                                        hasResponseHeader = true;
                                    }
                                }

                                //Notify先からのヘッダが無い場合は、空の履歴ヘッダを作る
                                if (!hasResponseHeader)
                                {
                                    var hist = new DocumentHistoryHeader(true, u.ControllerUrl, null).ToJson();
                                    var arr = new JArray();
                                    arr.Add(hist);
                                    var resHeader = new ResponseHeader(DOCUMENTHISTORY_HEADERNAME, arr);
                                    sourceHeaders.Add(resHeader);
                                }
                            }

                            if (string.IsNullOrEmpty(u.BeforeJson))
                                u.BeforeJson = res.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrEmpty(u.RollbackUrl) && u.targetHttpMethod == HttpMethod.Post)
                                u.RollbackUrl = u.Url.Replace("Register", "DeleteById") + "/" +
                                                u.BeforeJson.Replace("\r", "").Replace("\n", "").ToJson()["id"];
                            u.IsUpdated = true;
                        }
                        else // 更新失敗のためロールバックする
                        {
                            u.IsUpdated = false;
                            string element = pos != null ? $"要素{pos}番目のデータ{u.Json.ToString()}" : null;
                            errors.Add($"{element}登録による更新(他リソースの作成／更新／削除)に失敗しました。");
                            //Problem+Json は違うエラーメッセージを整形
                            IEnumerable<string> contenttype = null;
                            if (res.Content.Headers.TryGetValues("Content-Type", out contenttype))
                            {
                                if (contenttype.First().Contains(MEDIATYPE_ProblemJson))
                                {
                                    var problemContent = res.Content.ReadAsStringAsync().Result;
                                    //リターンするmessageにセット
                                    if (problemContent != null)
                                        jsonValidationErrorMsgs.Add(ToJsonValidationErrorMessage(problemContent));
                                }
                            }
                            results.Add(false); // 更新失敗
                        }
                    });
                    // threadSafeなlistを格納し直す
                    refErrors.AddRange(errors.ToList());
                    refSourceHeaders.AddRange(sourceHeaders.ToList());
                    refJsonValidationErrorMsgs.AddRange(jsonValidationErrorMsgs.ToList());
                }
            }

            return !results.Any(x => x == false); //falseがあったらfalseをそのまま返却する。
        }

        protected RFC7807ProblemDetailExtendErrors ToJsonValidationErrorMessage(string problemContent)
        {
            var notifyDetail = problemContent.ToJson();
            var rfcdetail = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10401, this.RelativeUri?.Value);

            rfcdetail.Instance = new Uri(notifyDetail["instance"].ToString(), UriKind.Relative);
            var errors = notifyDetail["errors"].ToList();

            Dictionary<string, dynamic> j = new Dictionary<string, dynamic>();
            for (int i = 0; i < errors.Count; i++)
            {
                var msgs = new List<string>();
                foreach (var err in errors[i].Children())
                {
                    foreach (var e in err)
                    {
                        msgs.Add(e.ToString());
                    }
                }
                j.Add(errors[i].Path.Replace("errors.", ""), msgs);
            }
            rfcdetail.Errors = j;
            rfcdetail.Detail = notifyDetail["detail"]?.ToString();
            return rfcdetail;
        }

        protected void OtherResourceRollback(List<JsonPropertyFormatProtect> notifies, ref List<string> errors, ref List<ResponseHeader> refSourceHeaders)
        {
            int RollbackFailCount = 0;
            for (int i = notifies.Count - 1; i >= 0; i--)
            {
                RollbackFailCount += otherResourceRollback(i, notifies[i], ref errors, ref refSourceHeaders);
            }
            errors.Add(RollbackFailCount > 0 ? "ロールバックには失敗しているものがあります。先の情報から手動でロールバックしてください。" : "ロールバックは成功しました。");
        }

        protected void OtherResourceRollback(JsonPropertyFormatProtect notify, ref List<string> errors, ref List<ResponseHeader> refSourceHeaders)
        {
            int RollbackFailCount = otherResourceRollback(null, notify, ref errors, ref refSourceHeaders);
            errors.Add(RollbackFailCount > 0 ? "ロールバックには失敗しているものがあります。先の情報から手動でロールバックしてください。" : "ロールバックは成功しました。");
        }

        protected int otherResourceRollback(int? pos, JsonPropertyFormatProtect notify, ref List<string> refErrors, ref List<ResponseHeader> refSourceHeaders)
        {
            string element = pos != null ? $"要素{pos}番目の" : null;
            refErrors.Add($"{element}登録をロールバックします。");
            int rollbackFailCount = 0;
            var api = new ApiHelper();
            if (notify.UpdateOtherResource?.Any() == true)
            {
                var merged = notify.UpdateOtherResource.Merge().ToList();

                // Parallel処理内で書き換えるのでlistをthreadSafeに
                var errors = new ConcurrentBag<string>(refErrors);
                var sourceHeaders = new ConcurrentBag<ResponseHeader>(refSourceHeaders);

                //ヘッダとヘッダ値リスト
                var headervalues = Enumerable.Range(0, 0).Select(x => new { heardername = "", val = "" }).ToList();

                TmpHttpContext = UnityCore.Resolve<IHttpContextAccessor>().HttpContext; // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
                TmpPerRequestDataContainer = PerRequestDataContainer?.DeepCopy(); // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用

                Parallel.ForEach(merged, new ComParallelOptions(), (rollback, state, i) =>
                {
                    // HACK Reference, Notify系の処理で非同期通信する際NullReferenceになってしまう問題の回避用
                    // Resolveに失敗するため、httpContextを偽装
                    ReplaceHttpContextCurrent();

                    if (rollback.IsUpdated == true)
                    {
                        var rollbackJson = rollback.BeforeJson.ToJson();
                        HttpResponseMessage res = null;

                        try
                        {
                            if (rollback.rollbackHttpMethod.Method == HttpMethod.Post.Method)
                            {
                                res = api.ExecutePostApi(rollback.RollbackUrl, rollbackJson.ToString());
                            }
                            else if (rollback.rollbackHttpMethod.Method == HttpMethod.Delete.Method)
                            {
                                res = api.ExecuteDeleteApi(rollback.RollbackUrl);
                            }
                            else
                            {
                                rollbackJson.FirstOrDefault()
                                    .AddAfterSelf(new JProperty(INTERNAL_UPDATE, INTERNAL_UPDATE));
                                res = api.ExecutePatchApi(rollback.RollbackUrl, rollbackJson.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"otherResourceRollback. ExecuteApi. Method={rollback.targetHttpMethod.Method}. url={rollback.RollbackUrl}");
                            Logger.Error(ex.Message, ex);
                            if (ex.InnerException != null)
                            {
                                Logger.Error(ex.InnerException.Message, ex.InnerException);
                            }
                            throw;
                        }

                        if (res.Headers.Any())
                        {
                            var hasResponseHeader = false;
                            //レスポンスヘッダがあれば、取っておく
                            foreach (var h in res.Headers)
                            {
                                if (h.Key != DOCUMENTHISTORY_HEADERNAME) continue;
                                var tval = h.Value.ToList();
                                foreach (var v in tval)
                                {
                                    sourceHeaders.Add(new ResponseHeader(h.Key, v.ToJson().ToString()));
                                    hasResponseHeader = true;
                                }
                            }
                            //Notify先からのヘッダが無い場合は、空の履歴ヘッダを作る
                            if (!hasResponseHeader)
                            {
                                var hist = new DocumentHistoryHeader(true, rollback.ControllerUrl, null).ToJson();
                                var arr = new JArray();
                                arr.Add(hist);
                                var resHeader = new ResponseHeader(DOCUMENTHISTORY_HEADERNAME, arr);
                                sourceHeaders.Add(resHeader);
                            }
                        }
                        rollback.IsRollbackFail = res.StatusCode == HttpStatusCode.NoContent || res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.Created ? false : true;
                        errors.Add($"外部更新[{i}]のデータのロールバックは" + (rollback.IsRollbackFail == true ? "失敗" : "成功"));
                        if (rollback.IsRollbackFail == true)
                        {
                            Interlocked.Increment(ref rollbackFailCount);
                        }
                    }

                });
                // threadSafeなlistを格納し直す
                refErrors.AddRange(errors.ToList());
                refSourceHeaders.AddRange(sourceHeaders.ToList());
            }
            return rollbackFailCount;
        }

        /// <summary>
        /// RequestのSchemaを取得
        /// RequestのSchemaが定義されていない場合はControllerのSchema定義を使う
        /// </summary>
        /// <returns></returns>
        protected DataSchema GetRequestSchema()
        {
            if (RequestSchema?.ToJSchema() != null)
            {
                return RequestSchema;
            }
            else if (ControllerSchema?.ToJSchema() != null)
            {
                return ControllerSchema;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ResponseのSchemaを取得
        /// ResponseのSchemaが定義されていない場合はControllerのSchema定義を使う
        /// </summary>
        /// <returns></returns>
        protected DataSchema GetResponseSchema()
        {
            if (ResponseSchema?.ToJSchema() != null)
            {
                return ResponseSchema;
            }
            else if (ControllerSchema?.ToJSchema() != null)
            {
                return ControllerSchema;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// キャッシュのキーを作成する
        /// </summary>
        protected string CreateBlobCacheKey(INewDynamicApiDataStoreRepository repository, QueryParam queryParam)
        {
            if (IsUseBlobCache.Value == false) return null;
            var path = CreateResourceBlobCacheKey() + "/" +
                       CacheManager.CreateBlobKey(
                           this.ApiId.Value,
                           GetCacheKey(repository, queryParam));
            return path.Length <= 1024 ? path : null;
        }

        /// <summary>
        /// リソースのキャッシュキーを作成する
        /// </summary>
        protected string CreateResourceBlobCacheKey()
        {
            return CacheManager.CreateBlobKey(CACHE_KEY_API_ACTION, this.ProviderVendorId.Value, this.ProviderSystemId.Value, this.ControllerId.Value);
        }

        protected List<ResponseHeader> histHeaders = null;
        protected void DeleteCallback(JToken json, RepositoryType repositoryType)
        {
            if (json != null)
            {
                var id = (json[ID] as JValue).Value;
                string versionKey = null;
                if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true)
                {
                    var ret = MakeHistory(id.ToString(), json, true);
                    if (histHeaders != null)
                    {
                        histHeaders.Add(new ResponseHeader(DOCUMENTHISTORY_HEADERNAME, ret[DOCUMENTHISTORY_HEADERNAME].ToJson()));
                    }
                    var historyHeader = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(ret.SingleOrDefault(x => x.Key == DOCUMENTHISTORY_HEADERNAME).Value).SingleOrDefault(x => x.isSelfHistory);
                    versionKey = historyHeader.documents.SingleOrDefault(x => x.documentKey == id.ToString())?.versionKey;
                }
                BlockchainEventHubStoreRepository?.Delete(id.ToString(), repositoryType, versionKey);
            }
        }

        protected Dictionary<string, string> MakeHistory(string key, JToken before, bool isDelete) => MakeHistory(new string[1] { key }, new JToken[1] { before }, isDelete);

        protected Dictionary<string, string> MakeHistory(string[] keys, JToken[] before, bool isDelete, bool isDriveout = false)
        {
            var header = new Dictionary<string, string>();
            // JsonDocument履歴は無効なら何もしない
            if (EnableJsonDocumentHistory == false)
            {
                return header;
            }
            if (this.IsDocumentHistory?.Value == true && HistoryEvacuationDataStoreRepository == null)
            {
                Logger.Warn("履歴退避用のRepositoryが設定されていません");
            }
            else if (this.IsDocumentHistory?.Value == true && HistoryEvacuationDataStoreRepository != null)
            {
                var headerinfo = new List<DocumentHistoryHeaderDocumentData>();
                this.ShallowMapProperty(DynamicApiDataStoreRepository[0].DocumentVersionRepository);
                for (int i = 0; i < keys.Length; i++)
                {
                    var varsionKey = Guid.NewGuid().ToString();
                    var documentKey = new DocumentKey(RepositoryKey, keys[i]);
                    DocumentHistories resultHistory = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(documentKey);
                    if (resultHistory != null)
                    {
                        var latest = resultHistory.DocumentVersions.LastOrDefault();
                        // beforeがあるということは、jsonデータが存在するということ（Deleteした直後ではない）
                        if (before[i] != null)
                        {
                            var param = ValueObjectUtil.Create<RegisterParam>(before[i], this);
                            var result = HistoryEvacuationDataStoreRepository.RegisterOnce(param);
                            var path = (result.Additional["Container"] as string).UrlCombine(result.Value);
                            latest = new DocumentHistory(latest.VersionKey, latest.VersionNo.Value, latest.CreateDate, latest.OpenId, DocumentHistory.StorageLocationType.LowPerformance, HistoryEvacuationDataStoreRepository.RepositoryKeyInfo, path);
                        }
                        // DriveOutされた場合はLowPerformanceにデータが入っている
                        else if (latest.LocationType != DocumentHistory.StorageLocationType.LowPerformance)
                        {
                            latest = new DocumentHistory(latest.VersionKey, latest.VersionNo.Value, latest.CreateDate, latest.OpenId, DocumentHistory.StorageLocationType.Delete, null, null);
                        }

                        if (isDriveout)
                        {
                            resultHistory = DynamicApiDataStoreRepository[0].DocumentVersionRepository.UpdateDocumentVersion(documentKey, latest);
                        }
                        else
                        {
                            resultHistory = DynamicApiDataStoreRepository[0].DocumentVersionRepository.SaveDocumentVersion(documentKey,
                                DynamicApiDataStoreRepository[0].DocumentVersionRepository.RepositoryKeyInfo, latest, isDelete);
                        }
                    }
                    else
                    {
                        // make first time
                        resultHistory = DynamicApiDataStoreRepository[0].DocumentVersionRepository.SaveDocumentVersion(documentKey, DynamicApiDataStoreRepository[0].DocumentVersionRepository.RepositoryKeyInfo, isDelete);
                    }
                    var headerdata = new DocumentHistoryHeaderDocumentData(keys[i], resultHistory.DocumentVersions.LastOrDefault()?.VersionKey);
                    headerinfo.Add(headerdata);
                }
                if (headerinfo.Count != 0)
                {
                    var histHeader = new DocumentHistoryHeader(true, this.ControllerRelativeUrl?.Value, headerinfo);
                    var histHeaderList = new List<DocumentHistoryHeader>();
                    histHeaderList.Add(histHeader);
                    header.Add(DOCUMENTHISTORY_HEADERNAME, histHeaderList.ToJson().ToString().Replace("\r", "").Replace("\n", ""));
                }

            }

            return header;
        }

        protected void RegisterBlockchainEvent(JToken data, RepositoryType repositoryType, RepositoryInfo repositoryInfo, string versionKey = null)
        {
            switch (repositoryInfo.Type)
            {
                case RepositoryType.AttachFileBlob:
                case RepositoryType.AttachFileMetaCosmosDb:
                case RepositoryType.AttachFileMetaSqlServer:
                    //attachfile系だけはid項目をfileidにする必要があるのでここから呼び出すようにする
                    BlockchainEventHubStoreRepository?.Register(data["FileId"].ToString(), data, repositoryType, versionKey);
                    break;
                default:
                    BlockchainEventHubStoreRepository?.Register(data[ID].ToString(), data, repositoryType, versionKey);
                    break;
            }
        }

        internal JsonDocument GetDocumentHistory(DocumentHistory target)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            //var param = new QueryParam(VendorId, SystemId, OpenId, null, UriSchema, RequestSchema, ResponseSchema,
            //    RepositoryKey, ApiQuery, PostDataType, ActionType, PartitionKey, KeyValue, IsVendor, IsPerson, IsOverPartition, IsAutomaticId, null, QueryType, null, null,
            //    ApiResourceSharing, ResourceSharingPersonRules, null, null, null, null);
            var param = ValueObjectUtil.Create<QueryParam>(this, perRequestDataContainer);
            if (HistoryEvacuationDataStoreRepository is INewBlobDataStoreRepository blob)
            {
                var strs = new List<string>(target.Location.Split('/'));
                // locationの最初のセンテンスがContainer名、残りがパスとファイル名
                blob.DefaultContainerFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => strs[0];
                blob.DefaultFileNameFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => string.Join("/", strs.Skip(1).ToArray());
                blob.DefaultRepositoryIds = () => new Tuple<Guid?, Guid?>(target.RepositoryGroupId, target.PhysicalRepositoryId);
            }
            return HistoryEvacuationDataStoreRepository.QueryOnce(param);
        }

        /// <summary>
        /// レスポンスヘッダをマージする
        /// </summary>
        /// <param name="mergeTarget"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Dictionary<string, string> MergeResponseHeader(Dictionary<string, string> mergeTarget, Dictionary<string, JToken> data)
        {
            var mergeKey = mergeTarget.Keys.Where(x => data.ContainsKey(x)).ToList();
            var addkey = data.Keys.Except(mergeKey).ToList();
            foreach (var m1 in mergeKey)
            {
                var mergedHeader = mergeTarget[m1].ToJson().Type == JTokenType.Array ? JArray.Parse(mergeTarget[m1]) : mergeTarget[m1].ToJArray();
                var refsourcejson = data[m1].ToJson();
                if (refsourcejson.Type == JTokenType.Array)
                {
                    var headerjson = JArray.Parse(refsourcejson.ToString());
                    refsourcejson.ToList().ForEach(x => mergedHeader.Add(x.ToString().Replace("\n", "").Replace("\r", "").ToJson()));
                    mergeTarget[m1] = mergedHeader.ToJson().ToString().Replace("\n", "").Replace("\r", "");
                }
                else
                {
                    mergedHeader.Add(refsourcejson);
                    mergeTarget[m1] = mergedHeader.ToJson().ToString().Replace("\n", "").Replace("\r", "");
                }
            }
            foreach (var ad1 in addkey)
            {
                mergeTarget.Add(ad1, data[ad1].ToString().Replace("\n", "").Replace("\r", ""));
            }

            return mergeTarget;
        }

        /// <summary>
        /// 履歴ヘッダのうち、同じResourcePathのものを、同一オブジェクトにマージする
        /// Notifyの処理上、同じAPIの履歴がListで複数作成されるので、一つにする。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="controllerUrl"></param>
        /// <returns></returns>
        protected List<ResponseHeader> MergeHistoryRefSourceHeader(string controllerUrl, List<ResponseHeader> data)
        {
            var ret = new List<ResponseHeader>();
            var mergedheader = data.GroupBy(x => x.Headername).ToList();
            var processedIndexList = new List<int>();
            foreach (var mh1 in mergedheader)
            {
                //履歴ヘッダだけ処理
                if (mh1.Key == DOCUMENTHISTORY_HEADERNAME)
                {
                    var resVal = new JArray();
                    //履歴ヘッダが複数無い場合は、isSelfHistoryだけ更新してリターンする
                    if (mh1.Count() == 1)
                    {
                        var arr = JArray.Parse(mh1.First().Value.ToString());
                        arr.ForEach(x => x["isSelfHistory"] = x["resourcePath"].ToString() == controllerUrl ? true : false);
                        ret.Add(new ResponseHeader(DOCUMENTHISTORY_HEADERNAME, arr));
                        break;
                    }

                    for (int i = 0; i < mh1.Count(); i++)
                    {
                        if (processedIndexList.Contains(i)) continue;

                        //各Historyでループ
                        var source = JArray.Parse(mh1.ElementAt(i).Value.ToString());

                        //documents がnullの場合は、そのまま追加
                        if (source[0]["documents"].Type == JTokenType.Null)
                        {
                            source[0]["isSelfHistory"] = source[0]["resourcePath"].ToString() == controllerUrl ? true : false;
                            bool isAlreadyAdded = false;
                            //ただし、重複防止のため、同じurl でdocuments null がある場合は、追加しない
                            resVal.ForEach(x => {
                                if (x["resourcePath"].ToString() == source[0]["resourcePath"].ToString()) isAlreadyAdded = true;
                            });
                            if (isAlreadyAdded) continue;
                            resVal.Add(source[0]);
                            processedIndexList.Add(i);
                            continue;
                        }

                        var target = JArray.Parse(source[0]["documents"].ToString());
                        for (int j = 0; j < mh1.Count(); j++)
                        {
                            var dest = JArray.Parse(mh1.ElementAt(j).Value.ToString());
                            if (i != j && source[0]["resourcePath"].ToString() == dest[0]["resourcePath"].ToString())
                            {
                                var destTarget = JArray.Parse(dest[0]["documents"].ToString());
                                destTarget.ForEach(x => {
                                    var skipFlg = false;
                                    for (int k = 0; k < target.Count; k++)
                                    {
                                        if (target[k]["documentKey"].ToString() == x["documentKey"].ToString() && target[k]["versionKey"].ToString() == x["versionKey"].ToString())
                                        {
                                            skipFlg = true;
                                        }
                                    }
                                    if (!skipFlg)
                                    {
                                        target.Add(x);
                                        processedIndexList.Add(j);
                                    }
                                });
                            }
                        }
                        source[0]["documents"] = target;
                        source[0]["isSelfHistory"] = source[0]["resourcePath"].ToString() == controllerUrl ? true : false;
                        resVal.Add(source[0]);
                        processedIndexList.Add(i);
                    }
                    ret.Add(new ResponseHeader(DOCUMENTHISTORY_HEADERNAME, resVal));
                }
                //他はそのまま
                else
                {
                    foreach (var m1 in mh1)
                    {
                        ret.Add(m1);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// レスポンスヘッダクラスを、レスポンスヘッダDictionaryに変換する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Dictionary<string, JToken> ToDictionary(List<ResponseHeader> data)
        {
            var ret = new Dictionary<string, JToken>();
            var mergedheader = data.GroupBy(x => x.Headername).ToList();
            foreach (var m1 in mergedheader)
            {
                var headervals = new List<JToken>();
                foreach (var m2 in m1)
                {
                    JToken j = m2.Value?.ToString().Replace("\n", "").Replace("\r", "").ToJson();
                    if (j != null && j.Type == JTokenType.Array)
                    {
                        foreach (var token in j)
                        {
                            headervals.Add(token.ToString().Replace("\n", "").Replace("\r", "").ToJson());
                        }
                    }
                    else
                    {
                        headervals.Add(m2.Value?.ToString().Replace("\n", "").Replace("\r", "").ToJson());
                    }
                }
                ret.Add(m1.Key, JArray.FromObject(headervals).ToJson());
            }
            return ret;
        }

        protected void CreateOrUpdateJsonErrorDictionary(IList<ValidationError> errors, ref List<RFC7807ProblemDetailExtendErrors> validationErros)
        {
            Dictionary<string, dynamic> problems = new Dictionary<string, dynamic>();
            var vals = problems;
            errors.ForEach(x =>
            {
                string prop = string.Empty;
                if (string.IsNullOrEmpty(x.Path) && x.Value == null)
                {
                    prop = "RootInvalid";
                }
                else
                {
                    //プロパティ設定のエラー以外の場合（required)
                    if (string.IsNullOrEmpty(x.Path))
                    {
                        var errProps = (List<string>)x.Value;
                        for (int i = 0; i < errProps.Count; i++)
                        {
                            prop += errProps[i];
                            if (i != (errProps.Count - 1))
                            {
                                prop += ",";
                            }
                        }
                    }
                    //プロパティ設定のエラーの場合
                    else
                    {
                        prop = x.Path;
                    }
                }

                //エラーリストに当該プロパティが存在の場合
                if (vals.ContainsKey(prop))
                {
                    var lst = vals[prop];
                    lst.Add($@"{x.Message}(code:{(int)x.ErrorType})");
                }
                //エラーリストに当該プロパティが非存在の場合
                else
                {
                    vals.Add(prop, new List<string>() { $@"{x.Message}(code:{(int)x.ErrorType})" });
                }
            });

            var error = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10402, this.RelativeUri?.Value);
            if (ReturnJsonValidatorErrorDetail || PerRequestDataContainer.ReturnNeedsJsonValidatorErrorDetail)
            {
                error.Errors = problems;
            }
            validationErros.Add(error);
        }

        protected string[] GetRemoveIgnoreFields()
        {
            if (IsOptimisticConcurrency != null && IsOptimisticConcurrency.Value)
            {
                return new string[1] { "_etag" };
            }
            return null;
        }

        protected void AddSnapshotToHistory(Dictionary<string, string> histHeader, List<JToken> resolvedReferenecDatas)
        {
            var histJson = histHeader.ContainsKey("X-DocumentHistory") ? histHeader["X-DocumentHistory"].ToJson() : null;
            if (histJson == null) return;
            if (resolvedReferenecDatas.Count == 0) return;
            List<JToken> hists = null;
            if (histJson.Type == JTokenType.Array)
            {
                hists = histJson.ToList();
            }
            else
            {
                hists = new List<JToken>() { histJson };
            }

            for (int i = 0; i < hists.Count; i++)
            {
                var varsionKey = Guid.NewGuid().ToString();
                var lst = hists[i]["documents"].ToList();
                for (int j = 0; j < lst.Count; j++)
                {
                    var id = hists[i]["documents"][j]["documentKey"].ToString();
                    var targetVersion = hists[i]["documents"][j]["versionKey"].ToString();
                    var documentKey = new DocumentKey(RepositoryKey, id);
                    var resultHistory = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(documentKey);
                    if (resultHistory != null)
                    {
                        var target = resultHistory.DocumentVersions.Where(x => x.VersionKey == targetVersion).First();
                        var jtoken = resolvedReferenecDatas.Where(x => x["id"].ToString().Replace("\r", "").Replace("\n", "") == id)?.ToList()[i];

                        if (jtoken == null) continue;

                        var param = ValueObjectUtil.Create<RegisterParam>(jtoken, this);
                        var result = HistoryEvacuationDataStoreRepository.RegisterOnce(param);
                        var path = (result.Additional["Container"] as string).UrlCombine(result.Value);
                        //現在の履歴に、lowperformanceのスナップショット追加
                        var snapshothist = new DocumentHistorySnapshot(DateTime.UtcNow, DocumentHistory.StorageLocationType.LowPerformance, HistoryEvacuationDataStoreRepository.RepositoryKeyInfo, path);
                        target = new DocumentHistory(target.VersionKey, target.VersionNo.Value, DateTime.Parse(target.CreateDate), target.OpenId, target.LocationType, DynamicApiDataStoreRepository[0].DocumentVersionRepository.RepositoryKeyInfo, target.Location, snapshothist);

                        DynamicApiDataStoreRepository[0].DocumentVersionRepository.UpdateDocumentVersion(documentKey, target);
                    }
                }
            }
        }

        /// <summary>
        /// 自身の履歴OFFでもReference先の履歴がONの場合は、自身に空のヘッダを作る
        /// </summary>
        /// <param name="header"></param>
        /// <param name="refHeaders"></param>
        /// <returns></returns>
        protected (Dictionary<string, string>, List<ResponseHeader>) MakeEmptyHistoryHeaderIfNoHistory(Dictionary<string, string> header, List<ResponseHeader> refHeaders)
        {
            if (EnableJsonDocumentHistory == true && (this.IsDocumentHistory == null || !this.IsDocumentHistory.Value) && !header.ContainsKey(DOCUMENTHISTORY_HEADERNAME))
            {
                var refSourceIsHistoryOn = false;
                refHeaders.ForEach(x => {
                    if (x.Headername == DOCUMENTHISTORY_HEADERNAME)
                    {
                        var data = x.Value.ToJson();
                        //履歴がONかどうかは、documentsがnullでないかどうか
                        if (data[0]["documents"].Type != JTokenType.Null)
                        {
                            refSourceIsHistoryOn = true;
                        }
                    }
                });
                //Notify先が複数あっても、１つでも履歴ONのNotify先があれば、自身に空の履歴ヘッダを作る
                if (refSourceIsHistoryOn)
                {
                    var ret = new Dictionary<string, string>();
                    var hist = new DocumentHistoryHeader(true, this.ControllerRelativeUrl.Value, null);
                    var jarr = new JArray();
                    jarr.Add(hist.ToJson());
                    ret.Add(DOCUMENTHISTORY_HEADERNAME, jarr.ToString());
                    return (ret, refHeaders);
                }
                //自身もNotify先も履歴設定が無い場合は、履歴ヘッダ不要
                else
                {
                    refHeaders = new List<ResponseHeader>();
                    return (header, refHeaders);
                }
            }
            //自身が履歴設定されている場合は何もしない
            return (header, refHeaders);
        }

        /// <summary>
        /// IDをチェックし、アクセス者がこのドキュメントにアクセス可能か(ベンダー領域超えをしていないか)チェックする
        /// </summary>
        /// <param name="id">アクセスしようとしているドキュメントのID</param>
        /// <returns>true:ドキュメントにアクセス可能、false：ドキュメントにアクセス不可能</returns>
        protected bool CheckRequestIdIsValid(string id)
        {
            //dummyの登録データ作成
            var registerParam = ValueObjectUtil.Create<RegisterParam>(this, "{'__dummy__':null}".ToJson(), new XResourceSharingPerson(PerRequestDataContainer.XResourceSharingPerson), new XResourceSharingWith(PerRequestDataContainer.XResourceSharingWith));
            //プライマリリポジトリ
            var repository = DynamicApiDataStoreRepository[0];
            //IDチェック
            return repository.KeyManagement.IsIdValid(id.ToJValue(), registerParam, repository.ResourceVersionRepository, out DocumentDataId _);
        }

        /// <summary>
        /// スペースを16進数からいつものスペースに変換する
        /// </summary>
        /// <param name="queryParam">変換前のQueryParam</param>
        /// <param name="outQueryParam">変換後のQueryParam</param>
        /// <returns>true:変換ができた、false:変換がないorできない</returns>
        protected bool ConvertHexToSpace(QueryParam queryParam, out QueryParam outQueryParam)
        {
            outQueryParam = null;
            if (queryParam.QueryString == null) return false;

            var queryDic = new Dictionary<QueryStringKey, QueryStringValue>();
            var hasHex = false;
            foreach (var x in queryParam.QueryString.Dic)
            {
                if (x.Value.Value.ToString().EndsWith(SPACE_HEX))
                {
                    queryDic.Add(x.Key, new QueryStringValue(x.Value.Value.ToString().Replace(SPACE_HEX, " ")));
                    hasHex = true;
                    continue;
                }
                queryDic.Add(x.Key, x.Value);
            }
            if (hasHex)
            {
                outQueryParam = new QueryParam(queryParam.VendorId, queryParam.SystemId, queryParam.OpenId, queryParam.ControllerSchema,
                                    queryParam.UriSchema, queryParam.RequestSchema, queryParam.ResponseSchema, queryParam.RepositoryKey,
                                    queryParam.ApiQuery, queryParam.PostDataType, queryParam.ActionType, queryParam.PartitionKey,
                                    queryParam.KeyValue, queryParam.IsVendor, queryParam.IsPerson, queryParam.IsOverPartition,
                                    queryParam.IsAutomaticId, new QueryStringVO(queryDic), queryParam.QueryType, queryParam.ODataQuery,
                                    queryParam.NativeQuery, queryParam.ApiResourceSharing, queryParam.ResourceSharingPersonRules,
                                    queryParam.XResourceSharingWith, queryParam.XResourceSharingPerson, queryParam.XVersion,
                                    queryParam.HasSingleData, queryParam.CacheInfo, queryParam.XRequestContinuation, queryParam.IsDocumentHistory,
                                    queryParam.Identification, queryParam.SelectCount, queryParam.SkipCount,
                                    queryParam.IsContainerDynamicSeparation, queryParam.ControllerId,
                                    queryParam.OperationInfo, queryParam.IsOtherResourceSqlAccess, queryParam.IsOptimisticConcurrency);
            }

            return hasHex;
        }


        private string GetFileExtenstion(string filename)
        {
            //ピリオド多数あっても、一番右のを取ってくれる
            var ext = Path.GetExtension(filename);
            //GetExtensionは、「.」付きで返るので、「.」を消す
            return (string.IsNullOrEmpty(ext) ? null : ext.Replace(".", ""));
        }

        /// <summary>
        /// リクエストのコンテントタイプが、リストの中にあるか(正規表現OK)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="checkList"></param>
        /// <returns></returns>
        private bool IsExistContentTypeInList(string value, List<string> checkList)
        {
            return IsMatchContentInCheckList(value, checkList);
        }

        /// <summary>
        /// リクエストのファイル拡張子が、リストの中にあるか(正規表現OK)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="checkList"></param>
        /// <param name="isCheckOkList"></param>
        /// <param name="isUploadOk_NoExtensionFile"></param>
        /// <returns></returns>
        private bool IsExistFileExtensionInList(string value, List<string> checkList, bool isCheckOkList, bool isUploadOk_NoExtensionFile)
        {
            //拡張子があるかどうか
            if (string.IsNullOrEmpty(value))
            {
                //無い場合、どうするか
                //OKリストをチェックしている場合
                if (isCheckOkList)
                {
                    //拡張子なしをOKとするか
                    if (isUploadOk_NoExtensionFile)
                    {
                        //拡張子無しをOKとする
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //ブロックリストチェック中の場合
                if (!isCheckOkList)
                {
                    //ブロックリストチェックは、trueを返すとブロックするので
                    //拡張子無しアップロードOKなら、falseを返す
                    if (isUploadOk_NoExtensionFile)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            //リスト検索
            return IsMatchContentInCheckList(value, checkList);
        }

        private bool IsMatchContentInCheckList(string content, List<string> checkList)
        {
            foreach (var c1 in checkList)
            {
                //先に、コンフィグで指定されているチェックリストの各要素の末尾が記号かどうか(正規表現終わりかどうか)チェック
                var reg = new Regex("[^0-9A-Za-z]$");
                var match = reg.Match(c1);
                //末尾が記号(正規表現終わり)だったら、完全一致でなく、指定の正規表現に任せる($付けない)
                var regexStr = match.Success ? $"^{c1}" : $"^{c1}$";
                reg = new Regex(regexStr, RegexOptions.IgnoreCase);
                match = reg.Match(content);
                if (match.Success)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// コンテントタイプとファイル名をチェックし、処理続行して良いかチェック
        /// </summary>
        /// <param name="contenttype"></param>
        /// <param name="filename"></param>
        /// <returns>true: OK  false:ブロック</returns>
        public bool IsContentOkByContentTypeAndFileName(string contenttype, string filename)
        {
            //OKリスト、ブロックリストどちらもチェックし、優先するのはどれか決める
            var checkOkContentType = IsExistContentTypeInList(contenttype, UploadOK_ContentTypeList);
            var checkOkExtension = IsExistFileExtensionInList(GetFileExtenstion(filename), UploadOK_ExtensionList, true, IsUploadOk_NoExtensionFile);
            var checkBlockContentType = IsExistContentTypeInList(contenttype, BlockContentTypeList);
            var checkBlockExtension = IsExistFileExtensionInList(GetFileExtenstion(filename), BlockExtensionList, false, IsUploadOk_NoExtensionFile);

            //OKリストでのチェック
            if ((UploadOK_ContentTypeList.Count != 0 || UploadOK_ExtensionList.Count != 0)
                    && (BlockContentTypeList.Count == 0 && BlockExtensionList.Count == 0))
            {
                if ((UploadOK_ContentTypeList.Count != 0 && !checkOkContentType) || (UploadOK_ExtensionList.Count != 0 && !checkOkExtension))
                {
                    return false;
                }
            }
            //ブロックリストでチェック
            else if (UploadOK_ContentTypeList.Count == 0 && UploadOK_ExtensionList.Count == 0
                    && (BlockContentTypeList.Count != 0 || BlockExtensionList.Count != 0))
            {
                if ((BlockContentTypeList.Count != 0 && checkBlockContentType) || (BlockExtensionList.Count != 0 && checkBlockExtension))
                {
                    return false;
                }
            }
            else
            {
                //組み合わせ系、優先フラグを見る
                //コンテントタイプ
                if (checkOkContentType == true && checkBlockContentType == true)
                {
                    //ブロックリストが優先の場合は、ここでリターン
                    if (IsPriorityHigh_OKList == false)
                    {
                        return false;
                    }
                }
                else if (checkOkContentType == false && checkBlockContentType == false)
                {
                    //どちらもヒットしなかった場合は、
                    //OKリストが優先ならブロック -> OKリストに無かったので
                    //ブロックリストが優先なら通す -> ブロックリストに無かったので
                    if (IsPriorityHigh_OKList == true)
                    {
                        return false;
                    }
                }
                //拡張子
                if (checkOkExtension == true && checkBlockExtension == true)
                {
                    //ブロックリストが優先の場合は、ここでリターン
                    if (IsPriorityHigh_OKList == false)
                    {
                        return false;
                    }
                }
                else if (checkOkExtension == false && checkBlockExtension == false)
                {
                    //どちらもヒットしなかった場合は、
                    //OKリストが優先ならブロック -> OKリストに無かったので
                    //ブロックリストが優先なら通す -> ブロックリストに無かったので
                    if (IsPriorityHigh_OKList == true)
                    {
                        return false;
                    }
                }
                //組み合わせ(ブロックリスト有り、OKリスト無し = ブロック)
                if ((checkBlockContentType == true && checkOkContentType == false)
                    || checkBlockExtension == true && checkOkExtension == false)
                {
                    //基本ブロックが強いが、OKリスト優先で且つ、OKリストにtrueがあれば、OKにする
                    if ((checkBlockContentType == true || checkBlockExtension == true) &&
                        (checkOkExtension == true || checkOkExtension))
                    {
                        if (IsPriorityHigh_OKList)
                        {
                            return true;
                        }
                    }
                    //ブロック
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// コンテントタイプをチェックし、処理続行して良いかチェック
        /// </summary>
        /// <param name="contenttype"></param>
        /// <returns>true: OK  false:ブロック</returns>
        public bool IsContentOkByContentType(string contenttype)
        {
            //OKリスト、ブロックリストどちらもチェックし、優先するのはどれか決める
            var checkOkContentType = IsExistContentTypeInList(contenttype, UploadOK_ContentTypeList);
            var checkBlockContentType = IsExistContentTypeInList(contenttype, BlockContentTypeList);

            //OKリストでのチェック
            if ((UploadOK_ContentTypeList.Count != 0)
                    && (BlockContentTypeList.Count == 0))
            {
                if ((UploadOK_ContentTypeList.Count != 0 && !checkOkContentType))
                {
                    return false;
                }
            }
            //ブロックリストでチェック
            else if (UploadOK_ContentTypeList.Count == 0
                    && BlockContentTypeList.Count != 0)
            {
                if ((BlockContentTypeList.Count != 0 && checkBlockContentType))
                {
                    return false;
                }
            }
            else
            {
                //組み合わせ系、優先フラグを見る
                //コンテントタイプ
                if (checkOkContentType == true && checkBlockContentType == true)
                {
                    //ブロックリストが優先の場合は、ここでリターン
                    if (IsPriorityHigh_OKList == false)
                    {
                        return false;
                    }
                }
                else if (checkOkContentType == false && checkBlockContentType == false)
                {
                    //どちらもヒットしなかった場合は、
                    //OKリストが優先ならブロック -> OKリストに無かったので
                    //ブロックリストが優先なら通す -> ブロックリストに無かったので
                    if (IsPriorityHigh_OKList == true)
                    {
                        return false;
                    }
                }
                //組み合わせ(ブロックリスト有り、OKリスト無し = ブロック)
                if (checkBlockContentType == true && checkOkContentType == false)
                {
                    //基本ブロックが強いが、OKリスト優先で且つ、OKリストにtrueがあれば、OKにする
                    if (IsPriorityHigh_OKList)
                    {
                        return true;
                    }
                    //ブロック
                    return false;
                }
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.HashAlgorithm;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Models.Async;
using JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage;
using Oci.ObjectstorageService.Responses;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    internal class AsyncDyanamicApiRepositoryOci : AbstractRepository, IAsyncDyanamicApiRepository
    {
        #region CACHEKEY
        private static string CACHE_KEY_ASYNCAPIREQUESTID_BYOPENID_CONTROLLERID = "AsyncDyanamicApiRepository:AsyncApiRequestId";
        private readonly TimeSpan cacheExpireTimeByOpenId = TimeSpan.FromDays(7);
        #endregion

        #region DI
        private static IConfiguration s_appConfig = UnityCore.Resolve<IConfiguration>().GetSection("AppConfig");

        private Lazy<IDynamicApiInterface> _lazyDynamicApiInterface = new Lazy<IDynamicApiInterface>(() => UnityCore.Resolve<IDynamicApiInterface>());
        private IDynamicApiInterface _dynamicApiInterface { get => _lazyDynamicApiInterface.Value; }

        private Lazy<ICache> _lazyCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("CsvDownloadCache"));
        private ICache _cache { get => _lazyCache .Value; }

        private Lazy<IOciObjectStorage> _lazyObjectStorage = new Lazy<IOciObjectStorage>(() => UnityCore.Resolve<IOciObjectStorage>("AsyncDynamicApiObjectStorage"));
        private IOciObjectStorage _objectStorage { get => _lazyObjectStorage.Value; }
        #endregion


        public AsyncApiResult GetStatus(AsyncRequestId asyncRequestId)
        {
            // X-Version指定は無視する
            this.PerRequestDataContainer.Xversion = 1;
            var requestModel = new DynamicApiRequestModel
            {
                HttpMethod = "GET",
                RelativeUri = string.Format(s_appConfig.GetValue<string>("AsyncApiGetStatusUrl"), asyncRequestId.Value),
                MediaType = MediaTypeConst.ApplicationJson
            };
            var result = _dynamicApiInterface.Request(requestModel, true);
            //データがない場合は別途例外を上げる。
            if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new AsyncApiStatusNotFoundException();
            }
            else if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApiException(result.ToHttpResponseMessage());
            }

            var jsonModel = JsonConvert.DeserializeObject<AsyncResultJsonModel>(result.Contents.ReadAsString());
            return new AsyncApiResult(jsonModel.RequestId, jsonModel.Status, jsonModel.RequestDate, jsonModel.EndDate, jsonModel.ResultPath);
        }

        /// <summary>
        /// キャッシュからOpenIdとControllerID別に非同期APIのリクエストを検索する
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="controllerId"></param>
        /// <remarks>misshit時でも何もしない nullを返す</remarks>
        /// <returns></returns>
        public AsyncRequestId GetRequestIdFromCache(OpenId openId, ControllerId controllerId) =>
            _cache.Get<AsyncRequestId>(CacheManager.CreateKey(CACHE_KEY_ASYNCAPIREQUESTID_BYOPENID_CONTROLLERID, openId.Value, controllerId.Value), out bool isNullValue);

        public HttpResponseMessage GetResult(AsyncRequestId asyncRequestId)
        {
            var requestContinuation = PerRequestDataContainer.XRequestContinuation;
            PerRequestDataContainer.XRequestContinuation = "";
            var status = GetStatus(asyncRequestId);
            if (status.Status != AsyncStatus.End && status.Status != AsyncStatus.Error)
            {
                throw new AsyncApiNotEndException();
            }

            var container = status.ResultPath.Split('/')[0];
            var path = string.Join("/", status.ResultPath.Split('/').Skip(1));

            try
            {
                GetObjectResponse bodyObject;
                GetObjectResponse headObject;
                if (string.IsNullOrEmpty(requestContinuation))
                {
                    bodyObject = _objectStorage.GetAppendObjectAsync(container, path + "/Body.bin").Result;
                    headObject = _objectStorage.GetAppendObjectAsync(container, path + "/Header.bin").Result;
                }
                else
                {
                    var requestContinuationPath = HashCalculation.ComputeHashString(Encoding.Unicode.GetBytes(requestContinuation));
                    bodyObject = _objectStorage.GetAppendObjectAsync(container, path + "/" + requestContinuationPath + "/Body.bin").Result;
                    headObject = _objectStorage.GetAppendObjectAsync(container, path + "/" + requestContinuationPath + "/Header.bin").Result;
                }
                var content = new StreamContent(bodyObject.InputStream);
                var headerJson = new StreamReader(headObject.InputStream).ReadToEnd().ToJson();
                //旧形式のヘッダ出来た場合は詰め替える
                if (headerJson != null && headerJson["HttpHeaders"].Type == JTokenType.Array)
                {
                    var json = (JObject)"{'Dummy':null}".ToJson();
                    foreach(JObject p1 in headerJson["HttpHeaders"])
                    {
                        foreach(var p2 in p1.Properties())
                        {
                            json.First.AddAfterSelf(new JProperty(p2.Name, p1.GetPropertyValue(p2.Name)));
                        }
                    }
                    headerJson["HttpHeaders"] = json.RemoveField("Dummy");
                }
                var header = JsonConvert.DeserializeObject<AsyncResultHeaderModel>(headerJson?.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue(header.MediaType)
                {
                    CharSet = header.CharSet
                };

                var res = new HttpResponseMessage() { StatusCode = header.StatusCode, Content = content };
                if (header.HttpHeaders != null)
                {
                    foreach (var kv in header.HttpHeaders.Where(kv => !res.Headers.Contains(kv.Key)).Select(kv => kv))
                    {
                        res.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                    }
                }

                return res;
            }
            catch (AggregateException ae) when (ae.InnerException is StorageException)
            {
                var se = ae.InnerException as StorageException;
                if (se.RequestInformation.HttpStatusCode == (int)System.Net.HttpStatusCode.NotFound)
                {
                    throw new AsyncApiResultNotFoundException();
                }

                throw;
            }
            catch (StorageException se) when (se.RequestInformation.HttpStatusCode ==
                                              (int)System.Net.HttpStatusCode.NotFound)
            {
                throw new AsyncApiResultNotFoundException();
            }

        }

        public void Request(AsyncRequestId requestId, JsonDocument requestBody, JsonDocument asyncLog)
        {
            // 非同期リクエストデータの登録時はX-Version=1とする。
            // リクエストで指定されたX-VersionはrequestBodyに含まれており、
            // 非同期バッチではリクエストで指定されたX-VersionでAPIが実行される。
            this.PerRequestDataContainer.Xversion = 1;
            this.PerRequestDataContainer.XAsync = false;
            RequestDynamicApiPost(requestBody, s_appConfig.GetValue<string>("AsycRequestBodyUrl"));
            RequestDynamicApiPost(asyncLog, s_appConfig.GetValue<string>("AsyncLogUrl"));

            var eventHubData = new JObject();
            eventHubData.Add(new JProperty("RequestId", requestId.Value));
            RequestDynamicApiPost(new JsonDocument(eventHubData), s_appConfig.GetValue<string>("AsyncEventHubUrl"));
        }

        public void RequestOnCache(AsyncRequestId requestId, JsonDocument requestBody, JsonDocument asyncLog, ControllerId controllerId)
        {
            Request(requestId, requestBody, asyncLog);
            _cache.Add(CacheManager.CreateKey(CACHE_KEY_ASYNCAPIREQUESTID_BYOPENID_CONTROLLERID, this.PerRequestDataContainer.OpenId, controllerId.Value), requestId, cacheExpireTimeByOpenId);
        }

        public bool SetResult(Stream content, string objectPath, string accept)
        {
            var container = objectPath.Split('/')[0];
            var objectName = string.Join("/", objectPath.Split('/').Skip(1));
            _ = _objectStorage.AppendObjectAsync(container, objectName, content, accept).Result;

            return true;
        }

        public bool SetResultOverwrite(Stream content, string objectPath, string accept)
        {
            var container = objectPath.Split('/')[0];
            var objectName = string.Join("/", objectPath.Split('/').Skip(1));
            _ = _objectStorage.OverwriteAppendObjectAsync(container, objectName, content, accept).Result;

            return true;
        }

        public bool SetResultOverwrite(string content, string objectPath, string accept)
        {
            var container = objectPath.Split('/')[0];
            var objectName = string.Join("/", objectPath.Split('/').Skip(1));
            _ = _objectStorage.OverwriteAppendObjectAsync(container, objectName, content, accept).Result;

            return true;
        }


        private DynamicApiResponse RequestDynamicApiPost(JsonDocument registData, string url)
        {
            var requestModel = new DynamicApiRequestModel
            {
                RelativeUri = url,
                Contents = registData.Value.ToString(),
                MediaType = MediaTypeConst.ApplicationJson,
                HttpMethod = "POST"
            };
            var response = _dynamicApiInterface.Request(requestModel, true);
            if ((int)response.StatusCode >= 400 )
            {
                throw new ApiException(new HttpResponseMessage(response.StatusCode));
            }
            return response;
        }
    }
}

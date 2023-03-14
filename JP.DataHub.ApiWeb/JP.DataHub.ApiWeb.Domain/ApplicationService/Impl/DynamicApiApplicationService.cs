using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web;
using StackExchange.Profiling;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Interception.Utilities;
using Newtonsoft.Json;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    internal class DynamicApiApplicationService : IDynamicApiApplicationService
    {
        private IConfiguration _configuration { get; } = UnityCore.Resolve<IConfiguration>();

        [Dependency]
        public IDynamicApiRepository DynamicApiRepository { get; set; }

#if Oracle
        [Dependency("AsyncDyanamicApiRepositoryOci")]
        public IAsyncDyanamicApiRepository AsyncDyanamicApiRepository { get; set; }
#else
        [Dependency]
        public IAsyncDyanamicApiRepository AsyncDyanamicApiRepository { get; set; }
#endif

        private readonly Lazy<IPerRequestDataContainer> _lazyPerRequestDataContainer = new Lazy<IPerRequestDataContainer>(() => 
            {
                try
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>();
                }
                catch
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                }
            });

        private IPerRequestDataContainer _perRequestDataContainer => _lazyPerRequestDataContainer.Value;

        public DynamicApiApplicationService()
        {
        }

        public DynamicApiResponse Request([Required] HttpMethodType httpMethodType, [Required] RequestRelativeUri relativeUri, [Required] Contents contents, QueryString queryString, HttpHeader header, MediaType mediaType, Accept accept, ContentRange contentRange, ContentType contentType, ContentLength contentLength, NotAuthentication? notAuthentication = null)
        {
            notAuthentication = notAuthentication ?? new NotAuthentication(false);
            var api = DynamicApiRepository.FindApi(httpMethodType, relativeUri, new GetQuery(HttpUtility.UrlDecode(queryString.Value)));
            HttpResponseMessage result;
            if (api == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                var dc = DataContainerUtil.ResolveDataContainer() as IPerRequestDataContainer;
                dc.ActionId = api.ApiId.Value;
                dc.ControllerId = api.ControllerId.Value;
                result = api.Request(new ActionId(Guid.NewGuid().ToString()), mediaType, notAuthentication, contents, accept, contentRange);
            }
            SqlToHeader(result);
            return new DynamicApiResponse(result);
        }

        private string OutputCommand(CustomTiming ct)
        {
            var str = ct.CommandString.NoCRLF();
            return str.Length > 200 ? str.Substring(0, 197) + "..." : str;
        }

        private void SqlToHeader(HttpResponseMessage response)
        {
            // OutputToHeaderの値によって出力するかどうかを決定する
            // true : 無条件に出力する
            // request : RequestヘッダーにX-Profilerがある場合には出力する
            // それ以外 : 出力しない
            var mode = _configuration.GetValue<string>("Profiler:OutputToHeader");
            if (mode == "true" || (mode == "request" && _perRequestDataContainer.XProfiler == true))
            {
                if (MiniProfiler.Current?.Root?.CustomTimings != null)
                {
                    foreach (var c in MiniProfiler.Current?.Root?.CustomTimings)
                    {
                        for (int i = 0; i < c.Value.Count; i++)
                        {
                            var item = c.Value[i];
                            response.Headers.Add($"X-Profiler{c.Key.ToPascal()}{i + 1}", $"{item.DurationMilliseconds}ms {OutputCommand(item)}");
                        }
                    }
                }
            }
        }

        public HttpResponseMessage Get(Contents contents, MediaType mediaType, RequestRelativeUri requestRelativeUri, GetQuery query, NotAuthentication notAuthentication, Accept accept)
        {
            var api = DynamicApiRepository.FindApi(new HttpMethodType(HttpMethodType.MethodTypeEnum.GET), requestRelativeUri, query);
            HttpResponseMessage result;
            if (api == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                result = api.Request(new ActionId(Guid.NewGuid().ToString()), mediaType, notAuthentication, contents, accept);
            }
            SqlToHeader(result);
            return result;
        }

        public HttpResponseMessage Post(Contents contents, MediaType mediaType, RequestRelativeUri requestRelativeUri, NotAuthentication notAuthentication, Accept accept, ContentRange contentRange = null)
        {
            var api = DynamicApiRepository.FindApi(new HttpMethodType(HttpMethodType.MethodTypeEnum.POST), requestRelativeUri);
            HttpResponseMessage result;
            if (api == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                result = api.Request(new ActionId(Guid.NewGuid().ToString()), mediaType, notAuthentication, contents, accept, contentRange);
            }
            SqlToHeader(result);
            return result;
        }

        public HttpResponseMessage Put(Contents contents, MediaType mediaType, RequestRelativeUri requestRelativeUri, NotAuthentication notAuthentication, Accept accept)
        {
            var api = DynamicApiRepository.FindApi(new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT), requestRelativeUri);
            HttpResponseMessage result;
            if (api == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                result = api.Request(new ActionId(Guid.NewGuid().ToString()), mediaType, notAuthentication, contents, accept);
            }
            SqlToHeader(result);
            return result;
        }

        public HttpResponseMessage Delete(Contents contents, MediaType mediaType, RequestRelativeUri requestRelativeUri, GetQuery query, NotAuthentication notAuthentication, Accept accept)
        {
            var api = DynamicApiRepository.FindApi(new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE), requestRelativeUri, query);
            HttpResponseMessage result;
            if (api == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                result = api.Request(new ActionId(Guid.NewGuid().ToString()), mediaType, notAuthentication, contents, accept);
            }
            SqlToHeader(result);
            return result;
        }

        public HttpResponseMessage Patch(Contents contents, MediaType mediaType, RequestRelativeUri requestRelativeUri, GetQuery query, NotAuthentication notAuthentication, Accept accept)
        {
            var api = DynamicApiRepository.FindApi(new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH), requestRelativeUri, query);
            HttpResponseMessage result;
            if (api == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                result = api.Request(new ActionId(Guid.NewGuid().ToString()), mediaType, notAuthentication, contents, accept);
            }
            SqlToHeader(result);
            return result;
        }

        public AsyncApiResult GetStatus(AsyncRequestId requestId)
        {
            return AsyncDyanamicApiRepository.GetStatus(requestId);
        }

        public AsyncRequestId GetRequestIdFromCache(OpenId openId, ControllerId controllerId)
        {
            return AsyncDyanamicApiRepository.GetRequestIdFromCache(openId, controllerId);
        }

        public DynamicApiResponse GetResult(AsyncRequestId requestId)
        {
            var result = AsyncDyanamicApiRepository.GetResult(requestId);
            SqlToHeader(result);
            return new DynamicApiResponse(result);
        }

        public HttpResponseMessage GetDataFromExecuteApiInfo(ExecuteApiInfo executeApiInfo, MediaType mediaType, NotAuthentication notAuthentication)
        {
            var queryStringDictionary = new Dictionary<QueryStringKey, QueryStringValue>();
            JsonConvert.DeserializeObject<string[][]>(executeApiInfo.QueryString)?.ForEach(queryString =>
            {
                queryStringDictionary.Add(new QueryStringKey(queryString[0]), new QueryStringValue(queryString[1]));
            });

            var keyValueDictionary = new Dictionary<UrlParameterKey, UrlParameterValue>();
            JsonConvert.DeserializeObject<string[][]>(executeApiInfo.KeyValue)?.ForEach(queryString =>
            {
                keyValueDictionary.Add(new UrlParameterKey(queryString[0]), new UrlParameterValue(queryString[1]));
            });
            var api = DynamicApiRepository.FindApiForGetExecuteApiInfo(
                new ControllerId(executeApiInfo.ControllerId),
                new ApiId(executeApiInfo.ApiId),
                new DataId(executeApiInfo.DataId),
                new Contents(executeApiInfo.Contents),
                new QueryStringVO(queryStringDictionary),
                new UrlParameter(keyValueDictionary)
            );

            HttpResponseMessage result;
            if (api == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                var contentString = "";
                result = api.Request(
                    new ActionId(Guid.NewGuid().ToString()),
                    mediaType,
                    notAuthentication,
                    new Contents(contentString));
            }
            SqlToHeader(result);
            return result;
        }

        public IEnumerable<string> GetEnumerable(Contents contents, MediaType mediaType, RequestRelativeUri requestRelativeUri, GetQuery query, NotAuthentication notAuthentication)
        {
            var api = DynamicApiRepository.FindEnumerableApi(new HttpMethodType(HttpMethodType.MethodTypeEnum.GET), requestRelativeUri, query);
            if (api == null)
            {
                throw new ApiException(new HttpResponseMessage(HttpStatusCode.NotImplemented));
            }

            var result = api.Request(new ActionId(Guid.NewGuid().ToString()), mediaType, notAuthentication, contents);
            var resultContent = result.Content as EnumerableQueryContent;
            if (resultContent == null)
            {
                throw new ApiException(new HttpResponseMessage(HttpStatusCode.NotImplemented));
            }

            return resultContent.GetContent();
        }

        public bool SetResult(Stream content, string blobPath, string accept)
        {
            return AsyncDyanamicApiRepository.SetResult(content, blobPath, accept);
        }

        public bool SetResultOverwrite(Stream content, string blobPath, string accept)
        {
            return AsyncDyanamicApiRepository.SetResultOverwrite(content, blobPath, accept);
        }

        public bool SetResultOverwrite(string content, string blobPath, string accept)
        {
            return AsyncDyanamicApiRepository.SetResultOverwrite(content, blobPath, accept);
        }


        /// <summary>
        /// DynamicApiAttachFileから任意のファイルをDLする
        /// </summary>
        /// <remarks>
        /// 外部のユーザーに実行させてはいけない 内部向けのメソッドのため ファイルのバリデーションを一部省略している
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="blobRepositoryInfo"></param>
        /// <returns></returns>
        public Stream DownloadDynamicApiAttachFile(DynamicApiAttachFileInformation info, RepositoryInfo blobRepositoryInfo)
        {
            if (blobRepositoryInfo.Type != RepositoryType.AttachFileBlob) { throw new InvalidOperationException("invalid repositorytype"); }
            var repository = UnityCore.Resolve<IDynamicApiDataStoreRepositoryFactory>().NewDataStoreRestore(blobRepositoryInfo.Type, blobRepositoryInfo, true.ToIsEnableResourceVersion());
            if (repository is IDynamicApiAttachFileRepository attachFileRepository)
            {
                return attachFileRepository.GetAttachFileFileStream(info);
            }
            return null;
        }

        /// <summary>
        /// DynamicApiAttachFileの任意のファイルを削除する
        /// </summary>
        /// <remarks>
        /// 外部のユーザーに実行させてはいけない 内部向けのメソッドのため ファイルのバリデーションを一部省略している
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="repositoryInfo"></param>
        /// <returns></returns>
        public void DeleteDynamicApiAttachFile(DynamicApiAttachFileInformation info, RepositoryInfo repositoryInfo)
        {
            if (repositoryInfo.Type != RepositoryType.AttachFileBlob) { throw new InvalidOperationException("invalid repositorytype"); }
            var repository = UnityCore.Resolve<IDynamicApiDataStoreRepositoryFactory>().NewDataStoreRestore(repositoryInfo.Type, repositoryInfo, true.ToIsEnableResourceVersion());
            if (repository is IDynamicApiAttachFileRepository attachFileRepository)
            {
                attachFileRepository.DeleteAttachFile(info);
            }
        }


        /// <summary>
        /// リソースURLからデータモデル(文字列)を返す
        /// </summary>
        /// <param name="url">リソースURL</param>
        /// <returns>データモデル文字列</returns>
        public string GetControllerSchemaByUrl(string url) => DynamicApiRepository.GetControllerSchemaByUrl(new ControllerUrl(url))?.Value;

        /// <summary>
        /// モデル名からデータモデル（文字列）を返す
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetSchemaModelByName(string name) => DynamicApiRepository.GetSchemaModelByName(new DataSchemaName(name))?.Value;
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoMapper;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class DeleteDataAction : AbstractDynamicApiAction, IEntity
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
        }).CreateMapper();


        public override HttpResponseMessage ExecuteAction()
        {
            if ((this.MethodType.Value != HttpMethodType.MethodTypeEnum.GET)
                && (this.MethodType.Value != HttpMethodType.MethodTypeEnum.POST)
                && (this.MethodType.Value != HttpMethodType.MethodTypeEnum.PUT)
                && (this.MethodType.Value != HttpMethodType.MethodTypeEnum.DELETE))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10419, this.RelativeUri?.Value); ;
            }

            if (string.IsNullOrEmpty(this.RepositoryKey.Value))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10420, this.RelativeUri?.Value); ;
            }

            // スキーマのチェック
            HttpResponseMessage schemaCheckResponse;
            if (IsValidUrlModelSchema(out schemaCheckResponse) == false)
            {
                return schemaCheckResponse;
            }

            var parallesExceptions = new ConcurrentQueue<Exception>();
            var lockObject = new object();
            bool result = false;
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            histHeaders = new List<ResponseHeader>();
            Parallel.ForEach(
                this.DynamicApiDataStoreRepository,
                repository =>
                {
                    var threadPerRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                    mapper.Map<IPerRequestDataContainer, IPerRequestDataContainer>(perRequestDataContainer, threadPerRequestDataContainer);

                    bool ret;
                    try
                    {
                        Action<JToken, RepositoryType> callbackDelete = null;
                        if ((EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true) || this.IsEnableBlockchain?.Value == true)
                        {
                            PerRequestDataContainer.XgetInternalAllField = true;
                            callbackDelete = (JToken token, RepositoryType repositoryType) => DeleteCallback(token, repositoryType);
                        }
                        //削除対象を取得
                        var queryParam = ValueObjectUtil.Create<QueryParam>(this);
                        var queryResult = repository.QueryEnumerable(queryParam).ToList();
                        ret = queryResult.Any();

                        if (!ret && queryParam.QueryString != null)
                        {
                            //0件のときに、パラメータのスペースの16進数変換を行い再取得する
                            if (ConvertHexToSpace(queryParam, out var queryParam2))
                            {
                                queryResult = repository.QueryEnumerable(queryParam2).ToList();
                                ret = queryResult.Any();
                            }
                        }

                        foreach (var data in queryResult)
                        {
                            var id = data.Value[ID].ToString();
                            repository.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(data.Value, this, callbackDelete));
                            DeleteBase64AttachFiles(id);
                        }
                        // メールテンプレートまたはWebhookがある場合、イベントハブに通知する
                        var enableWebHook = UnityCore.Resolve<bool>("EnableWebHookAndMailTemplate");
                        if (ret && enableWebHook == true && (HasMailTemplate?.Value == true || HasWebhook?.Value == true))
                        {
                            var eventHubRepository = UnityCore.Resolve<IResourceChangeEventHubStoreRepository>();
                            eventHubRepository.Delete(this);
                        }
                    }
                    catch (NotImplementedException exception)
                    {
                        parallesExceptions.Enqueue(exception);
                        ret = false;
                    }
                    lock (lockObject)
                    {
                        if (ret)
                        {
                            result = true;
                        }
                    }
                }
            );

            // remove cache
            string keyCache = CreateResourceCacheKey();
            if (keyCache != null)
            {
                //自身のリソースのキャッシュを削除する
                var task = RefreshApiResourceCache(keyCache);
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        this.Logger.Error(t.Exception.InnerException);
                    }
                });
            }

            if (result)
            {
                if (histHeaders.Count != 0)
                {
                    Dictionary<string, string> histheaders = new Dictionary<string, string>();
                    //自身の履歴ヘッダリストを、自身の空ディクショナリにマージする
                    histheaders = MergeResponseHeader(histheaders, ToDictionary(MergeHistoryRefSourceHeader(this.ControllerRelativeUrl.Value, histHeaders)));
                    return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.NoContent, null, histheaders));
                }
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, null));
            }
            if ((parallesExceptions.Count > 0) && (this.DynamicApiDataStoreRepository.Count == parallesExceptions.Count))
            {
                if (parallesExceptions.Count == 1)
                {
                    Exception exception;
                    parallesExceptions.TryDequeue(out exception);
                    throw exception;
                }
                else
                {
                    throw new AggregateException(parallesExceptions);
                }
            }
            return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10421, this.RelativeUri?.Value); ;
        }
    }
}

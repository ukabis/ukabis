using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.OData.Interface.Exceptions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    /// <summary>
    /// ODataDeleteAction
    /// 仕様
    /// ODataクエリ無しのリクエスト→全件削除
    /// 絞り込みは、$filter または$top
    /// -- 以下のクエリは除外（リクエストされたODataクエリから除外する）
    ///  $select：カラムを絞ると、削除に必要なデータが取得できないので、クエリから除外する
    ///  $count=true：Cosmosからの返却データが絞られてしまうため、クエリから除外する
    /// </summary>
    internal class ODataDeleteAction : ODataAction, IEntity
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
        }).CreateMapper();

        /// <summary>
        /// 初期化を行う、初期化はAbstractDynamicApiActionBaseクラスを継承したクラスそれぞれに実装をする。
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// ODataDelete で無視するODataクエリリスト
        /// </summary>
        private readonly List<string> odataDeleteIgnoreList = new List<string>()
        {
            "$select",
            "$count"
        };

        public override HttpResponseMessage ExecuteAction()
        {
            histHeaders = new List<ResponseHeader>();
            Exception PrimaryRepositoryException = null;
            ODataException PrimaryRepositoryExceptionOData = null;
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var lockObject = new object();
            bool delResultPrimaryRep = true;
            bool isDeleted = false;
            Parallel.ForEach(this.DynamicApiDataStoreRepository, repository =>
            {
                var threadPerRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                mapper.Map<IPerRequestDataContainer, IPerRequestDataContainer>(perRequestDataContainer, threadPerRequestDataContainer);

                bool ret = true;
                try
                {
                    Action<JToken, RepositoryType> callbackDelete = null;
                    if ((EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true) || this.IsEnableBlockchain?.Value == true)
                    {
                        PerRequestDataContainer.XgetInternalAllField = true;
                        callbackDelete = (JToken token, RepositoryType repositoryType) => DeleteCallback(token, repositoryType);
                    }
                    //削除対象を取得
                    var repositoryResult = repository.QueryEnumerable(ValueObjectUtil.Create<QueryParam>(new ODataQuery(this.Query?.GetQueryString(odataDeleteIgnoreList, true)), this));
                    if (repositoryResult != null)
                    {
                        var queryResult = repositoryResult.ToList();
                        foreach (var data in queryResult)
                        {
                            var id = data.Value[ID].ToString();
                            repository.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(data.Value, this, callbackDelete));
                            DeleteBase64AttachFiles(id);
                        }
                        if (queryResult.Any())
                            isDeleted = true;
                    }

                }
                catch (NotImplementedException exception)
                {
                    if (this.DynamicApiDataStoreRepository[0] == repository)
                    {
                        PrimaryRepositoryException = exception;
                        delResultPrimaryRep = false;
                    }
                }
                //クエリエラー
                catch (ODataException odataexception)
                {
                    if (this.DynamicApiDataStoreRepository[0] == repository)
                    {
                        PrimaryRepositoryExceptionOData = odataexception;
                        delResultPrimaryRep = false;
                    }
                }
                //クエリエラー
                catch (Exception ex) when (ex is ODataInvalidFilterColumnException or ODataNotConvertibleToQueryException)
                {
                    if (this.DynamicApiDataStoreRepository[0] == repository)
                    {
                        PrimaryRepositoryExceptionOData = new ODataException(ex.Message, ex);
                        delResultPrimaryRep = false;
                    }
                }
                // メールテンプレートまたはWebhookがある場合、イベントハブに通知する
                var enableWebHook = UnityCore.Resolve<bool>("EnableWebHookAndMailTemplate");
                if (delResultPrimaryRep && enableWebHook == true && (HasMailTemplate?.Value == true || HasWebhook?.Value == true))
                {
                    var eventHubRepository = UnityCore.Resolve<IResourceChangeEventHubStoreRepository>();
                    eventHubRepository.Delete(this);
                }
            });

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

            //PrimaryRepository でのエラー有無確認
            if (delResultPrimaryRep && isDeleted)
            {
                if (histHeaders.Count != 0)
                {
                    Dictionary<string, string> histheaders = new Dictionary<string, string>();
                    //自身の履歴ヘッダリストを、自身の空ディクショナリにマージする
                    histheaders = MergeResponseHeader(histheaders, ToDictionary(MergeHistoryRefSourceHeader(this.ControllerRelativeUrl.Value, histHeaders)));
                    return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.NoContent, null, histheaders));
                }
                //エラーなし
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, null));
            }
            //Primary Not Imple
            if (PrimaryRepositoryException != null)
            {
                throw PrimaryRepositoryException;
            }
            //クエリエラー
            if (PrimaryRepositoryExceptionOData != null)
            {
                throw PrimaryRepositoryExceptionOData;
            }

            return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10428, this.RelativeUri?.Value);
        }
    }
}

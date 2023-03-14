using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class ReturnDocumentAction : QueryAction
    {
        /// <summary>
        /// 初期化
        /// 基底クラスの初期化は呼び出すこと
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            //データを復帰させるためIDの上書きは行わない
            this.IsOverrideId = new IsOverrideId(false);
        }

        public override HttpResponseMessage ExecuteAction()
        {
            if (this.MethodType.IsGet != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30410, this.RelativeUri?.Value);
            }
            if (EnableJsonDocumentHistory == false || IsDocumentHistory?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30501, this.RelativeUri?.Value);
            }

            if (Query == null || Query.ContainKey("id") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30402, this.RelativeUri?.Value);
            }

            var id = new Identification(Query.GetValue("id"));
            var result = DynamicApiDataStoreRepository[0].QueryEnumerable(ValueObjectUtil.Create<QueryParam>(this, new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()), id)).ToList();
            // データが存在する場合はBadRequest
            if (result.Count > 0)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30408, this.RelativeUri?.Value);
            }

            // 履歴情報取得
            var documentVersions = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(new DocumentKey(this.RepositoryKey, id.Value));
            if (documentVersions == null || documentVersions.DocumentVersions.Count == 0)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }
            // idチェックを実施し、アクセス可能かどうかチェック
            if (string.IsNullOrEmpty(documentVersions.Id))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }
            else if (!CheckRequestIdIsValid(documentVersions.Id))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }

            var latest = documentVersions.Latest;

            // データ復元
            var returnDocument = GetDocumentHistory(latest);
            if (returnDocument == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30409, this.RelativeUri?.Value);
            }
            DynamicApiDataStoreRepository[0].RegisterOnce(ValueObjectUtil.Create<RegisterParam>(this, returnDocument.Value));

            // 履歴更新
            this.ShallowMapProperty(DynamicApiDataStoreRepository[0].DocumentVersionRepository);
            DynamicApiDataStoreRepository[0].DocumentVersionRepository.UpdateDocumentVersion(
                new DocumentKey(RepositoryKey, id.Value), new DocumentHistory(latest.VersionKey, latest.VersionNo.Value, DateTime.Parse(latest.CreateDate), latest.OpenId, DocumentHistory.StorageLocationType.HighPerformance, DynamicApiDataStoreRepository[0].RepositoryKeyInfo, id.Value));

            // キャッシュ削除
            var cacheDeleteTasks = RefreshApiResourceCache(CreateResourceCacheKey());

            // Blob削除(エラーになってもゴミが残るだけなので処理は続行)
            try
            {
                HistoryEvacuationDataStoreRepository.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(this));
            }
            catch (Exception ex)
            {
                Logger.Error("blob delete error.", ex);
            }
            try
            {
                Task.WaitAll(cacheDeleteTasks);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    if (e is NotImplementedException)
                    {
                        // cache none....
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, ""));
        }
    }
}

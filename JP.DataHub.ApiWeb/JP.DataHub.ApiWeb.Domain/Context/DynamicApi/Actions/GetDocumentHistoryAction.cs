using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class GetDocumentHistoryAction : QueryAction
    {
        public override HttpResponseMessage ExecuteAction()
        {
            if ((this.MethodType.Value != HttpMethodType.MethodTypeEnum.GET)
                && (this.MethodType.Value != HttpMethodType.MethodTypeEnum.POST)
                && (this.MethodType.Value != HttpMethodType.MethodTypeEnum.PUT))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30410, this.RelativeUri?.Value);
            }
            if (EnableJsonDocumentHistory == false || IsDocumentHistory?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30501, this.RelativeUri?.Value);
            }

            if (Query.ContainKey("id") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30402, this.RelativeUri?.Value);
            }
            var ver = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(new DocumentKey(this.RepositoryKey, Query.GetValue("id")));
            if (ver == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }

            //idチェックを実施し、アクセス可能かどうかチェック
            if (string.IsNullOrEmpty(ver.Id))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }
            else if (!CheckRequestIdIsValid(ver.Id))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }


            var search = Query.GetValue("version");
            DocumentHistory target = null;
            int versionNo;
            Guid versionKey;
            if (int.TryParse(search, out versionNo))
            {
                target = ver.DocumentVersions.Where(x => x.VersionNo == versionNo).FirstOrDefault();
            }
            else if (Guid.TryParse(search, out versionKey))
            {
                target = ver.DocumentVersions.Where(x => x.VersionKey == search).FirstOrDefault();
            }
            else if (string.IsNullOrEmpty(search))
            {
                target = ver.DocumentVersions.LastOrDefault();
            }
            else
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30405, this.RelativeUri?.Value);
            }

            if (target == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30406, this.RelativeUri?.Value);
            }

            if (target.LocationType == DocumentHistory.StorageLocationType.Delete)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30407, this.RelativeUri?.Value);
            }
            var isReferenceResolveNeed = false;
            var isUseSnapshot = false;
            //Reference指定があるかどうかチェックする
            if (string.IsNullOrEmpty(PerRequestDataContainer.XReferenceHistory))
            {
                //今までの処理
                isReferenceResolveNeed = false;
            }
            else
            {
                (var token, var iserror) = GetDataFromHeader(PerRequestDataContainer.XReferenceHistory);
                if (iserror)
                {
                    var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10406, this.RelativeUri?.Value);

                }
                //データチェック
                isReferenceResolveNeed = token != null ? token.reference.Value : false;
                //Snapshot解決かどうか
                if (isReferenceResolveNeed == true && (token.refhistinfo == null || token.refhistinfo.Count == 0))
                {
                    isUseSnapshot = true;
                }
            }

            JsonDocument result = null;
            if (target.LocationType == DocumentHistory.StorageLocationType.LowPerformance)
            {
                if (isUseSnapshot)
                {
                    //Snapshotの値設定
                    target = SetSnapshotData(target);
                    //スナップショットのデータ取得
                    result = GetDocumentHistory(target);
                }
                else
                {
                    result = GetDocumentHistory(target);
                    if (isReferenceResolveNeed)
                    {
                        var retJson = RecoveryReferenceAttribute(result.Value.ToString(), GetHistoryReference, isGetOtherResourceNull: true);
                        result = new JsonDocument(retJson);
                    }
                }
            }
            else if (target.LocationType == DocumentHistory.StorageLocationType.HighPerformance)
            {
                if (isReferenceResolveNeed)
                {
                    var jsondata = GetRepositoryData(this.DynamicApiDataStoreRepository[0]);
                    if (jsondata != null && jsondata.Count != 0)
                    {
                        var token = jsondata.JToken;
                        var retJson = RecoveryReferenceAttribute(token.ToString(), GetHistoryReference, isGetOtherResourceNull: true);
                        result = new JsonDocument(retJson);
                    }
                    else
                    {
                        return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30406, this.RelativeUri?.Value);
                    }
                }
                else
                {
                    // QueryStringのversionは不要なため削除
                    Query = new QueryStringVO(Query.Dic.Where(x => x.Key.Value != "version").ToDictionary(x => x.Key, y => y.Value));
                    return base.ExecuteAction();
                }
            }

            var resultToken = result?.RemoveTokenToJson(PerRequestDataContainer.XgetInternalAllField);
            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.OK, resultToken.ToJson().ToString()));
        }

        private DocumentHistory SetSnapshotData(DocumentHistory target)
        {
            if (target.Snapshot == null) return target;
            //スナップショットのデータと入れ替え
            target = new DocumentHistory(target.VersionKey, target.VersionNo.Value, target.Snapshot.CreateDate, target.OpenId, target.Snapshot.LocationType, new RepositoryKeyInfo(target.Snapshot.RepositoryGroupId.Value, target.Snapshot.PhysicalRepositoryId.Value), target.Snapshot.Location);
            return target;
        }

        /// <summary>
        /// リポジトリ（プライマリ）から$Referenceが書かれている生データを取得する
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        private JsonSearchResult GetRepositoryData(INewDynamicApiDataStoreRepository repository)
        {
            //id のみ指定する
            var query = new Dictionary<QueryStringKey, QueryStringValue>();
            query.Add(new QueryStringKey("id"), new QueryStringValue(Query.GetValue("id")));
            this.Query = new QueryStringVO(query);
            var queryParam = DataStoreParamFactory.CreateQueryParam(this, PerRequestDataContainer, new HasSingleData(((PostDataType?.Value ?? "").ToLower() != "array") && string.IsNullOrEmpty(ApiQuery?.Value ?? "")));

            JsonSearchResult result = null;
            if (!string.IsNullOrEmpty(this.RepositoryKey.Value) || !string.IsNullOrEmpty(this.ApiQuery.Value))
            {
                result = new JsonSearchResult(this.ApiQuery, this.PostDataType, this.ActionType);
                var repositoryResult = repository.QueryEnumerable(queryParam);
                if (repositoryResult?.Count() > 0)
                {
                    result.BeginData();
                    result.AddString(repositoryResult.First().Value.ToString());
                    result.EndData();
                }

                if (result.Count != 0)
                {
                    // 暗黙のプロパティはここで削除する
                    var newresult = new JsonSearchResult(result);
                    newresult.BeginData();
                    var jsonstring = result.Value;
                    var json = jsonstring.ToJson();
                    json.RemoveTokenToJson(PerRequestDataContainer.XgetInternalAllField, new string[1] { ETAG });
                    newresult.AddString(jsonstring);
                    newresult.EndData();
                    result = newresult;
                }
            }
            return result;
        }
    }
}
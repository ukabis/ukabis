using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.OData.Interface.Exceptions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class ODataAction : QueryAction
    {
        public override HttpResponseMessage ExecuteAction()
        {
            var exceptionLists = new List<Exception>();
            Dictionary<string, string> header = new Dictionary<string, string>();
            Dictionary<string, string> base64PathList = new Dictionary<string, string>();
            foreach (var repository in this.DynamicApiDataStoreRepository)
            {
                var text = "";
                XResponseContinuation responseContinuation = null;
                try
                {
                    text = GetRepositoryData(repository, out responseContinuation, ref header);
                    if (this.IsEnableAttachFile != null && this.IsEnableAttachFile.Value)
                    {
                        if (text != NULL_KEYWORD)
                        {
                            var jtoken = ReplaceJtoken(JToken.Parse(text), base64PathList, "", ReplaceJtokenPathToBase64);
                            if (base64PathList.Any())
                            {
                                text = jtoken.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex) when (ex is ODataInvalidFilterColumnException or ODataNotConvertibleToQueryException)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10426, this.RelativeUri?.Value, title: null, detail: ex.Message);
                }
                catch (NotImplementedException exception)
                {
                    exceptionLists.Add(exception);
                    text = NULL_KEYWORD;
                }
                if (text != NULL_KEYWORD)
                {
                    var resultMessage = TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.OK, text));
                    header.ToList().ForEach(x => resultMessage.Headers.Add(x.Key, x.Value));
                    if (responseContinuation != null)
                    {
                        resultMessage = AddResponseContinuation(resultMessage, responseContinuation);
                    }
                    AddHeaderBase64BlobPath(resultMessage, base64PathList.Keys.ToList());
                    return resultMessage;
                }
            }

            if ((exceptionLists.Count > 0) && (exceptionLists.Count == this.DynamicApiDataStoreRepository.Count))
            {
                if (exceptionLists.Count == 1)
                {
                    throw exceptionLists[0];
                }
                else
                {
                    throw new AggregateException(exceptionLists);
                }
            }
            return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.I10401, this.RelativeUri?.Value);
        }

        private string GetRepositoryData(INewDynamicApiDataStoreRepository repository, out XResponseContinuation xResponseContinuation, ref Dictionary<string, string> header)
        {
            xResponseContinuation = null;
            var repositoryResult = repository.Query(ValueObjectUtil.Create<QueryParam>(this, new ODataQuery(this.Query?.GetQueryString(true)), new XResourceSharingWith(PerRequestDataContainer.XResourceSharingWith),
                new XResourceSharingPerson(PerRequestDataContainer.XResourceSharingPerson)), out xResponseContinuation);
            if (repositoryResult == null)
            {
                return NULL_KEYWORD;
            }

            if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true && HistoryEvacuationDataStoreRepository != null)
            {
                var headerinfo = new List<DocumentHistoryHeaderDocumentData>();
                var isIdExist = false;
                repositoryResult.ForEach(x =>
                {
                    if (x.Value.GetPropertyValue(ID) != null) isIdExist = true;
                });
                IList<JsonDocument> repResultForHist = repositoryResult;
                if (!isIdExist)
                {
                    //ODataの結果でid が無い場合は、$selectで項目を絞られているからなので、$selectを取り除いて他は同条件で再クエリ
                    var lst = new List<string>();
                    lst.Add("$select");
                    //$select以外をクエリから抽出
                    var query = this.Query?.GetQueryString(lst);
                    repResultForHist = repository.Query(ValueObjectUtil.Create<QueryParam>(this, new ODataQuery(query), new XResourceSharingWith(PerRequestDataContainer.XResourceSharingWith),
                        new XResourceSharingPerson(PerRequestDataContainer.XResourceSharingPerson)), out xResponseContinuation);
                }

                var histList = new List<DocumentHistories>();
                repResultForHist.ForEach(x =>
                {
                    var docKey = x.Value.GetPropertyValue(ID)?.ToString();
                    if (docKey != null)
                    {
                        var repKey = new DocumentKey(RepositoryKey, docKey);
                        var docHist = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(repKey);
                        var headerdata = new DocumentHistoryHeaderDocumentData(docKey, docHist?.DocumentVersions.LastOrDefault()?.VersionKey);
                        headerinfo.Add(headerdata);
                    }
                });
                if (headerinfo.Count != 0)
                {
                    var histHeader = new DocumentHistoryHeader(true, this.ControllerRelativeUrl?.Value, headerinfo);
                    var histHeaderList = new List<DocumentHistoryHeader>();
                    histHeaderList.Add(histHeader);
                    header.Add(DOCUMENTHISTORY_HEADERNAME, histHeaderList.ToJson().ToString().Replace("\r", "").Replace("\n", ""));
                }
            }

            var result = repositoryResult.ToList();
            JsonSearchResult jsonSearchResult = new JsonSearchResult(this.ApiQuery, this.PostDataType, this.ActionType);
            jsonSearchResult.BeginData();
            result.ForEach(x => jsonSearchResult.AddString(x.Value.RemoveTokenToJson(XGetInnerAllField.Value, GetRemoveIgnoreFields()).ToString()));
            jsonSearchResult.EndData();
            if (jsonSearchResult.Count == 0)
            {
                return NULL_KEYWORD;
            }
            // Refrence属性
            (var ret, var lstHeaders) = RecoveryReferenceAttributeWithHeader(jsonSearchResult.Value);
            //自身の履歴OFFでもReference先の履歴がONの場合は、自身に空の履歴ヘッダを作る
            // ただし自身の履歴OFFで、Reference先も履歴OFFの場合は、履歴ヘッダ無くて良いので、refSourceHeadersをクリアする
            (header, lstHeaders) = MakeEmptyHistoryHeaderIfNoHistory(header, lstHeaders);

            //Reference先からのレスポンスヘッダがあればセットする
            header = MergeResponseHeader(header, ToDictionary(MergeHistoryRefSourceHeader(this.ControllerRelativeUrl?.Value, lstHeaders)));

            return ret.ToString();
        }
    }
}

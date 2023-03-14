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
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using System.Web;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class QueryAction : AbstractDynamicApiAction, IEntity
    {
        private string[] etag = null;

        public override HttpResponseMessage ExecuteAction()
        {
            if ((this.MethodType.IsGet != true) && (this.MethodType.IsPost != true) && (this.MethodType.IsPut != true))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10419, this.RelativeUri?.Value);
            }

            // スキーマのチェック
            HttpResponseMessage schemaCheckResponse;
            if (IsValidUrlModelSchema(out schemaCheckResponse) == false)
            {
                return schemaCheckResponse;
            }

            // データが個人領域でかつ、OpenIdが指定されていない場合はエラーとする（Query条件である個人を特定できないため）
            if (IsPerson?.Value == true && string.IsNullOrEmpty(OpenId?.Value) == true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E01401, this.RelativeUri?.Value);
            }

            // etagを消すかどうかをここで持つ（楽観排他のときは消さない）
            if (IsOptimisticConcurrency?.Value == true)
            {
                etag = new string[1] { ETAG };
            }
            Dictionary<string, string> resHeader = new Dictionary<string, string>();

            foreach (var repository in this.DynamicApiDataStoreRepository)
            {
                //クエリのコンパイル(SQLServerのリソース指定の置換などを行う)
                var compiler = UnityCore.Resolve<IApiQueryCompiler>(repository.RepositoryInfo.Type.ToCode());
                var compiledQuery = compiler.Compile(this);
                if (compiledQuery.Item2 != null)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(compiledQuery.Item2, this.RelativeUri?.Value);
                }
                var param = DataStoreParamFactory.CreateQueryParam(this, PerRequestDataContainer, new HasSingleData((PostDataType?.Value ?? "").ToLower() != "array"), compiledQuery.Item1);

                var repData = GetRepositoryData(repository, param, ref resHeader);
                if (repData.jsonData != null && repData.jsonData.Count != 0)
                {
                    //Base64AttachFile
                    Dictionary<string, string> base64PathList = new Dictionary<string, string>();
                    if (this.IsEnableAttachFile != null && this.IsEnableAttachFile.Value)
                    {
                        var jtoken = ReplaceJtoken(repData.jsonData.JToken, base64PathList, "", ReplaceJtokenPathToBase64);
                        if (base64PathList.Any())
                        {
                            repData.jsonData = new JsonSearchResult(jtoken.ToString(), repData.jsonData.Count);
                        }
                    }

                    var responseMessage = TupleToHttpResponseMessage((HttpStatusCode.OK, repData.jsonData));
                    if (!string.IsNullOrEmpty(repData.xcache)) responseMessage.Headers.Add("X-Cache", repData.xcache);
                    AddHeaderBase64BlobPath(responseMessage, base64PathList.Keys.ToList());
                    //履歴ヘッダ追加
                    resHeader.ToList().ForEach(x => responseMessage.Headers.Add(x.Key, x.Value));
                    return responseMessage;
                }
            }

            return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.I10403, this.RelativeUri?.Value);
        }

        private (JsonSearchResult jsonData, string xcache) GetRepositoryData(INewDynamicApiDataStoreRepository repository, QueryParam queryParam, ref Dictionary<string, string> resHeader)
        {
            string keyCache = CreateCacheKey(repository, queryParam);
            var keyBlobCache = CreateBlobCacheKey(repository, queryParam);
            if (IsUseBlobCache.Value && !string.IsNullOrEmpty(keyBlobCache) && BlobCache.Contains(keyBlobCache) && !ResourceSharingPersonRules.Any())
            {
                var result = BlobCache.Get<JsonSearchResult>(keyBlobCache, out var isNullValue);
                return (result, $"HIT key:{keyBlobCache}");
            }

            if (keyCache != null && Cache.Contains(keyCache) == true && !this.ResourceSharingPersonRules.Any())
            {
                var result = Cache.Get<JsonSearchResult>(keyCache, out var isNullValue);
                return (result, $"HIT key:{keyCache}");
            }
            else
            {
                JsonSearchResult result = null;
                if (!string.IsNullOrEmpty(this.RepositoryKey.Value) || !string.IsNullOrEmpty(this.ApiQuery.Value))
                {
                    result = new JsonSearchResult(this.ApiQuery, this.PostDataType, this.ActionType);
                    var repositoryResult = repository.Query(queryParam, out XResponseContinuation responseContinuation);

                    if (repositoryResult?.Any() == false && queryParam.QueryString != null)
                    {
                        //0件のときに、パラメータのスペースの16進数変換を行い再取得する
                        if (ConvertHexToSpace(queryParam, out var queryParam2))
                        {
                            repositoryResult = repository.Query(queryParam2, out responseContinuation);
                        }
                    }

                    var hasSingle = new HasSingleData(((PostDataType?.Value ?? "").ToLower() != "array") && string.IsNullOrEmpty(ApiQuery?.Value));
                    if (repositoryResult?.Count() > 0)
                    {
                        result.BeginData();
                        if (hasSingle.Value)
                        {
                            result.AddString(repositoryResult.First().Value.ToString());
                        }
                        else
                        {
                            repositoryResult.ForEach(x => result.AddString(x.Value.ToString()));
                        }
                        result.EndData();
                    }
                    if (result.Count == 0)
                    {
                        if (IsUseBlobCache.Value && !string.IsNullOrEmpty(keyBlobCache) && !ResourceSharingPersonRules.Any())
                        {
                            result = null;
                            BlobCache.Add(keyBlobCache, result, CacheInfo.CacheSecond);
                            return (null, null);
                        }
                        if (keyCache != null && !this.ResourceSharingPersonRules.Any())
                        {
                            result = null;
                            Cache.Add(keyCache, result, CacheInfo.CacheSecond);
                            return (null, null);
                        }
                    }
                    else
                    {
                        // 暗黙のプロパティはここで削除する
                        var newresult = new JsonSearchResult(result);
                        newresult.BeginData();
                        var jsonstring = result.Value;
                        var json = jsonstring.ToJson();
                        JToken ret = null;
                        List<ResponseHeader> lstHeaders = new List<ResponseHeader>();
                        if (newresult.IsArray == true)
                        {
                            foreach (var item in json as JArray)
                            {
                                var tmplst = new List<ResponseHeader>();
                                item.RemoveTokenToJson(PerRequestDataContainer.XgetInternalAllField, etag);
                                // Reference属性の解決
                                (ret, tmplst) = RecoveryReferenceAttributeWithHeader(item.ToString());
                                lstHeaders.AddRange(tmplst);
                                jsonstring = ret?.ToString();
                                newresult.AddString(jsonstring);
                            }
                            if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true && HistoryEvacuationDataStoreRepository != null)
                            {
                                //履歴ヘッダの処理
                                var headerinfo = new List<DocumentHistoryHeaderDocumentData>();
                                foreach (var item in json as JArray)
                                {
                                    headerinfo = MakeHeaderInfo(item, headerinfo);
                                }
                                resHeader = MakeResHistHeader(headerinfo, resHeader);
                            }
                        }
                        else if (json is JValue)
                        {
                            newresult.AddString(jsonstring);
                        }
                        else
                        {
                            if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true && this.HistoryEvacuationDataStoreRepository != null)
                            {
                                //履歴ヘッダの処理
                                var headerinfo = new List<DocumentHistoryHeaderDocumentData>();
                                headerinfo = MakeHeaderInfo(json, headerinfo);
                                resHeader = MakeResHistHeader(headerinfo, resHeader);
                            }
                            json.RemoveTokenToJson(PerRequestDataContainer.XgetInternalAllField, etag);
                            // Reference属性の解決
                            (ret, lstHeaders) = RecoveryReferenceAttributeWithHeader(json.ToString());

                            jsonstring = ret?.ToString();
                            newresult.AddString(jsonstring);
                        }
                        newresult.EndData();
                        result = newresult;
                        //自身の履歴OFFでもReference先の履歴がONの場合は、自身に空の履歴ヘッダを作る
                        // ただし自身の履歴OFFで、Reference先も履歴OFFの場合は、履歴ヘッダ無くて良いので、refSourceHeadersをクリアする
                        (resHeader, lstHeaders) = MakeEmptyHistoryHeaderIfNoHistory(resHeader, lstHeaders);

                        //Reference先からのレスポンスヘッダがあればセットする
                        resHeader = MergeResponseHeader(resHeader, ToDictionary(MergeHistoryRefSourceHeader(this.ControllerRelativeUrl?.Value, lstHeaders)));
                        if (responseContinuation != null && !hasSingle.Value)
                        {
                            resHeader.Add("X-ResponseContinuation", responseContinuation.ContinuationString);
                        }
                    }
                }
                if (IsUseBlobCache.Value && !string.IsNullOrEmpty(keyBlobCache) && !ResourceSharingPersonRules.Any())
                {
                    result.InUse(true); //非同期処理に入れるとDisposeされてしまう場合があるため先にフラグを立てる
                    Task.Run(() =>
                    {
                        try
                        {
                            BlobCache.Add(keyBlobCache, result, CacheInfo.CacheSecond);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            result.InUse(false);
                        }
                    });
                }
                else if (keyCache != null && !this.ResourceSharingPersonRules.Any())
                {
                    Cache.Add(keyCache, result, CacheInfo.CacheSecond, MaxSaveApiResponseCacheSize);
                }
                return (result, null);
            }
        }

        /// <summary>
        /// 履歴データを取得ししてリストを作る
        /// </summary>
        /// <param name="json"></param>
        /// <param name="headerinfo"></param>
        /// <returns></returns>
        private List<DocumentHistoryHeaderDocumentData> MakeHeaderInfo(JToken json, List<DocumentHistoryHeaderDocumentData> headerinfo)
        {
            var id = json.GetPropertyValue(ID)?.ToString();
            if (id != null)
            {
                var repKey = new DocumentKey(RepositoryKey, id);
                var docHist = DynamicApiDataStoreRepository[0].DocumentVersionRepository?.GetDocumentVersion(repKey);
                var headerdata = new DocumentHistoryHeaderDocumentData(id, docHist?.DocumentVersions.LastOrDefault()?.VersionKey);
                headerinfo.Add(headerdata);
            }
            return headerinfo;
        }

        /// <summary>
        /// 履歴データを、レスポンスヘッダ用に整形する
        /// </summary>
        /// <param name="headerinfo"></param>
        /// <param name="resHeader"></param>
        /// <returns></returns>
        private Dictionary<string, string> MakeResHistHeader(List<DocumentHistoryHeaderDocumentData> headerinfo, Dictionary<string, string> resHeader)
        {
            if (headerinfo.Count != 0)
            {
                var histHeader = new DocumentHistoryHeader(true, this.ControllerRelativeUrl?.Value, headerinfo);
                var histHeaderList = new List<DocumentHistoryHeader>();
                histHeaderList.Add(histHeader);
                resHeader.Add(DOCUMENTHISTORY_HEADERNAME, histHeaderList.ToJson().ToString().Replace("\r", "").Replace("\n", ""));
            }

            return resHeader;
        }
    }
}

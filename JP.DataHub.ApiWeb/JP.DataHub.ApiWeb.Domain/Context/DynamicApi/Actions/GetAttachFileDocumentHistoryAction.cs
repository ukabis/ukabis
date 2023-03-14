using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class GetAttachFileDocumentHistoryAction : AttachFileDownloadAction
    {
        public override HttpResponseMessage ExecuteAction()
        {
            if ((this.MethodType.IsGet != true) && (this.MethodType.IsPost != true) && (this.MethodType.IsPut != true))
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }
            if (EnableJsonDocumentHistory == false || IsDocumentHistory?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30501, this.RelativeUri?.Value);
            }

            if (Query.ContainKey("FileId") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30402, this.RelativeUri?.Value);
            }
            if (Query.ContainKey("version") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30403, this.RelativeUri?.Value);
            }

            string id = Query.GetValue("FileId");
            var ver = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(new DocumentKey(this.RepositoryKey, id));
            if (ver == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }

            // idチェックを実施し、アクセス可能かどうかチェック
            if (string.IsNullOrEmpty(ver.DocumentIdForAttachFile))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }
            else if (!CheckRequestIdIsValid(ver.DocumentIdForAttachFile))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }

            var search = Query.GetValue("version");
            DocumentHistory target = null;
            if (int.TryParse(search, out int versionNo))
            {
                target = ver.DocumentVersions.Where(x => x.VersionNo == versionNo).FirstOrDefault();
            }
            else if (Guid.TryParse(search, out Guid versionKey))
            {
                target = ver.DocumentVersions.Where(x => x.VersionKey == search).FirstOrDefault();
            }
            else
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30405, this.RelativeUri?.Value);
            }
            if (target == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30406, this.RelativeUri?.Value);
            }

            switch (target.LocationType)
            {
                case DocumentHistory.StorageLocationType.Delete:
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30407, this.RelativeUri?.Value);
                case DocumentHistory.StorageLocationType.LowPerformance:
                    // keyの指定があるファイルなら指定する
                    if (!string.IsNullOrEmpty(target.AttachFileMetaInfo?.Key))
                    {
                        var queryKey = new QueryStringKey("Key");
                        string keyString = "";
                        if (this.Query.Dic.ContainsKey(queryKey))
                        {
                            keyString = this.Query.Dic[queryKey].Value;
                        }
                        else
                        {
                            return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20402, this.RelativeUri?.Value);
                        }
                        if (keyString != target.AttachFileMetaInfo.Key)
                        {
                            return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20402, this.RelativeUri?.Value);
                        }
                    }

                    var result = GetDocumentHistoryStream(target);
                    var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(result) };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(target.AttachFileMetaInfo.ContentType);
                    return response;
                case DocumentHistory.StorageLocationType.HighPerformance:
                    // FileDownload
                    // QueryStringのversionは不要なため削除
                    Query = new QueryStringVO(Query.Dic.Where(x => x.Key.Value != "version").ToDictionary(x => x.Key, y => y.Value));
                    return base.ExecuteAction();
                default:
                    throw new NotImplementedException("unrecognized storagelocation type");
            }
        }
    }
}
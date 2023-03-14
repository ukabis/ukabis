using System.Net;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class DriveOutAttachFileDocumentAction : AttachFileUploadAction
    {
        public override HttpResponseMessage ExecuteAction()
        {
            if (this.MethodType.IsGet != true)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }
            if (EnableJsonDocumentHistory == false || IsDocumentHistory?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30501, this.RelativeUri?.Value);
            }
            if (Query == null || Query.ContainKey("FileId") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30402, this.RelativeUri?.Value);
            }

            var id = new Identification(Query.GetValue("FileId"));

            // RDBMSリポジトリの場合はメタ情報用のテーブルへのアクセスとなるためリソースモデルをメタ情報のモデルに差し替える
            ControllerSchema = UnityCore.Resolve<IDynamicApiRepository>().GetSchemaModelById(AttachFileMetaDataSchemaId);
            var result = DynamicApiDataStoreRepository[0].QueryOnce(ValueObjectUtil.Create<QueryParam>(this));
            if (result == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30412, this.RelativeUri?.Value);
            }

            // データ退避
            var info = DynamicApiAttachFileInformation.PerseFromJToken(result.Value);
            var srcFileUriWithSas = FileDataStoreRepository.GetUriWithSharedAccessSignature(info);
            var regParam = new RegisterParam(srcFileUriWithSas, result.Value, VendorId, SystemId, OpenId, RepositoryKey, PartitionKey, Xversion, IsVendor, IsPerson, ApiId, ControllerId, ControllerRepositoryKey, ApiUri);
            var regResult = HistoryEvacuationDataStoreRepository.RegisterOnce(regParam);
            var historyBlobPath = (regResult.Additional["Container"] as string).UrlCombine(regResult.Value);


            MakeHistory(id.Value, historyBlobPath, result.Value, isDelete: false, isDriveout: true);
            // データ削除
            AttachFileDynamicApiDataStoreRepository.DeleteAttachFile(info);

            // メタデータの更新 isuploadedをfalseに
            var etag = new JsonDocument(result.Value).GetEtag();
            PerRequestDataContainer.XgetInternalAllField = true;
            UpdateMeta(result.Value.RemoveTokenToJson(PerRequestDataContainer.XgetInternalAllField).ToString().ToJson(), etag, isUploaded: false);

            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, ""));
        }
    }
}

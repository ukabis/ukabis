using System.Net;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Log;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class ReturnAttachFileDocumentAction : AttachFileUploadAction
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

            // Meta情報などがあるデータを取得と同時にベンダー領域超えをしていないかのチェック
            // RDBMSリポジトリの場合はメタ情報用のテーブルへのアクセスとなるためリソースモデルをメタ情報のモデルに差し替える
            ControllerSchema = UnityCore.Resolve<IDynamicApiRepository>().GetSchemaModelById(AttachFileMetaDataSchemaId);
            var result = DynamicApiDataStoreRepository[0].QueryOnce(ValueObjectUtil.Create<QueryParam>(this))?.Value;
            if (result == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }

            var info = DynamicApiAttachFileInformation.PerseFromJToken(result);

            //// IfExistsとしてGetAttachFileFileStreamを使用
            //try
            //{
            //    var blobFile = FileDataStoreRepository.GetAttachFileFileStream(info);
            //    // データが存在する場合はBadRequest
            //    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30408, this.RelativeUri?.Value);
            //}
            //catch (NotFoundException)
            //{
            //    /* do nothing */
            //}


            // 履歴情報取得
            var documentVersions = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(new DocumentKey(this.RepositoryKey, id.Value));
            if (documentVersions == null || documentVersions.DocumentVersions.Count == 0)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }
            var latest = documentVersions.Latest;

            // データ復元
            var returnDocumentUri = GetDocumentHistoryUri(latest);
            if (returnDocumentUri == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30409, this.RelativeUri?.Value);
            }
            AttachFileDynamicApiDataStoreRepository.CopyFile(info.BlobName, info.BlobContainerName, returnDocumentUri);

            // メタデータの更新
            var etag = new JsonDocument(result).GetEtag();
            PerRequestDataContainer.XgetInternalAllField = true;
            UpdateMeta(result.RemoveTokenToJson(PerRequestDataContainer.XgetInternalAllField).ToString().ToJson(), etag, isUploaded: true);

            // 履歴更新
            this.ShallowMapProperty(DynamicApiDataStoreRepository[0].DocumentVersionRepository);
            DynamicApiDataStoreRepository[0].DocumentVersionRepository.UpdateDocumentVersion(
                new DocumentKey(RepositoryKey, id.Value), new DocumentHistory(latest.VersionKey, latest.VersionNo.Value, DateTime.Parse(latest.CreateDate), latest.OpenId, DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(FileDataStoreRepositoryInfo), id.Value));

            // Blob削除(エラーになってもゴミが残るだけなので処理は続行)
            try
            {
                HistoryEvacuationDataStoreRepository.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(this));
            }
            catch (Exception ex)
            {
                Logger.Fatal("blob delete error.", ex);
            }

            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, ""));
        }
    }
}

using System.Net;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class AttachFileDeleteAction : AbstractAttachFileAction, IEntity
    {
        public IAttachFileRepository AttachFileRepository => UnityCore.Resolve<IAttachFileRepository>();

        public override HttpResponseMessage ExecuteAction()
        {
            if (!this.IsEnableAttachFile.Value)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotImplemented, null));
            }

            if (this.MethodType.IsDelete != true)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }
            FileId fileid = GetQueryStringToFileId();
            if (fileid == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20401, this.RelativeUri?.Value);
            }

            // ファイル情報を取得
            // RDBMSリポジトリの場合はメタ情報用のテーブルへのアクセスとなるためリソースモデルをメタ情報のモデルに差し替える
            ControllerSchema = UnityCore.Resolve<IDynamicApiRepository>().GetSchemaModelById(AttachFileMetaDataSchemaId);
            var infoJToken = MetaDataStoreRepository.QueryOnce(ValueObjectUtil.Create<QueryParam>(this))?.Value;
            if (infoJToken == null)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotFound, null));
            }

            var info = DynamicApiAttachFileInformation.PerseFromJToken(infoJToken);
            if (info == null)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotFound, null));
            }
            // Keyチェック
            if (IsKeyUnMatch(info))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20402, this.RelativeUri?.Value);
            }

            // 履歴有効な場合は先に退避処理
            string historyBlobPath = null;
            if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true)
            {
                historyBlobPath = ReplicateFile(infoJToken, info);
            }
            MetaDataStoreRepository.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(infoJToken, this));

            // FileDelete
            if (!info.IsExternalAttachFile)
            {
                try
                {
                    FileDataStoreRepository.DeleteAttachFile(info);
                }
                catch (Exception e)
                {// メタデータの削除に成功していればユーザーからはアクセスできなくなるので エラーを返さないようにする
                 // ただしアラートは出す
                    Logger.Fatal("AttachFile Blob 削除エラー", e);
                }
            }

            IEnumerable<DocumentHistoryHeader> historyHeaders = null;
            if (EnableJsonDocumentHistory == true && this.IsDocumentHistory?.Value == true)
            {
                historyHeaders = MakeHistory(fileid.Value.ToString(), historyBlobPath, infoJToken, isDelete: true);
            }

            // ブロックチェーン機能有効な場合はイベントハブへ通知
            if (BlockchainEventHubStoreRepository != null)
            {
                BlockchainEventHubStoreRepository.Delete(info.FileId, MetaDataStoreRepository.RepositoryInfo.Type);
                BlockchainEventHubStoreRepository.Delete(info.FileId, RepositoryType.AttachFileBlob);//ファイルを読む必要がないのでRepositoryGroupIdはつけない
            }

            return historyHeaders == null ? TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, null))
                : TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(HttpStatusCode.NoContent, null, new Dictionary<string, string>() { { DOCUMENTHISTORY_HEADERNAME, historyHeaders.ToJson().ToString().Replace("\r", "").Replace("\n", "") } }));
        }
    }
}

using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal class CreateAttachFileActionInjector : AbstractAttachFileActionInjector
    {
        public override void Execute(Action action)
        {
            var target = Target as AbstractDynamicApiAction;
            if (!target.IsEnableAttachFile.Value)
            {
                ReturnValue = target.TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotImplemented, null));
                return;
            }

            // リクエストのバリデーション
            DynamicApiAttachFileInformation info;
            try
            {
                var json = JToken.Parse(target.Contents.ReadToString());
                if (json[nameof(DynamicApiAttachFileInformation.FileId)] != null)
                {
                    ReturnValue = ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20410);
                    return;
                }

                info = DynamicApiAttachFileInformation.PerseFromJToken(json);
            }
            catch (Exception ex)
            {
                var log = new JPDataHubLogger(this.GetType());
                log.Error(ex);
                ReturnValue = target.TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, ex.Message));
                return;
            }

            // ファイル名に利用できない文字は許可しない
            var invalidChars = Path.GetInvalidFileNameChars();
            if (info.FileName?.IndexOfAny(invalidChars) >= 0)
            {
                ReturnValue = ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20417);
                return;
            }

            if (info.IsExternalAttachFile)
            {
                // 履歴ONの場合は外部添付ファイル使用不可
                if (target.IsDocumentHistory?.Value == true)
                {
                    ReturnValue = ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20418);
                    return;
                }

                if (info.ExternalAttachFile == null)
                {
                    ReturnValue = ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20413);
                    return;
                }
                else if (!ExternalAttachFileInfomation.SupportedDataSourceTypes.Contains(info.ExternalAttachFile.DataSourceType))
                {
                    ReturnValue = ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20414);
                    return;
                }

                var repository = UnityCore.Resolve<IExternalAttachFileRepository>(info.ExternalAttachFile.DataSourceType);
                if (!repository.Validate(info.ExternalAttachFile, out var errorCode))
                {
                    ReturnValue = ErrorCodeMessage.GetRFC7807HttpResponseMessage(errorCode.Value);
                    return;
                }
            }

            if (target.IsEnableUploadContentCheck)
            {
                var contentType = info.ContentType?.ToLower();
                var fileName = info.FileName?.ToLower();

                if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(fileName))
                {
                    // コンテンツチェック
                    if (!target.IsContentOkByContentTypeAndFileName(contentType, fileName))
                    {
                        // ブロック
                        ReturnValue = ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20411);
                        return;
                    }
                }
            }

            // 履歴への書き出しを防ぐために履歴を明示的に無効にしてaction実行
            // メタデータの更新操作が実装され も履歴管理するということであれば以下を除外すること
            target.IsDocumentHistory = new IsDocumentHistory(false);

            // RDBMSリポジトリの場合はメタ情報用のテーブルへのアクセスとなるため、
            // 添付ファイルの処理であることを通知するフラグをONにする
            // また、リソースモデルをメタ情報のモデルに差し替える
            target.OperationInfo = new OperationInfo(OperationInfo.OperationType.AttachFileMeta);
            target.ControllerSchema = UnityCore.Resolve<IDynamicApiRepository>().GetSchemaModelById(AttachFileMetaDataSchemaId);

            // 添付ファイルのデータを登録
            var content = info.Serialize();
            target.Contents = new Contents(content);
            action();

            // 成功時はFileIdを返却する
            if (((HttpResponseMessage)ReturnValue).IsSuccessStatusCode)
            {
                ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(JsonConvert.SerializeObject((new { FileId = info.FileId.ToString() })), Encoding.UTF8, MEDIATYPE_JSON) };
            }
        }
    }
}

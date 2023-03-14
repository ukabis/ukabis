using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class AttachFileUploadAction : AbstractAttachFileAction, IEntity
    {
        [DataContract]
        private class RegistReturnId
        {
            [DataMember]
            public string id { get; set; }
        }

        private Lazy<JSchema> requestSchema = null;
        /// <summary>
        /// blockchainへのイベントへの発行タスク 
        /// 単体テストでブロックチェーン関連処理が実行されたかチェックする目的
        /// </summary>
        private Task TaskPublishEventBlockchain = null;

        /// <summary>
        /// 初期化のため
        /// 必ず規定クラスの初期化は呼び出すこと
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            requestSchema = new Lazy<JSchema>(() => (JSchema)RequestSchema.ToJSchema());
        }

        private object lockObj = new object();
        public override HttpResponseMessage ExecuteAction()
        {
            if (!this.IsEnableAttachFile.Value)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotImplemented, null));
            }
            if (this.MethodType.IsPost != true)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }

            if (string.IsNullOrEmpty(this.RepositoryKey.Value))
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }
            FileId fileid = GetQueryStringToFileId();
            if (fileid == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20401, this.RelativeUri?.Value);
            }
            // コンテンツチェックを行うか
            if (IsEnableUploadContentCheck)
            {
                string contenttype = string.Empty;
                var contenttypeKeyName = HttpContextAccessor.Current.Request.Headers.Keys.Where(x => x.ToLower() == "content-type").FirstOrDefault();
                // HttpContextに無い場合は、内部呼び出しの可能性があるので、PerRequestから取ってみる
                if (contenttypeKeyName == null)
                {
                    lock (lockObj)
                    {
                        contenttypeKeyName = PerRequestDataContainer.RequestHeaders.Keys.Where(x => x.ToLower() == "content-type").FirstOrDefault();
                        if (!string.IsNullOrEmpty(contenttypeKeyName))
                        {
                            contenttype = PerRequestDataContainer.RequestHeaders[contenttypeKeyName][0];
                        }
                    }
                }
                else
                {
                    contenttype = HttpContextAccessor.Current.Request.Headers[contenttypeKeyName];
                }
                if (!IsContentOkByContentType(contenttype))
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20411);
                }
            }

            var input = new DynamicApiAttachFileInputStream(this.Contents.Value, ContentRange?.Value);
            PerRequestDataContainer.XgetInternalAllField = true;

            // ファイル情報を取得
            // RDBMSリポジトリの場合はメタ情報用のテーブルへのアクセスとなるためリソースモデルをメタ情報のモデルに差し替える
            ControllerSchema = UnityCore.Resolve<IDynamicApiRepository>().GetSchemaModelById(AttachFileMetaDataSchemaId);
            var infoJson = MetaDataStoreRepository.QueryOnce(ValueObjectUtil.Create<QueryParam>(this))?.Value;
            if (infoJson == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            if (infoJson.IsExistProperty(nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)) &&
                infoJson[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)].Value<bool>())
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20412, this.RelativeUri?.Value);
            }

            JsonDocument jsonDocument = new JsonDocument(infoJson);
            var etag = new JsonDocument(infoJson).GetEtag();
            infoJson.RemoveTokenToJson(PerRequestDataContainer.XgetInternalAllField);
            DynamicApiAttachFileInformation info = DynamicApiAttachFileInformation.PerseFromJToken(infoJson);

            string historyBlobPath = null;
            if (EnableJsonDocumentHistory == true && IsDocumentHistory?.Value == true && info.IsUploaded && input.IsStartStream)
            {// 履歴有効ですでにUL済ならblobをあらかじめコピーしておく
             // chunk upload時は最初の一回だけ実行されるように
                historyBlobPath = ReplicateFile(infoJson, info);
            }

            // ファイルアップロード
            FileDataStoreRepository.UploadAttachFile(info, input);

            // ファイルアップロード完了時の処理
            var returnMessage = new HttpResponseMessage(HttpStatusCode.OK);
            if (input.IsEndStream)
            {
                UpdateMeta(infoJson, etag, isUploaded: true);
                // 履歴の書き込み
                if (EnableJsonDocumentHistory == true && IsDocumentHistory?.Value == true)
                {
                    var historyHeader = MakeHistory(fileid.Value.ToString(), historyBlobPath, infoJson, isDelete: false);
                    returnMessage.Headers.Add(DOCUMENTHISTORY_HEADERNAME, historyHeader.ToJson().ToString().Replace("\r", "").Replace("\n", ""));
                }
            }


            // ブロックチェーン機能有効な場合はファイルをコピーする
            // ファイルをアップロードされたイベントを発行して blockchainへの通知はSubscriber側
            if (IsEnableBlockchain?.Value == true)
            {
                TaskPublishEventBlockchain = Task.Run(() =>
                {
                    PublishEventBlockchain(infoJson, info);
                }).ContinueWith((task) =>
                {
                    // アラート発報のためにfatal
                    Logger.Fatal(task.Exception);
                    task.Exception.InnerExceptions.ToList().ForEach(x => Logger.Error(x));
                }, TaskContinuationOptions.OnlyOnFaulted);
            }

            return returnMessage;
        }

        private void PublishEventBlockchain(JToken infoJson, DynamicApiAttachFileInformation info)
        {
            var newFileExt = $".{Guid.NewGuid()}.bak";
            FileDataStoreRepository.CopyFile(Path.ChangeExtension(info.BlobName, newFileExt), info.BlobContainerName, info.BlobName, info.BlobContainerName);
            BlockchainEventHubStoreRepository.Register(infoJson["FileId"].ToString(), infoJson.ReplaceField("FilePath", Path.ChangeExtension(info.FilePath, newFileExt)).AddField("RepositoryGroupId",
                AttachFileBlobRepositoryGroupId?.Value), RepositoryType.AttachFileBlob);
        }
    }
}

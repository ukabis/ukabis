using System.Net;
using System.Net.Http.Headers;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class AttachFileDownloadAction : AbstractAttachFileAction, IEntity
    {
        protected IAttachFileRepository AttachFileRepository => UnityCore.Resolve<IAttachFileRepository>();


        public override HttpResponseMessage ExecuteAction()
        {
            if (!this.IsEnableAttachFile.Value)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotImplemented, null));
            }
            if (this.MethodType.IsGet != true)
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

            // ファイル情報を取得
            // RDBMSリポジトリの場合はメタ情報用のテーブルへのアクセスとなるためリソースモデルをメタ情報のモデルに差し替える
            ControllerSchema = UnityCore.Resolve<IDynamicApiRepository>().GetSchemaModelById(AttachFileMetaDataSchemaId);
            var infoJToken = MetaDataStoreRepository.QueryOnce(ValueObjectUtil.Create<QueryParam>(this))?.Value;
            if (infoJToken == null)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NotFound, null));
            }
            var info = DynamicApiAttachFileInformation.PerseFromJToken(infoJToken);

            // Keyチェック
            if (IsKeyUnMatch(info))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E20402, this.RelativeUri?.Value);
            }

            // ファイルストリーム取得
            Stream stream;
            if (info.IsExternalAttachFile)
            {
                // 外部添付ファイル
                try
                {
                    var repository = UnityCore.Resolve<IExternalAttachFileRepository>(info.ExternalAttachFile.DataSourceType);
                    stream = repository.GetAttachFileFileStream(info.ExternalAttachFile);
                }
                catch (ExternalAttachFileException ex)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ex.ErrorCode.Value, this.RelativeUri?.Value);
                }
            }
            else
            {
                try
                {
                    stream = FileDataStoreRepository.GetAttachFileFileStream(info);
                }
                catch (NotFoundException nfe)
                {
                    //アップロード済みの状態でファイルが存在しない場合はエラーとする
                    if (info.IsUploaded)
                    {
                        throw new Exception($"storage file not found : {nfe.Message}");
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }
                }

                if (stream == null || stream.Length == 0)
                {
                    if (info.IsUploaded)
                    {
                        throw new Exception($"storage file not found");
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }
                }
            }

            // ストリーム返却
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(info.ContentType);

            return result;
        }

    }
}

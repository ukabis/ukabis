using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IExternalAttachFileRepository
    {
        /// <summary>
        /// 外部添付ファイル設定を検証する
        /// </summary>
        bool Validate(ExternalAttachFileInfomation info, out ErrorCodeMessage.Code? errorCode);

        /// <summary>
        /// 外部データソースからファイルストリームを取得する
        /// </summary>
        Stream GetAttachFileFileStream(ExternalAttachFileInfomation info);
    }
}

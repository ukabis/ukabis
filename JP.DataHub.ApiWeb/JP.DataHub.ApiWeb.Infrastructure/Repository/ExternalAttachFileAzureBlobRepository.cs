using JP.DataHub.Com.Log;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Configuration;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class ExternalAttachFileAzureBlobRepository : IExternalAttachFileRepository
    {
        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(ExternalAttachFileAzureBlobRepository));


        /// <summary>
        /// 外部添付ファイル設定を検証する
        /// </summary>
        public bool Validate(ExternalAttachFileInfomation info, out ErrorCodeMessage.Code? errorCode)
        {
            errorCode = null;

            if (!TryParseSettings(info, out var connectionString, out var containerName, out var fileName))
            {
                errorCode = ErrorCodeMessage.Code.E20414;
                return false;
            }

            try
            {
                BlobStorage blobStorage = new BlobStorage(new AzureStorageSetting(connectionString));
                var blob = blobStorage.GetBlockBlobAsync(containerName, fileName).Result;
                if (!blob.ExistsAsync().Result)
                {
                    logger.Warn($"Specified file not found. (ContainerName={containerName}, FileName={fileName})");
                    errorCode = ErrorCodeMessage.Code.E20416;
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex, $"Failed to get external azure blob file stream. (Type:{ex.GetType()}, Message={ex.Message})");
                errorCode = ErrorCodeMessage.Code.E20415;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Blobからファイルストリームを取得する
        /// </summary>
        public Stream GetAttachFileFileStream(ExternalAttachFileInfomation info)
        {
            if (!TryParseSettings(info, out var connectionString, out var containerName, out var fileName))
            {
                throw new ExternalAttachFileException(ErrorCodeMessage.Code.E20414);
            }

            try
            {
                BlobStorage blobStorage = new BlobStorage(new AzureStorageSetting(connectionString));
                var blob = blobStorage.GetBlockBlobAsync(containerName, fileName).Result;
                if (!blob.ExistsAsync().Result)
                {
                    logger.Warn($"Specified file not found. (ContainerName={containerName}, FileName={fileName})");
                    throw new ExternalAttachFileException(ErrorCodeMessage.Code.E20416);
                }
                return blob.OpenReadAsync().Result;
            }
            catch (Exception ex)
            {
                logger.Warn(ex, $"Failed to get external azure blob file stream. (Type:{ex.GetType()}, Message={ex.Message})");
                throw new ExternalAttachFileException(ErrorCodeMessage.Code.E20415);
            }
        }


        private bool TryParseSettings(ExternalAttachFileInfomation info, out string connectionString, out string containerName, out string fileName)
        {
            connectionString = null;
            containerName = null;
            fileName = null;

            try
            {
                connectionString = info.Credentials[0];

                var index = info.FilePath.IndexOf("/");
                containerName = info.FilePath.Substring(0, index);
                fileName = info.FilePath.Substring(index + 1, info.FilePath.Length - (index + 1));

                return true;
            }
            catch (Exception ex)
            {
                logger.Warn(ex, $"Invalid ExternalAttachFileInformation. (Type:{ex.GetType()}, Message={ex.Message})");
                return false;
            }
        }
    }
}

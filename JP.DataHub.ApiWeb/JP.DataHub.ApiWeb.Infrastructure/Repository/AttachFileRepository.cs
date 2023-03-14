using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Unity.Injection;
using Unity.Resolution;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Infrastructure.Database.AttachFile;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [CacheKey]
    internal class AttachFileRepository : IAttachFileRepository
    {
        [CacheKey(CacheKeyType.Id, "attachfile_storage_id")]
        public static string CACHE_KEY_ATTACH_FILE_STORAGE_CONNECTIONSTRING = "AttachFileStorageConnectionString";

        private static readonly TimeSpan s_cacheExpireTime = TimeSpan.Parse("0:30:00");

        private ICache Cache => _lazyCache.Value;
        private Lazy<ICache> _lazyCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>());

        private IJPDataHubDbConnection SqlConnection => _lazySqlConnection.Value;
        private Lazy<IJPDataHubDbConnection> _lazySqlConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("AttachFile"));


        /// <summary>
        /// ファイルを一時アップロードする。
        /// </summary>
        public FilePath TempFileUpload(FileId fileId, FileName fileName, InputStream inputStream, IsAppendStream isAppendStream, AppendPosition appendPosition)
        {
            var filePath = Path.Combine(UnityCore.Resolve<string>("AttachFileTmpPath"), fileId.Value.ToString(), fileName.Value);
            if (!isAppendStream.Value)
            {
                // ファイルが既に存在する場合は削除する。
                DeleteLocalFile(filePath);
            }
            if (!StreamSave(filePath, inputStream.Value, appendPosition.Value))
            {
                throw new System.Exception("not match uploaded file position");
            }
            return new FilePath(filePath);
        }

        /// <summary>
        /// ファイルをアップロードする。
        /// </summary>
        public void Upload(FilePath filePath, FileId fileId, FileName fileName, VendorId vendorId, ContentType contentType, AttachFileStorageId attachFilestorageId)
        {
            var connectionString = GetConnectionString(attachFilestorageId.Value);
            var blobStorage = GetBlobStorage(connectionString);

            _ = blobStorage.GetContainerAsync(vendorId.Value.ToString(), BlobContainerPublicAccessType.Off).Result;
            string blobName = $"{fileId.Value}/{fileName.Value}";

            var fileInfo = new FileInfo(filePath.Value);
            using (FileStream stream = fileInfo.OpenRead())
            {
                _ = blobStorage.SaveBlobAsync(vendorId.Value.ToString(), blobName, stream, contentType.Value).Result;
            }
            // ファイルが既に存在する場合は削除する。
            DeleteLocalFile(filePath.Value);
        }

        /// <summary>
        /// Blobからファイルストリームを取得する。
        /// </summary>
        public Stream GetFileStream(
            VendorId vendorId,
            FileId fileId,
            FileName fileName,
            AttachFileStorageId attachFilestorageId)
        {
            try
            {
                var connectionString = GetConnectionString(attachFilestorageId.Value.ToString());
                var blobStorage = GetBlobStorage(connectionString);
                var blob = blobStorage.GetBlockBlobAsync(vendorId.Value.ToString(), $"{fileId.Value.ToString()}/{fileName.Value}").Result;
                return blob.OpenReadAsync().Result;
            }
            catch (StorageException se)
            {
                if (se.RequestInformation.HttpStatusCode == (int)System.Net.HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(se.Message);
                }
                throw;
            }
        }

        /// <summary>
        /// Blobからファイルを削除する。
        /// </summary>
        public void DeleteFile(
            VendorId vendorId,
            FileId fileId,
            FileName fileName,
            AttachFileStorageId attachFilestorageId)
        {
            var connectionString = GetConnectionString(attachFilestorageId.Value.ToString());
            var blobStorage = GetBlobStorage(connectionString);
            var blob = blobStorage.GetBlockBlobAsync(vendorId.Value.ToString(), $"{fileId.Value.ToString()}/{fileName.Value}").Result;

            blobStorage.DeleteBlobAsync(blob).Wait();
            return;
        }


        private bool StreamSave(string filePath, Stream inputStream, long appendPosition)
        {
            string dir = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var size = inputStream.Length;

            using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                if (stream.Length != appendPosition)
                {
                    return false;
                }

                byte[] buf = new byte[size];
                inputStream.Position = 0;

                int len;
                while ((len = inputStream.Read(buf, 0, buf.Length)) > 0)
                    stream.Write(buf, 0, len);
            }
            return true;
        }


        private void DeleteLocalFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        /// <summary>
        /// ストレージIDからコネクションストリングを取得する
        /// </summary>
        private string GetConnectionString(string storageId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    a.*
FROM
    attach_file_storage a
WHERE
    a.attachfile_storage_id = /*ds storageId*/'1' 
AND a.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    a.*
FROM
    AttachFileStorage a
WHERE
    a.attachfile_storage_id = @storageId
AND a.is_active = 1
";
            }
            var param = new { storageId = storageId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Cache.Get<DB_AttachFileStorage>(
                CacheManager.CreateKey(CACHE_KEY_ATTACH_FILE_STORAGE_CONNECTIONSTRING, storageId),
                s_cacheExpireTime,
                () => SqlConnection.Query<DB_AttachFileStorage>(twowaySql.Sql, dynParams).FirstOrDefault());

            return result.connection_string;
        }

        private IBlobStorage GetBlobStorage(string connectionString)
        {
            return UnityCore.Resolve<IBlobStorage>("AttachFileBlobStorage", new ParameterOverride("connectionString", new InjectionParameter<string>(connectionString)));
        }
    }
}

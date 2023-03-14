using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Log;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using Oci.Common.Model;
using System.Net;

namespace JP.DataHub.ApiWeb.Infrastructure.DataRepository
{
    // .NET6
    internal class OracleDynamicApiAttachFileRepository : OciObjectStorageDataStoreRepository, IDynamicApiAttachFileRepository
    {
        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(OracleDynamicApiAttachFileRepository));

        /// <summary>
        /// 添付ファイルのアップロード
        /// </summary>
        public void UploadAttachFile(DynamicApiAttachFileInformation info, DynamicApiAttachFileInputStream input)
        {
            logger.Info($"UploadAttachFile start");
            //FileUpload
            logger.Info($"UploadAttachFile LocalStorage Save Start");
            var path = TempFileUpload(info, input);
            logger.Info($"UploadAttachFile LocalStorage Save End");
            if (input.IsEndStream)
            {
                logger.Info($"UploadAttachFile LocalStorage BlobUpload Start");
                BlobUpload(info, path);
                logger.Info($"UploadAttachFile LocalStorage BlobUpload End");
            }
        }
        /// <summary>
        /// Blobからファイルストリームを取得する
        /// </summary>
        public Stream GetAttachFileFileStream(DynamicApiAttachFileInformation info)
        {
            try
            {
                SetAttachFileFromat();
                return QueryToStream(new QueryParam(new FilePath(info.FilePath)));
            }
            catch (AggregateException ae) when (ae.InnerException is OciException oe)
            {
                if (oe.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(oe.Message);
                }
                throw;
            }
        }
        /// <summary>
        /// Blobからファイルを削除する
        /// </summary>
        public void DeleteAttachFile(DynamicApiAttachFileInformation info)
        {
            DeleteOnce(new DeleteParam(new FilePath(info.FilePath), throwNotFoundExcption: true));
        }

        private string TempFileUpload(DynamicApiAttachFileInformation info, DynamicApiAttachFileInputStream input)
        {
            logger.Info($"UploadAttachFile TempFileUpload start");
            var filePath = Path.Combine(UnityCore.Resolve<string>("AttachFileTmpPath"), info.FileId, info.BlobFileName);
            if (!input.IsAppendStream)
            {
                //ファイルが既に存在する場合は削除する。
                logger.Info($"UploadAttachFile DeleteLocalFile start {filePath}");
                DeleteLocalFile(filePath);
                logger.Info($"UploadAttachFile DeleteLocalFile end");
            }
            logger.Info($"UploadAttachFile StreamSave start");
            if (!StreamSave(filePath, input.InputStream, input.AppendPosition).Result)
            {
                throw new Exception("not match uploaded file position");
            }
            logger.Info($"UploadAttachFile StreamSave end");
            return filePath;
        }

        private void BlobUpload(DynamicApiAttachFileInformation info, string filePath)
        {
            logger.Info($"UploadAttachFile LocalStorage BlobUpload Method Start");
            SetAttachFileFromat();
            var fileInfo = new FileInfo(filePath);
            logger.Info($"UploadAttachFile LocalStorage BlobUpload Method Blob Write Start");
            using (FileStream stream = fileInfo.OpenRead())
            {
                RegisterOnce(new RegisterParam(stream, null, new FilePath(info.FilePath), info.ContentType, false));
            }
            logger.Info($"UploadAttachFile LocalStorage BlobUpload Method Blob Write End");
            logger.Info($"UploadAttachFile LocalStorage BlobUpload Method Delete LocalFile Start");
            //ファイルが既に存在する場合は削除する。
            DeleteLocalFile(filePath);
            logger.Info($"UploadAttachFile LocalStorage BlobUpload Method Delete LocalFile End");
        }


        /// <summary>
        /// ファイルのアップロードをする
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="inputStream"></param>
        /// <param name="appendPosition"></param>
        private async Task<bool> StreamSave(string filePath, Stream inputStream, long appendPosition)
        {
            logger.Info($"UploadAttachFile StreamSave Method start");
            string dir = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dir))
            {
                logger.Info($"UploadAttachFile StreamSave Method Create Directory Start");
                Directory.CreateDirectory(dir);
                logger.Info($"UploadAttachFile StreamSave Method Create Directory End");
            }

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            logger.Info($"UploadAttachFile StreamSave file write start");
            using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                if (stream.Length != appendPosition)
                {
                    return false;
                }
                
                await inputStream.CopyToAsync(stream);
            }
            logger.Info($"UploadAttachFile StreamSave file write end");
            return true;
        }

        public string GetFiletoBase64String(string filePath)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    var stream = QueryToStream(new QueryParam(new FilePath(filePath, isAbsolutePath: true)));
                    stream.CopyTo(ms);
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch (NotFoundException)
                {
                    return "error_notfound";
                }
            }
        }

        public Uri GetUriWithSharedAccessSignature(DynamicApiAttachFileInformation info)
        {
            try
            {
                SetAttachFileFromat();
                return GetUriWithSharedAccessSignature(new QueryParam(new FilePath(info.FilePath)));
            }
            catch (OciException oe)
            {
                if (oe.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException(oe.Message);
                }
                throw;
            }
        }

        public void UploadBase64ToFile(VendorId vendorId, string base64String, string filePath)
        {
            SetBase64AttachFileFromat();
            byte[] byteArray = Convert.FromBase64String(base64String);
            RegisterOnce(new RegisterParam(new MemoryStream(byteArray), vendorId, new FilePath(filePath), null, false));
        }

        public void DeleteFiletoBase64(VendorId vendorId, string filePath)
        {
            DeleteOnce(new DeleteParam(vendorId, new FilePath(filePath), throwNotFoundExcption: false));
        }

        public void DeleteFilestoBase64(VendorId vendorId, string prefix)
        {
            Delete(new DeleteParam(vendorId, new FilePath(prefix), throwNotFoundExcption: false));
        }

        private void SetAttachFileFromat()
        {
            DefaultContainerFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => GetRelativePathToStorageContainerName(dic[nameof(RegisterParam.FilePath)]);
            DefaultFileNameFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => GetRelativePathToStorageName(dic[nameof(RegisterParam.FilePath)]);
        }
        private void SetBase64AttachFileFromat()
        {
            DefaultContainerFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => dic[nameof(RegisterParam.VendorId)];
            DefaultFileNameFormat = (Dictionary<string, string> dic, JToken json, string defaultContainerName) => dic[nameof(RegisterParam.FilePath)];
        }

        private void DeleteLocalFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }
    }
}
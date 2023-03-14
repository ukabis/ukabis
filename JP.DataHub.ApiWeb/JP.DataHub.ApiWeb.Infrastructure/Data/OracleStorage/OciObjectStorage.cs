using System.Net;
using Oci.Common;
using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;
using JP.DataHub.Com.Log;
using JP.DataHub.ApiWeb.Infrastructure.Configuration;
using Oci.ObjectstorageService.Models;
using Oci.ObjectstorageService.Responses;
using System.Text;
using JP.DataHub.Com.Extensions;
using Oci.Common.Model;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage
{
    /// <summary>
    /// Object Storage Storage.
    /// </summary>
    public class OciObjectStorage : AbstractOciStorage, IOciObjectStorage
    {
        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(OciObjectStorage));
        private readonly ObjectStorageClient _objectStorageClient;
        private readonly string _namespaceName;
        private readonly string _bucketName;
        private readonly string _hostName;

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public OciObjectStorage()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="storageSetting">AzureStorageSetting</param>
        public OciObjectStorage(OciStorageSetting storageSetting)
            : base(storageSetting)
        {
            _namespaceName = storageSetting.NameSpaceName;
            _bucketName = storageSetting.BucketName;
            _hostName = storageSetting.HostName;
            string configurationFilePath = storageSetting.ConfigPath;
            string configProfile = storageSetting.ProfileName;
            string pemFilePath = storageSetting.PemFilePath;
            var pemFile = new FilePrivateKeySupplier(pemFilePath, null);
            ConfigFile configWithProfile = ConfigFileReader.Parse(configurationFilePath, configProfile);
            var provider = new ConfigFileAuthenticationDetailsProvider(configurationFilePath, configProfile, pemFile);
            _objectStorageClient = new ObjectStorageClient(provider, new ClientConfiguration());

        }

        /// <summary>
        /// Get Object.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">ObjectStorage Name</param>
        /// <returns>GetObjectResponse</returns>
        public async Task<GetObjectResponse> GetObjectAsync(string containerName, string objectName)
        {
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                var response = await _objectStorageClient.GetObject(new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectName = objectPathName,
                    NamespaceName = _namespaceName,
                });
                return response;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Get Object.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">ObjectStorage</param>
        /// <returns>GetObjectResponse</returns>
        public GetObjectResponse GetObject(string containerName, string objectName)
        {
            try
            {
                return _objectStorageClient.GetObject(new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectName = GetObjectPathName(objectName, containerName),
                    NamespaceName = _namespaceName,
                }).Result;
            }
            catch (OciException oe)
            {
                throw oe;
            }
}

        /// <summary>
        /// Get HeadObject.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">ObjectStorage</param>
        /// <returns>HeadObjectResponse</returns>
        public HeadObjectResponse HeadObject(string containerName, string objectName)
        {
            try
            {
                return _objectStorageClient.HeadObject(new HeadObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectName = GetObjectPathName(objectName, containerName),
                    NamespaceName = _namespaceName,
                }).Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Object.
        /// </summary>
        /// <param name="srcUri">Uri</param>
        /// <returns>GetObjectResponse</returns>
        public async Task<GetObjectResponse> GetObjectAsync(Uri srcUri)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the specified file if the file does not exists.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="objectName">Name of the Object.</param>
        /// <param name="message">The message.</param>
        /// <returns>Object StorageのURI</returns>
        public async Task<string> CreateNewObjectAsync(string containerName, string objectName, string message)
        {
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                var putObjectRequest = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    NamespaceName = _namespaceName,
                    ObjectName = objectPathName,
                    PutObjectBody = await GenerateStreamFromString(message),
                    ContentType = "text/plain"
                };
                var result = await _objectStorageClient.PutObject(putObjectRequest);
                return objectPathName;
            }
            catch (OciException oe)
            {
                if (oe.StatusCode == HttpStatusCode.Conflict)
                {
                    return null;
                }
                throw oe;
            }
        }

        /// <summary>
        /// Saves the specified container name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="objectName">Name of the Object.</param>
        /// <param name="message">The message.</param>
        /// <returns>Object StorageのURI</returns>
        public async Task<string> SaveObjectAsync(string containerName, string objectName, Stream message)
        {
            try 
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                var result = _objectStorageClient.PutObject(new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectName = objectPathName,
                    NamespaceName = _namespaceName,
                    PutObjectBody = message
                }).Result;
                return objectPathName;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Saves the specified container name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="objectName">Name of the Object.</param>
        /// <param name="message">The message.</param>
        /// <param name="contentType">The contentType.</param>
        /// <returns>Object StorageのURI</returns>
        public async Task<string> SaveObjectAsync(string containerName, string objectName, Stream message, string contentType)
        {
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                var result = _objectStorageClient.PutObject(new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectName = objectPathName,
                    NamespaceName = _namespaceName,
                    PutObjectBody = message,
                    ContentType = contentType
                }).Result;
                return objectPathName;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Save Object.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">Object Name</param>
        /// <param name="srcUri">Src Object</param>
        /// <returns>Uri</returns>
        public async Task<string> SaveObjectAsync(string containerName, string objectName, Uri srcUri)
        {
            try
            {
                var destObjectName = GetObjectPathName(objectName, containerName);

                WebClient wc = new WebClient();
                using (MemoryStream stream = new MemoryStream(wc.DownloadData(srcUri.ToString())))
                {
                    var result = _objectStorageClient.PutObject(new PutObjectRequest()
                    {
                        BucketName = _bucketName,
                        ObjectName = destObjectName,
                        NamespaceName = _namespaceName,
                        PutObjectBody = stream
                    }).Result;
                    return destObjectName;
                }
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Save Object.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">Object Name</param>
        /// <param name="message">Message</param>
        /// <returns>Uri</returns>
        public async Task<string> SaveObjectAsync(string containerName, string objectName, string message)
        {
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                var putObjectRequest = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    NamespaceName = _namespaceName,
                    ObjectName = objectPathName,
                    PutObjectBody = await GenerateStreamFromString(message),
                    ContentType = "text/plain"
                };
                var result = _objectStorageClient.PutObject(putObjectRequest);
                return objectPathName;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Saves the specified container name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="objectName">Name of the Object.</param>
        /// <param name="message">The message.</param>
        /// <param name="contentType">The contentType.</param>
        /// <returns>objectFileのURI</returns>
        public async Task<string> SaveObjectAsync(string containerName, string objectName, string message, string contentType)
        {
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                var putObjectRequest = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    NamespaceName = _namespaceName,
                    ObjectName = objectPathName,
                    PutObjectBody = await GenerateStreamFromString(message),
                    ContentType = contentType
                };
                var result = _objectStorageClient.PutObject(putObjectRequest);
                return objectPathName;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Delete Object.
        /// </summary>
        /// <param name="objectPathName">Cloud ObjectStorage Path</param>
        public async Task<bool?> DeleteObjectAsync(string objectPathName)
        {
            try
            {
                if (objectPathName == null)
                {
                    return false;
                }
                var result = await _objectStorageClient.DeleteObject(new DeleteObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectName = objectPathName,
                    NamespaceName = _namespaceName
                });
                return result.IsDeleteMarker;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Delete Object.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">Object Name</param>
        public async Task DeleteObjectAsync(string containerName, string objectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete Object.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">Object Name</param>
        public async Task<bool?> DeleteIfExistsAsync(string containerName, string objectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move Object.
        /// </summary>
        /// <param name="srcContainerName">Source ContainerName</param>
        /// <param name="srcObjectName">Source Object Name</param>
        /// <param name="destContainerName">Destination ContainerName</param>
        /// <param name="destObjectName">Destination Object Name</param>
        /// <returns>Uri</returns>
        public async Task<string> MoveObjectAsync(string srcContainerName, string srcObjectName, string destContainerName, string destObjectName)
        {
            try
            {
                var objectFile = await GetObjectAsync(srcContainerName, srcObjectName);
                if (objectFile == null)
                {
                    return null;
                }

                var destUri = await SaveObjectAsync(destContainerName, destObjectName, objectFile.InputStream);
                await DeleteObjectAsync(GetObjectPathName(srcObjectName, srcContainerName));
                return destUri;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Copy Object.
        /// </summary>
        /// <param name="srcUri">Source ContainerName</param>
        /// <param name="destContainerName">Destination ContainerName</param>
        /// <param name="destObjectName">Destination Object Name</param>
        /// <returns>Uri</returns>
        public async Task<string> CopyObject(Uri srcUri, string destContainerName, string destObjectName)
        {
            try
            {
                var destUri = await SaveObjectAsync(destContainerName, destObjectName, srcUri);
                return destUri;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// Copy Object.
        /// </summary>
        /// <param name="srcContainerName">Source ContainerName</param>
        /// <param name="srcObjectName">Source Object Name</param>
        /// <param name="destContainerName">Destination ContainerName</param>
        /// <param name="destObjectName">Destination Object Name</param>
        /// <returns>Uri</returns>
        public async Task<string> CopyObjectAsync(string srcContainerName, string srcObjectName, string destContainerName, string destObjectName)
        {
            try
            {
                var src = GetObject(srcContainerName, srcObjectName);
                var destUri = await SaveObjectAsync(destContainerName, destObjectName, src.InputStream);
                return destUri;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        /// <summary>
        /// GetObjectPreSignedUrl.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="objectName">Blob Name</param>
        /// <param name="expireMinutes">expireMinutes</param>
        /// <returns>Uri</returns>
        public async Task<string> GetObjectPreSignedUrlAsync(
            string containerName,
            string objectName,
            int expireMinutes = 5)
        {
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);

                var createPreauthenticatedRequestDetails = new CreatePreauthenticatedRequestDetails
                {
                    Name = objectPathName,
                    BucketListingAction = PreauthenticatedRequest.BucketListingActionEnum.Deny,
                    ObjectName = objectPathName,
                    AccessType = CreatePreauthenticatedRequestDetails.AccessTypeEnum.AnyObjectReadWrite,
                    TimeExpires = DateTime.UtcNow.AddMinutes(expireMinutes)
                };

                var createPreauthenticatedRequestRequest = new CreatePreauthenticatedRequestRequest
                {
                    NamespaceName = _namespaceName,
                    BucketName = _bucketName,
                    CreatePreauthenticatedRequestDetails = createPreauthenticatedRequestDetails,
                };

                var result = await _objectStorageClient.CreatePreauthenticatedRequest(createPreauthenticatedRequestRequest);
                var presignedUrl = "https://" + _hostName + result.PreauthenticatedRequest.AccessUri + result.PreauthenticatedRequest.ObjectName;
                return presignedUrl;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        public async Task<GetObjectResponse> PutObjectAsync(string containerName, string objectName, string contentType, string blockId, byte[] chunk, bool commit, List<string> ids = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Append Object, If not exist target block, create this.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="objectName"></param>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns>Uri</returns>
        public async Task<string> AppendObjectAsync(string containerName, string objectName, Stream message, string contentType = null)
        {
            string tempFileName = Path.GetTempFileName();
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                var headObject = HeadObject(containerName, objectName);
                if (headObject == null)
                {
                    await CreateNewObjectAsync(containerName, objectName, "");
                }

                var objectFile = await GetObjectAsync(containerName, objectName);

                using (MemoryStream ms = new MemoryStream())
                {
                    objectFile.InputStream.CopyTo(ms);
                    message.CopyTo(ms);
                    ms.Position = 0;
                    var result = await SaveObjectAsync(containerName, objectName, ms, contentType);
                    return objectPathName;
                }
            }
            catch (OciException oe)
            {
                throw oe;
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        /// <summary>
        /// Append Object, create this regardless of its existence.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="objectName"></param>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns>Uri</returns>
        public async Task<string> OverwriteAppendObjectAsync(string containerName, string objectName, Stream message, string contentType = null)
        {
            string tempFileName = Path.GetTempFileName();
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                await CreateNewObjectAsync(containerName, objectName, "");

                var objectFile = await GetObjectAsync(containerName, objectName);

                using (MemoryStream ms = new MemoryStream())
                {
                    objectFile.InputStream.CopyTo(ms);
                    message.CopyTo(ms);
                    ms.Position = 0;
                    var result = await SaveObjectAsync(containerName, objectName, ms, contentType);
                    return objectPathName;
                }
            }
            catch (OciException oe)
            {
                throw oe;
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        /// <summary>
        /// Append Object, create this regardless of its existence.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="objectName"></param>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns>Uri</returns>
        public async Task<string> OverwriteAppendObjectAsync(string containerName, string objectName, string message, string contentType = null)
        {
            string tempFileName = Path.GetTempFileName();
            try
            {
                var objectPathName = GetObjectPathName(objectName, containerName);
                await CreateNewObjectAsync(containerName, objectName, "");

                var objectFile = await GetObjectAsync(containerName, objectName);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sWriter = new StreamWriter(tempFileName, true))
                    {
                        Encoding encoding = Encoding.GetEncoding("utf-8");
                        sWriter.Write(encoding.GetString(objectFile.InputStream.ToByteArray()));
                        byte[] bytesUTF8 = encoding.GetBytes(message);
                        sWriter.Write(encoding.GetString(bytesUTF8));
                        sWriter.Flush();
                    }
                    using (StreamReader sReader = new StreamReader(tempFileName))
                    {
                        sReader.BaseStream.CopyTo(ms);
                    }
                    ms.Position = 0;
                    var result = await SaveObjectAsync(containerName, objectName, ms, contentType);
                    return objectPathName;
                }
            }
            catch (OciException oe)
            {
                throw oe;
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }

        public async Task<GetObjectResponse> GetAppendObjectAsync(string containerName, string objectName)
        {
            return await GetObjectAsync(containerName, objectName);
        }

        public async Task<bool?> DeleteAppendObjectAsync(string containerName, string objectName)
        {
            return await DeleteObjectAsync(GetObjectPathName(containerName, objectName));
        }

        public async Task<ListObjectsResponse> GetObjectListAsync(string prefix)
        {
            try
            {
                var list = await _objectStorageClient.ListObjects(new ListObjectsRequest()
                {
                    BucketName = _bucketName,
                    Prefix = prefix,
                    NamespaceName = _namespaceName
                });
                return list;
            }
            catch (OciException oe)
            {
                throw oe;
            }
        }

        public async Task<GetObjectResponse> GetAnyObjectAsync(string containerName, string objectName)
        {
            throw new NotImplementedException();
        }

        private string GetObjectPathName(string objectName, string containerName)
        {
            return Path.Combine(containerName, objectName).Replace("\\", "/");
        }

        private async Task<Stream> GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        Task<string> IOciObjectStorage.GetObjectPathName(string objectName, string containerName)
        {
            throw new NotImplementedException();
        }

        Task<Stream> IOciObjectStorage.GenerateStreamFromString(string s)
        {
            throw new NotImplementedException();
        }
    }
}

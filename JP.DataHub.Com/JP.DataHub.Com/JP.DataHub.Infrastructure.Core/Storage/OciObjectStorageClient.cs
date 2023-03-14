using Oci.Common;
using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Storage
{
    public class OciObjectStorageClient : IStorageClient
    {
        private readonly ObjectStorageClient _objectStorageClient;
        private readonly string _namespaceName;
        private readonly string _bucketName;
        private readonly string _rootPath;

        public OciObjectStorageClient(string configFile, string nameSpaceName, string bucketName, string rootPath)
        {
            _bucketName = bucketName;
            _namespaceName = nameSpaceName;
            _rootPath = rootPath;
            ConfigFile configWithProfile = ConfigFileReader.Parse(configFile);
            var provider = new ConfigFileAuthenticationDetailsProvider(configWithProfile);
            _objectStorageClient = new ObjectStorageClient(provider, new ClientConfiguration());
        }

        public void CopyTo(string objectName, Stream createObject) => CopyToAsync(objectName, createObject).Wait();

        public Task CopyToAsync(string objectName, Stream createObject)
        {
            objectName = GetObjectName(objectName, _rootPath);
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = _bucketName,
                NamespaceName = _namespaceName,
                ObjectName = objectName,
                PutObjectBody = createObject
            };
            return _objectStorageClient.PutObject(putObjectRequest);
        }

        public Stream GetStream(string objectName)
        {
            objectName = GetObjectName(objectName, _rootPath);
            var result = _objectStorageClient.GetObject(new GetObjectRequest()
            {
                BucketName = _bucketName,
                ObjectName = objectName,
                NamespaceName = _namespaceName,
            }).Result;
            return result.InputStream;
        }

        public void Delete(string objectName) => DeleteAsync(objectName).Wait();

        public Task DeleteAsync(string objectName)
        {
            objectName = GetObjectName(objectName, _rootPath);
            return _objectStorageClient.DeleteObject(new DeleteObjectRequest()
            {
                BucketName = _bucketName,
                ObjectName = objectName,
                NamespaceName = _namespaceName,
            });
        }

        public IEnumerable<string> List(string prefix)
        {
            prefix = GetObjectName(prefix, _rootPath);
            var list = _objectStorageClient.ListObjects(new ListObjectsRequest()
            {
                BucketName = _bucketName,
                Prefix = prefix,
                NamespaceName = _namespaceName,
            }).Result;
            return list.ListObjects.Objects.Select(x => x.Name);
        }

        public bool Exist(string objectName)
        {
            var list = List(objectName);
            objectName = GetObjectName(objectName, _rootPath);
            return list.Any(x => x == objectName);
        }

        public long GetSize(string objectName)
        {
            throw new NotImplementedException();
        }

        private string GetObjectName(string objectName, string rootPath)
        {
            if(string.IsNullOrEmpty(rootPath))
            {
                return objectName;
            }
            else
            {
                return Path.Combine(rootPath, objectName);
            }
        }

        public string GetObjectPath(string objectName)
        {
            throw new NotImplementedException();
        }
    }
}

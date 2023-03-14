using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Serializer;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.Cache;

namespace JP.DataHub.ApiWeb.Infrastructure.Cache
{
    public class BlobCache : AbstractCache, IBlobCache
    {
        private IConfiguration _configuration { get; } = UnityCore.Resolve<IConfiguration>();

        //[Dependency]
        protected ISerializer Serializer { get; set; }

        private readonly JPDataHubLogger _log = new JPDataHubLogger(typeof(BlobCache));

        private readonly string _targetConnection;
        private readonly string _containerName;

        private static readonly Dictionary<string, CloudBlobContainer> ReadConnectionDic = new Dictionary<string, CloudBlobContainer>();
        private static readonly Dictionary<string, CloudBlobContainer> WriteConnectionDic = new Dictionary<string, CloudBlobContainer>();


        public CloudBlobContainer ReadBlobCache
        {
            get
            {
                if (!ReadConnectionDic.ContainsKey(_targetConnection))
                {
                    ReadConnectionDic.Add
                    (
                        _targetConnection,
                        CloudStorageAccount
                            .Parse(_configuration.GetValue<string>($"ConnectionStrings:{_targetConnection}"))
                            .CreateCloudBlobClient().GetContainerReference(_containerName)
                    );
                }

                return ReadConnectionDic[_targetConnection];
            }
        }

        public CloudBlobContainer WriteBlobCache
        {
            get
            {
                if (!WriteConnectionDic.ContainsKey(_targetConnection))
                {
                    WriteConnectionDic.Add
                    (
                        _targetConnection,
                        CloudStorageAccount
                            .Parse(_configuration.GetValue<string>($"ConnectionStrings:{_targetConnection}"))
                            .CreateCloudBlobClient().GetContainerReference(_containerName)
                    );
                }

                return WriteConnectionDic[_targetConnection];
            }
        }


        public BlobCache(string targetConnection)
        {
            _targetConnection = targetConnection;
            _containerName = _configuration.GetValue<string>("AppConfig:BlobCacheContainerName");
        }

        public override void Close()
        {
        }

        public override void Clear()
        {
            // 手動削除だから処理はしない
        }

        public override bool Contains(string key)
        {
            if (IsFlash)
            {
                return false;
            }
            try
            {
                return ReadBlobCache.GetBlockBlobReference(key).ExistsAsync().Result;
            }
            catch (AggregateException ex)
            {
                _log.Error($"Cache Exception ExistKey Message={ex.Message}");
                foreach (var inner in ex.InnerExceptions)
                {
                    _log.Error($"Cache Inner Exception ExistKey Message={inner.Message}");
                }
                return false;
            }
        }

        public override void Add(string key, object obj, TimeSpan absoluteExpiration) => Add(key, obj, absoluteExpiration, 0);
        public override void Add(string key, object obj, TimeSpan absoluteExpiration, int maxSaveSize)
        {
            try
            {
                var tempSerialized = Serializer.SerializeByte(obj);
                if (maxSaveSize > 0 && tempSerialized.Length > maxSaveSize)
                {
                    return;
                }
                using (MemoryStream memoryStream = new MemoryStream(tempSerialized))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    CloudBlockBlob blob = WriteBlobCache.GetBlockBlobReference(key);
                    blob.UploadFromStreamAsync(memoryStream);
                }
            }
            catch (AggregateException ex)
            {
                _log.Error($"Cache Exception Add Message={ex.Message}");
                foreach (var inner in ex.InnerExceptions)
                {
                    _log.Error($"Cache Inner Exception Add Message={inner.Message}");
                }
            }
        }

        public override void Remove(string key)
        {
            // 手動削除だから処理はしない
        }

        public override void RemoveFirstMatch(string key)
        {
            // 手動削除だから処理はしない
        }

        public override T Get<T>(string key, out bool isNullValue, bool isUseMessagePack = false)
        {
            isNullValue = false;
            if (IsFlash)
            {
                return default(T);
            }
            byte[] tempData = null;
            try
            {
                var blob = ReadBlobCache.GetBlockBlobReference(key);
                //ローカルで動かすとたまにblob.Properties.Lengthが失敗するので
                if (blob.ExistsAsync().Result)
                {
                    tempData = new byte[blob.Properties.Length];
                    var tmp = blob.DownloadToByteArrayAsync(tempData, 0).Result;
                }
            }
            catch (AggregateException ex)
            {
                _log.Error($"Cache Exception Get Message={ex.Message}");
                foreach (var inner in ex.InnerExceptions)
                {
                    _log.Error($"Cache Inner Exception Get Message={inner.Message}");
                }
            }
            if (tempData == null)
            {
                return default(T);
            }

            return Serializer.Deserialize<T>(tempData);
        }

        //public override CacheResult<T> GetObject<T>(string key)
        //{
        //    if (IsFlash)
        //    {
        //        return new CacheResult<T>(false, default(T));
        //    }
        //    byte[] tempData = null;
        //    try
        //    {
        //        var blob = ReadBlobCache.GetBlockBlobReference(key);
        //        //ローカルで動かすとたまにblob.Properties.Lengthが失敗するので
        //        if (blob.ExistsAsync().Result)
        //        {
        //            tempData = new byte[blob.Properties.Length];
        //            var tmp = blob.DownloadToByteArrayAsync(tempData, 0).Result;
        //        }
        //    }
        //    catch (AggregateException ex)
        //    {
        //        _log.Error($"Cache Exception Get Message={ex.Message}");
        //        foreach (var inner in ex.InnerExceptions)
        //        {
        //            _log.Error($"Cache Inner Exception Get Message={inner.Message}");
        //        }
        //    }
        //    if (tempData == null)
        //    {
        //        return new CacheResult<T>(false, default(T));
        //    }

        //    var tempSerialized = Serializer.Deserialize<T>(tempData);
        //    return new CacheResult<T>(true, tempSerialized);
        //}

        public override IEnumerable<string> Keys()
        {
            var result = ReadBlobCache.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, null, null, null).Result;
            foreach (var key in result.Results)
            {
                yield return ((CloudBlockBlob)key).Name;
            }
        }

        //public override IEnumerable<string> KeysFirstMatch(string keyPattern)
        //{
        //    var result = ReadBlobCache.ListBlobsSegmentedAsync(keyPattern, true, BlobListingDetails.Metadata, null, null, null, null).Result;
        //    foreach (var key in result.Results)
        //    {
        //        yield return ((CloudBlockBlob)key).Name;
        //    }
        //}
    }
}

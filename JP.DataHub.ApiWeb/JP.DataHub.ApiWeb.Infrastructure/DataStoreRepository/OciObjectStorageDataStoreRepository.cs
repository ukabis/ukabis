using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Configuration;
using JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using Oci.Common.Model;
using System.Net;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    internal class OciObjectStorageDataStoreRepository : NewAbstractDynamicApiDataStoreRepository, INewBlobDataStoreRepository
    {
        public override bool CanQuery { get => false; }
        public override bool CanOptimisticConcurrency { get => false; }

        public Func<Dictionary<string, string>, JToken, string, string> DefaultContainerFormat { get; set; }
        public Func<Dictionary<string, string>, JToken, string, string> DefaultFileNameFormat { get; set; }
        public Func<string> DefaultConnectionString { get; set; }
        public Func<Tuple<Guid?, Guid?>> DefaultRepositoryIds { get; set; }
        public Func<Encoding> DefaultEncoding { get; set; }

        [Dependency("DynamicApi")]
        public IJPDataHubDbConnection DbConnection { get; set; }

        [Dependency]
        public ICache Cache { get; set; }

        protected readonly TimeSpan cacheExpireTime = TimeSpan.Parse("0:30:00");

        [CacheKey(CacheKeyType.Entity, "repository")]
        public static string CACHE_KEY_ALL_GETCONNECTIONSTRING = "NewDataStoreRepository-GetConnectionString";


        public override JsonDocument QueryOnce(QueryParam param)
        {
            var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
            OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
            string container = ToContainerName(null);
            string filename = ToFileName(null);
            var objectFile = objectStorage.GetObjectAsync(container, filename).Result;

            Stream stream = objectFile.InputStream;
            var json = stream.ReadToEnd().ToJson();
            return new JsonDocument(json);
        }

        public Uri GetUriWithSharedAccessSignature(QueryParam query)
        {
            var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
            OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
            string container;
            string filename;
            (container, filename) = GetContainerObjectName(query.FilePath.Value);

            return new Uri(objectStorage.GetObjectPreSignedUrlAsync(container, filename, expireMinutes: 30).Result);
        }

        public Stream QueryToStream(QueryParam param)
        {
            try
            {
                string container;
                string filename;

                if (param.HasFilePath)
                {
                    string connectionstring = ToConnectionString(RepositoryInfo.ConnectionString);
                    if (param.FilePath.IsAbsolutePath)
                    {
                        string endpoint = GetFilePathToEndpoint(param.FilePath.Value);
                        connectionstring = ToConnectionString(RepositoryInfo.GetConnectionStringFromEndPoint(endpoint));
                    }
                    var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
                    OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
                    (container, filename) = GetContainerObjectName(param.FilePath.Value);
                    var objectFile = objectStorage.GetObjectAsync(container, filename).Result;
                    return objectFile.InputStream;
                }
                else
                {
                    var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
                    OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
                    container = ToContainerName(null);
                    filename = ToFileName(null);

                    var objectFile = objectStorage.GetObjectAsync(container, filename).Result;
                    return objectFile.InputStream;
                }
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

        public override IList<JsonDocument> Query(QueryParam param, out XResponseContinuation xResponseContinuation)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<JsonDocument> QueryEnumerable(QueryParam param)
        {
            yield break;
        }

        public override RegisterOnceResult RegisterOnce(RegisterParam param)
        {
            // MEMO:添付ファイルの場合
            // BlobContainer：VendorId
            // パス：FieldId（添付ファイルごとに振られるユニークなID）
            // ファイル名：元のファイル名


            var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
            OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
            var container = ToContainerName(param, param.Json);
            var filename = ToFileName(param, param.Json, param.NameToLower);

            if (param.HasStream)
            {
                param.Stream.Seek(0, SeekOrigin.Begin);
                _ = objectStorage.SaveObjectAsync(container, filename, param.Stream).Result;
            }
            else if (param.SourceUri != null)
            {
                _ = objectStorage.CopyObject(param.SourceUri, container, filename).Result;
            }
            else
            {
                byte[] byteArray = ToEncoding().GetBytes(param.Json.ToString());
                using (var memoryStream = new MemoryStream(byteArray))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    _ = objectStorage.SaveObjectAsync(container, filename, memoryStream).Result;
                }
            }
            var add = new Dictionary<string, object>();
            add.Add("Container", container);
            return new RegisterOnceResult(filename, add);
        }

        public override void DeleteOnce(DeleteParam param)
        {
            try
            {
                string containerName = "";
                string storageName = "";
                string connectionString = ToConnectionString(RepositoryInfo.ConnectionString);

                if (param.HasFilePath)
                {
                    if (param.FilePath.IsAbsolutePath)
                    {
                        string endpoint = GetFilePathToEndpoint(param.FilePath.Value);
                        connectionString = ToConnectionString(RepositoryInfo.GetConnectionStringFromEndPoint(endpoint));
                    }
                    (containerName, storageName) = GetContainerObjectName(param.FilePath.Value);
                }
                else
                {
                    containerName = ToContainerName(null);
                    storageName = ToFileName(null);
                }

                var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
                OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);

                var objectFile = objectStorage.GetObject(containerName, storageName);
                if (param.ThrowNotFoundExcption)
                {
                    objectStorage.DeleteObjectAsync(param.FilePath.Value).Wait();
                }
                else
                {
                    var tmp = objectStorage.DeleteIfExistsAsync(containerName, storageName).Result;
                }
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

        public override IEnumerable<string> Delete(DeleteParam param)
        {
            try
            {
                var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
                OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
                var list = objectStorage.GetObjectListAsync(param.FilePath.Value).Result;

                if (param.ThrowNotFoundExcption)
                {
                    list.ListObjects.Objects.ForEach(async x => await objectStorage.DeleteObjectAsync(x.Name));
                }
                else
                {
                    list.ListObjects.Objects.ForEach(async x => await objectStorage.DeleteObjectAsync(x.Name));
                }
                return list.ListObjects.Objects.Select(x => x.Name.ToString());
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

        public Uri CopyFile(string destStorageName, string destContainerName, string srcStorageName, string srcContainerName)
        {
            var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
            OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
            return new Uri("https://" + objectStorage.CopyObjectAsync(srcContainerName, srcStorageName, destContainerName ?? srcContainerName, destStorageName).Result);
        }

        public Uri CopyFile(string destStorageName, string destContainerName, Uri srcUri)
        {
            var objectStorageSetting = JsonConvert.DeserializeObject<OciStorageSetting>(RepositoryInfo.ConnectionString);
            OciObjectStorage objectStorage = new OciObjectStorage(objectStorageSetting);
            return new Uri("https://" + objectStorage.CopyObject(srcUri, destContainerName, destStorageName).Result);
        }


        private string ToConnectionString(string connectionString)
        {
            if (DefaultConnectionString != null)
            {
                return DefaultConnectionString();
            }
            else if (DefaultRepositoryIds != null)
            {
                var repositoryIds = DefaultRepositoryIds();
                return GetConnectionString(repositoryIds.Item1, repositoryIds.Item2);
            }
            return connectionString;
        }

        private string GetConnectionString(Guid? repository_group_id, Guid? physical_repository_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var result = Cache.Get<string>(CacheManager.CreateKey(CACHE_KEY_ALL_GETCONNECTIONSTRING, repository_group_id, physical_repository_id), cacheExpireTime, () =>
            {
                if (dbSettings.Type == "Oracle")
                {
                    var sql = @"
SELECT
    pr.connection_string
FROM
    REPOSITORY_GROUP rg
    INNER JOIN PHYSICAL_REPOSITORY pr ON rg.repository_group_id=pr.repository_group_id
WHERE
    rg.repository_group_id= /*ds RepositoryGroupId*/'1'
    AND pr.physical_repository_id= /*ds PhysicalRepositoryId*/'1'
";
                    var param = new { RepositoryGroupId = repository_group_id, PhysicalRepositoryId = physical_repository_id };
                    var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                    return DbConnection.Query<string>(twowaySql.Sql, param).FirstOrDefault();
                }
                else
                {
                    return DbConnection.Query<string>(@"
SELECT
    TOP 1
    pr.connection_string
FROM
    RepositoryGroup rg
    INNER JOIN PhysicalRepository pr ON rg.repository_group_id=pr.repository_group_id
WHERE
    rg.repository_group_id=@RepositoryGroupId
    AND pr.physical_repository_id=@PhysicalRepositoryId
", new { RepositoryGroupId = repository_group_id, PhysicalRepositoryId = physical_repository_id }).FirstOrDefault();
                }
            });
            return result;
        }

        private string ToContainerName(RegisterParam param, JToken json = null)
        {
            string name = null;
            if (DefaultContainerFormat == null)
            {
                name = DefaultContainerName(param);
            }
            else
            {
                name = DefaultContainerFormat(ToDictionary(param), json, DefaultContainerName(param));
            }
            return name.ToLower();
        }

        private string DefaultContainerName(RegisterParam param)
        {
            // Publicなデータ："PublicData"
            // PrivateでVendor依存："Vendor_{VendorId}"
            // Privateで個人依存："Person_{OpenId}"
            // Privateで、Vendor & 個人依存：VendorPerson_{VendorId}_{OpenId}"

            string name = null;
            if (param?.IsVendor?.Value == true && param?.IsPerson?.Value != true)
            {
                // PrivateでVendor & 個人依存
                //Container名の最大長は63文字のためGuidをBase32にして短縮する
                name = $"vp-{Guid.Parse(param.VendorId?.Value).ToBase32String()}-{Guid.Parse(param.OpenId?.Value).ToBase32String()}";
            }
            else if (param?.IsVendor?.Value == true)
            {
                // PrivateでVendor依存
                name = $"Vendor-{param.VendorId?.Value}";
            }
            else if (param?.IsPerson?.Value == true)
            {
                // Privateで個人依存
                name = $"Person-{param.OpenId?.Value}";
            }
            else
            {
                // Publicデータ
                name = "PublicData";
            }
            return name;
        }

        private string ToFileName(RegisterParam param, JToken json = null, bool NameToLower = true)
        {
            string name = null;
            if (DefaultFileNameFormat == null)
            {
                name = DefaultFileName(param, json);
            }
            else
            {
                name = DefaultFileNameFormat(ToDictionary(param), json, DefaultFileName(param, json));
            }
            if (NameToLower && name != null) name = name.ToLower();

            while (name?.StartsWith("/") == true)
            {
                name = name.Substring(1);
            }
            return name;
        }

        private string DefaultFileName(RegisterParam param, JToken json = null)
        {
            // Publicなデータ："{RepositoryKey}"
            // PrivateでVendor依存："{SystemId}\\{RepositoryKey}"
            // Privateで個人依存：???
            // Privateで、Vendor & 個人依存：???

            string name = null;
            //if (param?.IsVendor?.Value == true && param?.IsPerson?.Value != true)
            //{
            //    // PrivateでVendor & 個人依存
            //}
            //else if (param?.IsVendor?.Value == true)
            //{
            //    // PrivateでVendor依存
            //}
            //else if (param?.IsPerson?.Value == true)
            //{
            //    // Privateで個人依存
            //}
            //else
            //{
            //    // Publicデータ
            //    name = ToMap(ToDictionary(json), $"{param?.RepositoryKey?.Value}");
            //}
            name = ToMap(ToDictionary(json), $"{param?.RepositoryKey?.Value}");
            return name;
        }

        private Dictionary<string, string> ToDictionary(JToken json)
        {
            var result = new Dictionary<string, string>();
            if (json != null)
            {
                foreach (JProperty c in json.Children())
                {
                    result.Add(c.Name, c.Value?.ToString());
                }
            }
            return result;
        }

        private Dictionary<string, string> ToDictionary(object param)
        {
            var result = new Dictionary<string, string>();

            if (param == null) return result;

            Type type = param.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in properties)
            {
                var proValue = p.GetValue(param);

                if (proValue != null && proValue.GetType().GetProperties().Any(x => x.Name == "Value"))
                {
                    result.Add(p.Name, proValue.GetType().GetProperty("Value").GetValue(proValue)?.ToString());
                }
                else
                {
                    result.Add(p.Name, p.GetValue(param)?.ToString());
                }
            }

            return result;
        }

        private string ToMap(Dictionary<string, string> dic, string format)
        {
            foreach (var x in dic)
            {
                format = format.Replace($"{{{x.Key}}}", x.Value);
            }
            return format;
        }

        private Encoding ToEncoding()
        {
            if (DefaultEncoding == null)
            {
                return System.Text.Encoding.UTF8;
            }
            else
            {
                return DefaultEncoding();
            }
        }

        private (string, string) GetContainerObjectName(string storagePath)
        {
            string endpoint = GetFilePathToEndpoint(storagePath);
            var tmpPath = storagePath.Substring(endpoint.Length);
            var split = tmpPath.Split('/');
            var containerName = split[0];
            var storageName = tmpPath.Substring(split[0].Length + 1);

            return (containerName, storageName);
        }


        private string GetFilePathToEndpoint(string filePath)
        {
            string pattern = $@"https?://[^/]+/";
            var m = Regex.Match(filePath, pattern);
            if (!m.Success)
            {
                return "";
            }
            return m.Value;
        }

        protected string GetRelativePathToStorageContainerName(string filePath)
        {
            var ps = filePath.Split('/');
            if (ps.Length > 1)
            {
                return ps[0];
            }
            return "";
        }
        protected string GetRelativePathToStorageName(string filePath)
        {
            var index = filePath.IndexOf("/");
            if (index > 1)
            {
                return filePath.Substring(index + 1, filePath.Length - (index + 1));
            }
            return "";
        }
    }
}
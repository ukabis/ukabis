using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Newtonsoft.Json;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class ResourceVersionRepository : IResourceVersionRepository
    {
        [Dependency]
        public ICache Cache { get; set; }

        [Dependency]
        public IPerRequestDataContainer PerRequestDataContainer { get; set; }

        private readonly TimeSpan cacheExpireTime = TimeSpan.Parse("0:30:00");

        public INewDynamicApiDataStoreRepository PhysicalRepository { get; set; }

        public VendorId VendorId { get; set; }
        public SystemId SystemId { get; set; }
        public OpenId OpenId { get; set; }
        public RepositoryKey RepositoryKey { get; set; }
        public IsAutomaticId IsAutomaticId { get; set; }

        private ResourceVersionKey CreateResourceVersionKey(RepositoryKey repositoryKey) => new ResourceVersionKey(repositoryKey);

        /// <summary>
        /// ResourceVersionを取得するのを保持する
        /// これは１回のAPI呼び出しで２回これを呼び出されておりRedisCacheに無駄に負荷をかけてしまっている
        /// よって1回のAPI呼び出し中は使いまわすようにする
        /// </summary>
        private ResourceVersion currentResourceVersion = null;

        public ResourceVersion AddNewVersion(RepositoryKey repositoryKey)
        {
            var key = CreateResourceVersionKey(repositoryKey);
            var doc = GetVersionInfo(key);
            int result = 1;
            if (doc == null)
            {
                InsertNewVersionInfo(key);
            }
            else
            {
                UpdateNewVersionInfo(doc, true);
                result = doc.CurrentVersion;
            }
            this.Cache.Remove(key.Id);
            this.Cache.Remove(key.Id + "~MaxVersion");
            currentResourceVersion = null;
            return new ResourceVersion(result);
        }

        public ResourceVersion GetMaxVersion(RepositoryKey repositoryKey)
        {
            var key = CreateResourceVersionKey(repositoryKey);
            string keyCache = key.Id + "~MaxVersion";
            return this.Cache.Get<ResourceVersion>(keyCache, cacheExpireTime, () =>
            {
                var doc = GetVersionInfo(key);
                if (doc != null)
                {
                    int? maxVersion = doc.DocumentVersions.Max(x => x.Version);
                    return new ResourceVersion(maxVersion.Value);
                }
                else
                {
                    InsertNewVersionInfo(key);
                    return new ResourceVersion(1);
                }
            });
        }

        public ResourceVersion GetRegisterVersion(RepositoryKey repositoryKey, XVersion xversion)
        {
            var currentVersion = GetCurrentVersion(repositoryKey);
            //XverSion 未設定ならカレントバージョン返却
            if (xversion == null || xversion.Value == 0)
            {
                return currentVersion;
            }
            var maxVersion = GetMaxVersion(repositoryKey);
            if (maxVersion.Value < xversion.Value)
            {
                throw new XVersionNotFoundException("Invalid X-Version");
            }
            else
            {
                return new ResourceVersion(xversion.Value);
            }
        }

        public ResourceVersion GetCurrentVersion(RepositoryKey repositoryKey)
        {
            if (currentResourceVersion == null)
            {
                var key = CreateResourceVersionKey(repositoryKey);
                currentResourceVersion = this.Cache.Get<ResourceVersion>(key.Id, cacheExpireTime, () =>
                {
                    var result = GetVersionInfo(key);
                    if (result != null)
                    {
                        return new ResourceVersion(result.CurrentVersion);
                    }
                    else
                    {
                        InsertNewVersionInfo(key);
                        return new ResourceVersion(1);
                    }
                });
            }
            return currentResourceVersion;
        }

        /// <summary>
        /// バージョン情報を取得する。
        /// </summary>
        public string GetVersionInfo(RepositoryKey repositoryKey)
        {
            var key = CreateResourceVersionKey(repositoryKey);
            var result = GetVersionInfo(key);
            if (result != null)
            {
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                return InsertNewVersionInfo(key).ToJson().ToString();
            }
        }

        public void RefreshVersion(RepositoryKey repositoryKey)
        {
            var key = CreateResourceVersionKey(repositoryKey);
            var doc = GetVersionInfo(key);
            if (doc != null && doc.CurrentVersion == 0)
            {
                doc.CurrentVersion = doc.DocumentVersions.Max(x => x.Version);
                doc.DocumentVersions.ForEach(x => x.IsCurrent = false);
                doc.DocumentVersions[doc.CurrentVersion - 1].IsCurrent = true;

                PhysicalRepository.RegisterOnce(new RegisterParam(
                    doc.ToJson(), VendorId, SystemId, OpenId, IsAutomaticId, RepositoryKey, new PartitionKey(doc.Partitionkey), null,
                    new IsOptimisticConcurrency(true), PerRequestDataContainer.Xversion == null ? null : new XVersion(PerRequestDataContainer.Xversion.Value), new IsVendor(false), new IsPerson(false),
                    new List<ResourceSharingPersonRule>(), null, null, null, null, null, null, null, null, null, null, new OperationInfo(OperationInfo.OperationType.VersionInfo)));
            }
        }

        public ResourceVersion CreateRegisterVersion(RepositoryKey repositoryKey)
        {
            var key = CreateResourceVersionKey(repositoryKey);
            var doc = GetVersionInfo(key);
            int result = 1;
            if (doc == null)
            {
                InsertNewVersionInfo(key);
            }
            else
            {
                int max = doc.DocumentVersions.Max(x => x.Version);
                if (doc.CurrentVersion == max)
                {
                    UpdateNewVersionInfo(doc, false);
                    result = doc.DocumentVersions.Max(x => x.Version);
                }
                else
                {
                    result = max;
                }
            }
            this.Cache.Remove(key.Id);
            this.Cache.Remove(key.Id + "~MaxVersion");
            currentResourceVersion = null;
            return new ResourceVersion(result);
        }

        public ResourceVersion CompleteRegisterVersion(RepositoryKey repositoryKey)
        {
            var key = CreateResourceVersionKey(repositoryKey);
            var doc = GetVersionInfo(key);
            int result = doc.CurrentVersion;
            int max = doc.DocumentVersions.Max(x => x.Version);
            if (doc.CurrentVersion != max)
            {
                result = UpdateRegisterVersionInfo(doc);
            }
            this.Cache.Remove(key.Id);
            this.Cache.Remove(key.Id + "~MaxVersion");
            currentResourceVersion = null;
            return new ResourceVersion(result);
        }


        private DocumentDbResourceVersions GetVersionInfo(ResourceVersionKey key)
        {
            var nativeQuery = new NativeQuery(PhysicalRepository.VersionInfoQuery, key.Dictionary, false);
            var queryParam = ValueObjectUtil.Create<QueryParam>(nativeQuery, new HasSingleData(true), new OperationInfo(OperationInfo.OperationType.VersionInfo));
            return PhysicalRepository.QueryOnce<DocumentDbResourceVersions>(queryParam);
        }

        private DocumentDbResourceVersions InsertNewVersionInfo(ResourceVersionKey key)
        {
            var date = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            var openid = PerRequestDataContainer.OpenId;
            var info = new DocumentDbResourceVersions()
            {
                Id = key.Id,
                Partitionkey = key.PartitionKey,
                Type = key.Type,
                RegDate = date,
                RegUserId = openid,
                UpdUserId = openid,
                UpdDate = date,
                CurrentVersion = 1,
                DocumentVersions = new List<DocumentDbResourceVersion> { new DocumentDbResourceVersion { Version = 1, RegDate = date, RegUserId = openid, IsCurrent = true } }
            };
            PhysicalRepository.RegisterOnce(new RegisterParam(info.ToJson(), VendorId, SystemId, OpenId, IsAutomaticId, RepositoryKey,
                new PartitionKey(info.Partitionkey), null, new IsOptimisticConcurrency(true), PerRequestDataContainer.Xversion == null ? null : new XVersion(PerRequestDataContainer.Xversion.Value),
                new IsVendor(false), new IsPerson(false), new List<ResourceSharingPersonRule>(), null, null, null, null, null, null, null, null, null, null, new OperationInfo(OperationInfo.OperationType.VersionInfo)));

            return info;
        }

        private void UpdateNewVersionInfo(DocumentDbResourceVersions info, bool incCurrentVersion)
        {
            var date = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            var openid = PerRequestDataContainer.OpenId;
            info.UpdDate = date;
            info.UpdUserId = openid;
            if (info.DocumentVersions == null)
            {
                info.DocumentVersions = new List<DocumentDbResourceVersion>();
            }
            info.DocumentVersions.Add(new DocumentDbResourceVersion { RegDate = date, RegUserId = openid, Version = info.DocumentVersions.Count() == 0 ? 1 : info.DocumentVersions.Max(x => x.Version) + 1, IsCurrent = false });
            if (incCurrentVersion == true)
            {
                //SetNewversionでVesionを上げる場合
                info.CurrentVersion = info.DocumentVersions.Max(x => x.Version);
                //一旦すべてのバージョンのIsCurrentをFalse変更
                info.DocumentVersions.ForEach(x => x.IsCurrent = false);
                info.CurrentVersion = info.DocumentVersions.Max(x => x.Version);
                //最新のバージョンをTrueにする
                info.DocumentVersions[info.CurrentVersion - 1].IsCurrent = true;
            }
            try
            {
                PhysicalRepository.RegisterOnce(new RegisterParam(info.ToJson(), VendorId, SystemId, OpenId, IsAutomaticId, RepositoryKey,
                    new PartitionKey(info.Partitionkey), null, new IsOptimisticConcurrency(true), PerRequestDataContainer.Xversion == null ? null : new XVersion(PerRequestDataContainer.Xversion.Value),
                    new IsVendor(false), new IsPerson(false), new List<ResourceSharingPersonRule>(), null, null, null, null, null, null, null, null, null, null, new OperationInfo(OperationInfo.OperationType.VersionInfo)));
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    throw;
                }
            }
        }

        private int UpdateRegisterVersionInfo(DocumentDbResourceVersions info)
        {
            var date = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            var openid = PerRequestDataContainer.OpenId;
            info.UpdDate = date;
            info.UpdUserId = openid;
            info.CurrentVersion = info.DocumentVersions.Max(x => x.Version);
            info.DocumentVersions.ForEach(x => x.IsCurrent = false);
            info.DocumentVersions[info.CurrentVersion - 1].IsCurrent = true;
            try
            {
                PhysicalRepository.RegisterOnce(new RegisterParam(info.ToJson(), VendorId, SystemId, OpenId, IsAutomaticId, RepositoryKey, new PartitionKey(info.Partitionkey), null,
                    new IsOptimisticConcurrency(true), PerRequestDataContainer.Xversion == null ? null : new XVersion(PerRequestDataContainer.Xversion.Value), new IsVendor(false),
                    new IsPerson(false), new List<ResourceSharingPersonRule>(), null, null, null, null, null, null, null, null, null, null, new OperationInfo(OperationInfo.OperationType.VersionInfo)));
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    throw;
                }
            }
            return info.CurrentVersion;
        }
    }
}
using AutoMapper;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    internal class DocumentVersionRepository : IDocumentVersionRepository
    {
        #region Mapper
        private static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentHistorySnapshot, DocumentDbDocumentVersionSnapshot>().ReverseMap();
                cfg.CreateMap<DocumentHistoryAttachFileMetaData, DocumentDbHistoryAttachFileMetaData>().ReverseMap();
                cfg.CreateMap<DocumentHistory, DocumentDbDocumentVersion>()
                    .ForMember(dst => dst.Snapshot, opt => {
                        opt.Condition((src) => src.Snapshot != null);
                        opt.MapFrom(src => src.Snapshot);
                    })
                    .ForMember(dst => dst.AttachFileMetaInfo, opt => opt.MapFrom(src => src.AttachFileMetaInfo))
                    .ReverseMap()
                    .ConstructUsing(s => CreateInstance(s));
                cfg.CreateMap<DocumentHistories, DocumentDbDocumentVersions>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });
        /// <summary>
        /// マッピングオブジェクト
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        private static IMapper Mapper { get => _Mapper.Value; }
        #endregion

        [Dependency]
        public IPerRequestDataContainer PerRequestDataContainer { get; set; }

        private readonly TimeSpan cacheExpireTime = TimeSpan.Parse("0:30:00");
        public INewDynamicApiDataStoreRepository PhysicalRepository { get; set; }

        public VendorId VendorId { get; set; }
        public SystemId SystemId { get; set; }
        public OpenId OpenId { get; set; }
        public RepositoryKey RepositoryKey { get; set; }
        public RepositoryKeyInfo RepositoryKeyInfo { get => PhysicalRepository.RepositoryKeyInfo; }

        private IsAutomaticId IsAutomaticId { get; } = new IsAutomaticId(false);

        public DocumentHistories GetDocumentVersion(DocumentKey key) => Mapper.Map<DocumentHistories>(Get(key));


        /// <summary>
        /// 履歴情報を保存する（新規）
        /// </summary>
        /// <param name="documentKey">ドキュメントのキー情報</param>
        /// <param name="repositoryKeyInfo">メインのリポジトリキー情報</param>
        /// <param name="isDelete">削除情報か</param>
        /// <returns>ドキュメント履歴の情報</returns>
        public DocumentHistories SaveDocumentVersion<T>(T documentKey, RepositoryKeyInfo repositoryKeyInfo, bool isDelete = false) where T : DocumentKey
            => Mapper.Map<DocumentHistories>(Save(documentKey, repositoryKeyInfo, null, isDelete));

        /// <summary>
        /// 履歴情報を保存する
        /// </summary>
        /// <param name="documentKey">ドキュメントのキー情報</param>
        /// <param name="repositoryKeyInfoMain">メインのリポジトリキー情報</param>
        /// <param name="newlatest">新しいバージョンを作る前のそれまでの最終履歴</param>
        /// <param name="isDeleteNewHistoy">削除情報か</param>
        /// <returns>ドキュメント履歴の情報</returns>
        public DocumentHistories SaveDocumentVersion<T>(T documentKey, RepositoryKeyInfo repositoryKeyInfoMain, DocumentHistory newlatest, bool isDeleteNewHistoy) where T : DocumentKey
            => Mapper.Map<DocumentHistories>(Save(documentKey, repositoryKeyInfoMain, newlatest, isDeleteNewHistoy));

        /// <summary>
        /// 履歴の過去分を削除する（最新のみ残す）
        /// </summary>
        /// <param name="documentKey"></param>
        public bool HistoryThrowAway(DocumentKey documentKey)
        {
            var ver = Get(documentKey);
            if (ver == null)
            {
                return false;
            }
            var max = ver.DocumentVersions.Select(x => x.VersionNo).Max();
            var latest = ver.DocumentVersions.Where(x => x.VersionNo == max).FirstOrDefault();
            ver.DocumentVersions.Clear();
            ver.DocumentVersions.Add(latest);
            Save(ver);
            return true;
        }


        public DocumentHistories UpdateDocumentVersion(DocumentKey documentKey, DocumentHistory newlatest) => Mapper.Map<DocumentHistories>(Update(documentKey, newlatest));

        private DocumentDbDocumentVersions Get(DocumentKey key) => PhysicalRepository.QueryOnce<DocumentDbDocumentVersions>(CreateSimpleQuery(PhysicalRepository.DocumentVersionQuery, key.Dictionary));

        private QueryParam CreateSimpleQuery(string sql, IDictionary<string, object> condition) => ValueObjectUtil.Create<QueryParam>(new NativeQuery(sql, condition, false), new HasSingleData(true), new OperationInfo(OperationInfo.OperationType.DocumentVersion));

        private string NowDateTimeString() => PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime().ToString("yyyy/M/d HH:mm:ss.fff").Replace(" ", "T") + "Z";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">ドキュメントのキー情報(Type,Partition,Idの３つ）</param>
        /// <param name="repositoryKeyInfoMain">メインのリポジトリキー情報</param>
        /// <param name="newlatest">新しいバージョンを作る前のそれまでの最終履歴</param>
        /// <param name="isDeleteNewHistoy">新しい履歴</param>
        /// <returns>ドキュメントバージョンの情報</returns>
        private DocumentDbDocumentVersions Save<T>(T key, RepositoryKeyInfo repositoryKeyInfoMain, DocumentHistory newlatest, bool isDeleteNewHistoy) where T : DocumentKey
        {
            DocumentDbDocumentVersions doc = null;
            if (newlatest != null)
            {
                doc = Get(key);
            }
            if (doc == null)
            {
                doc = new DocumentDbDocumentVersions();
                doc.RegUserId = PerRequestDataContainer.OpenId;
                doc.RegDate = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
                doc.Id = key.Id;
                doc.Type = key.Type;
                doc.Partitionkey = key.PartitionKey;
                if (typeof(T) == typeof(AttachFileDocumentKey))
                {
                    var tmpKey = key as AttachFileDocumentKey;
                    doc.DocumentIdForAttachFile = tmpKey.DocumentIdForAttachFile;
                }
            }
            doc.UpdUserId = PerRequestDataContainer.OpenId;
            doc.UpdDate = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            var latest = doc.DocumentVersions.LastOrDefault();
            if (newlatest != null && latest != null)
            {
                // これまでの最新履歴
                latest.LocationType = newlatest.LocationType.ToString();
                latest.Location = newlatest.Location;
                latest.RepositoryGroupId = newlatest.RepositoryGroupId;
                latest.PhysicalRepositoryId = newlatest.PhysicalRepositoryId;
                if (newlatest.AttachFileMetaInfo != null)
                {
                    latest.AttachFileMetaInfo = new DocumentDbHistoryAttachFileMetaData() { ContentType = newlatest.AttachFileMetaInfo.ContentType, Key = newlatest.AttachFileMetaInfo.Key };
                }

                // 新しい履歴
                DocumentHistory add = isDeleteNewHistoy == true ?
                    new DocumentHistory(Guid.NewGuid().ToString(), latest.VersionNo.Value + 1, NowDateTimeString(), OpenId?.Value, DocumentHistory.StorageLocationType.Delete, null, null) :
                    new DocumentHistory(Guid.NewGuid().ToString(), latest.VersionNo.Value + 1, NowDateTimeString(), OpenId?.Value, DocumentHistory.StorageLocationType.HighPerformance, repositoryKeyInfoMain, key.Id);
                doc.DocumentVersions.Add(Mapper.Map<DocumentDbDocumentVersion>(add));
            }
            else
            {
                var add = isDeleteNewHistoy == true ?
                    new DocumentHistory(Guid.NewGuid().ToString(), 1, NowDateTimeString(), OpenId?.Value, DocumentHistory.StorageLocationType.Delete, null, null) :
                    new DocumentHistory(Guid.NewGuid().ToString(), 1, NowDateTimeString(), OpenId?.Value, DocumentHistory.StorageLocationType.HighPerformance, repositoryKeyInfoMain, key.Id);
                doc.DocumentVersions.Add(Mapper.Map<DocumentDbDocumentVersion>(add));
            }
            Save(doc);
            return doc;
        }

        private DocumentDbDocumentVersions Update(DocumentKey key, DocumentHistory newtarget)
        {
            var doc = Get(key);
            if (doc == null)
            {
                throw new NotFoundException("update history not found");
            }
            doc.UpdUserId = PerRequestDataContainer.OpenId;
            doc.UpdDate = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            var target = doc.DocumentVersions.Where(x => x.VersionKey == newtarget.VersionKey).First();
            //該当の履歴
            target.LocationType = newtarget.LocationType.ToString();
            target.Location = newtarget.Location;
            target.RepositoryGroupId = newtarget.RepositoryGroupId;
            target.PhysicalRepositoryId = newtarget.PhysicalRepositoryId;
            target.AttachFileMetaInfo = Mapper.Map<DocumentDbHistoryAttachFileMetaData>(newtarget.AttachFileMetaInfo);
            if (newtarget.Snapshot != null)
            {
                target.Snapshot = new DocumentDbDocumentVersionSnapshot();
                target.Snapshot.CreateDate = newtarget.Snapshot.CreateDate.ToString();
                target.Snapshot.LocationType = newtarget.Snapshot.LocationType.ToString();
                target.Snapshot.RepositoryGroupId = newtarget.Snapshot.RepositoryGroupId;
                target.Snapshot.PhysicalRepositoryId = newtarget.Snapshot.PhysicalRepositoryId;
                target.Snapshot.Location = newtarget.Snapshot.Location;
            }

            Save(doc);
            return doc;
        }

        private void Save(DocumentDbDocumentVersions info)
        {
            info.UpdDate = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            info.UpdUserId = PerRequestDataContainer.OpenId;
            try
            {
                PhysicalRepository.RegisterOnce(new RegisterParam(
                    info.ToJson(), VendorId, SystemId, OpenId, IsAutomaticId, RepositoryKey, new PartitionKey(info.Partitionkey), null, new IsOptimisticConcurrency(true),
                    PerRequestDataContainer.Xversion == null ? null : new XVersion(PerRequestDataContainer.Xversion.Value), 
                    new IsVendor(false), new IsPerson(false), new List<ResourceSharingPersonRule>(), null, null, null, null, null, null, null, null, null, null,
                    new OperationInfo(OperationInfo.OperationType.DocumentVersion)));
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    // .NET:Microsoft.Azure.Documents.DocumentClientExceptionはinnerクラスのため使えない
                    // 文字列から判定する必要あり
                    if (inner.GetType().ToString() == "Microsoft.Azure.Documents.DocumentClientException")
                    {
                        {
                            throw new DynamicApiException(DynamicApiException.ErrorTypeEnum.ConflictVersionInfo);
                        }
                    }
                    throw;
                }
            }
        }


        private static DocumentHistory CreateInstance(DocumentDbDocumentVersion documentVersion)
        {
            DocumentHistorySnapshot dsnap = null;
            if (documentVersion.Snapshot != null)
            {
                dsnap = new DocumentHistorySnapshot(
                    documentVersion.Snapshot.CreateDate, 
                    (DocumentHistory.StorageLocationType)Enum.Parse(typeof(DocumentHistory.StorageLocationType), documentVersion.Snapshot.LocationType), 
                    documentVersion.Snapshot.RepositoryKeyInfo, 
                    documentVersion.Snapshot.Location);
            }
            DocumentHistoryAttachFileMetaData attachfileInfo = null;
            if (documentVersion.AttachFileMetaInfo != null)
            {
                attachfileInfo = new DocumentHistoryAttachFileMetaData(documentVersion.AttachFileMetaInfo.ContentType, documentVersion.AttachFileMetaInfo.Key);
            }
            return new DocumentHistory(
                documentVersion.VersionKey, 
                documentVersion.VersionNo.Value, 
                documentVersion.CreateDate, 
                documentVersion.OpenId, (DocumentHistory.StorageLocationType)Enum.Parse(typeof(DocumentHistory.StorageLocationType), documentVersion.LocationType), 
                documentVersion.RepositoryKeyInfo,
                documentVersion.Location, 
                dsnap, 
                attachfileInfo);
        }
    }
}
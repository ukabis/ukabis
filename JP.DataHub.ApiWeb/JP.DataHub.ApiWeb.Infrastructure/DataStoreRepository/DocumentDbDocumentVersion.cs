using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    internal class DocumentDbDocumentVersion
    {
        [DataMember(Name = "VersionKey")]
        public string VersionKey { get; set; }

        [DataMember(Name = "VersionNo")]
        public int? VersionNo { get; set; }

        [DataMember(Name = "CreateDate")]
        public string CreateDate { get; set; }

        [DataMember(Name = "OpenId")]
        public string OpenId { get; set; }

        [DataMember(Name = "LocationType")]
        public string LocationType { get; set; }

        [DataMember(Name = "RepositoryGroupId")]
        public Guid? RepositoryGroupId { get; set; }

        [DataMember(Name = "PhysicalRepositoryId")]
        public Guid? PhysicalRepositoryId { get; set; }

        [DataMember(Name = "Location")]
        public string Location { get; set; }

        [DataMember(Name = "Snapshot")]
        public DocumentDbDocumentVersionSnapshot Snapshot { get; set; } = null;

        /// <summary>
        /// 添付ファイル関連情報
        /// </summary>
        [DataMember(Name = "AttachFileMetaInfo")]
        public DocumentDbHistoryAttachFileMetaData AttachFileMetaInfo { get; set; }

        [IgnoreDataMember]
        public RepositoryKeyInfo RepositoryKeyInfo { get => new RepositoryKeyInfo(RepositoryGroupId, PhysicalRepositoryId); }
    }

    internal class DocumentDbDocumentVersionSnapshot
    {
        [DataMember(Name = "CreateDate")]
        public string CreateDate { get; set; }

        [DataMember(Name = "LocationType")]
        public string LocationType { get; set; }

        [DataMember(Name = "RepositoryGroupId")]
        public Guid? RepositoryGroupId { get; set; }

        [DataMember(Name = "PhysicalRepositoryId")]
        public Guid? PhysicalRepositoryId { get; set; }

        [DataMember(Name = "Location")]
        public string Location { get; set; }

        [IgnoreDataMember]
        public RepositoryKeyInfo RepositoryKeyInfo { get => new RepositoryKeyInfo(RepositoryGroupId, PhysicalRepositoryId); }
    }

    /// <summary>
    /// 履歴用の添付ファイル情報
    /// 履歴から添付ファイルを取得するのに最低限必要なデータだけ保持する
    /// </summary>
    internal class DocumentDbHistoryAttachFileMetaData
    {
        /// <summary>
        /// MIMEタイプ
        /// </summary>
        [DataMember(Name = "ContentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        [DataMember(Name = "Key")]
        public string Key { get; set; }
    }
}

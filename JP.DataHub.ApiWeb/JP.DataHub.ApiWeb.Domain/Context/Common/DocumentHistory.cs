using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal record DocumentHistory : IValueObject
    {
        /// <summary>
        /// ドキュメントが格納されている場所
        /// </summary>
        public enum StorageLocationType
        {
            /// <summary>
            /// 高速アクセス=主にCosmosDBとかだけど、PrimaryRepository
            /// </summary>
            HighPerformance,
            /// <summary>
            /// 低速アクセス＝Blob
            /// </summary>
            LowPerformance,
            /// <summary>
            /// 削除したという履歴
            /// </summary>
            Delete,
        }

        /// <summary>
        /// ドキュメントバージョンキー
        /// </summary>
        public string VersionKey { get; }

        /// <summary>
        /// バージョン番号（１からの通し番号）
        /// </summary>
        public int? VersionNo { get; }

        /// <summary>
        /// 履歴が作られた日付
        /// </summary>
        public string CreateDate { get; }

        /// <summary>
        /// 操作した人のOpenID
        /// </summary>
        public string OpenId { get; }

        /// <summary>
        /// 履歴ドキュメントの格納場所
        /// </summary>
        public StorageLocationType LocationType { get; }

        /// <summary>
        /// 永続化層グループID
        /// </summary>
        public Guid? RepositoryGroupId { get; }

        /// <summary>
        /// 物理の永続化層ID
        /// </summary>
        public Guid? PhysicalRepositoryId { get; }

        /// <summary>
        /// ストレージ上のパス
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// 静的データの保存場所データ
        /// </summary>
        public DocumentHistorySnapshot Snapshot { get; } = null;

        /// <summary>
        /// 添付ファイル関連情報
        /// </summary>
        public DocumentHistoryAttachFileMetaData AttachFileMetaInfo { get; }


        public DocumentHistory(string versionKey, int versionNo, DateTime createDate, string openId, StorageLocationType locationType, RepositoryKeyInfo repositoryKeyInfo, string location, DocumentHistorySnapshot snapshot = null, DocumentHistoryAttachFileMetaData attachFileInfo = null)
        {
            VersionKey = versionKey;
            VersionNo = versionNo;
            CreateDate = createDate.ToString("yyyy/M/d HH:mm:ss.fff").Replace(" ", "T") + "Z";
            OpenId = openId;
            LocationType = locationType;
            RepositoryGroupId = repositoryKeyInfo?.RepositoryGroupId;
            PhysicalRepositoryId = repositoryKeyInfo?.PhysicalRepositoryId;
            Location = location;
            Snapshot = snapshot;
            AttachFileMetaInfo = attachFileInfo;
        }

        public DocumentHistory(string versionKey, int versionNo, string createDate, string openId, StorageLocationType locationType, RepositoryKeyInfo repositoryKeyInfo, string location, DocumentHistorySnapshot snapshot = null, DocumentHistoryAttachFileMetaData attachFileInfo = null)
        {
            VersionKey = versionKey;
            VersionNo = versionNo;
            CreateDate = createDate;
            OpenId = openId;
            LocationType = locationType;
            RepositoryGroupId = repositoryKeyInfo?.RepositoryGroupId;
            PhysicalRepositoryId = repositoryKeyInfo?.PhysicalRepositoryId;
            Location = location;
            Snapshot = snapshot;
            AttachFileMetaInfo = attachFileInfo;
        }

        public static bool operator ==(DocumentHistory me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentHistory me, object other) => !me?.Equals(other) == true;
    }
}

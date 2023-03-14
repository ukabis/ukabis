using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal record DocumentHistorySnapshot : IValueObject
    {
        /// <summary>
        /// 履歴が作られた日付
        /// </summary>
        public string CreateDate { get; }

        /// <summary>
        /// 履歴ドキュメントの格納場所
        /// </summary>
        public DocumentHistory.StorageLocationType LocationType { get; }

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


        public DocumentHistorySnapshot(DateTime createDate, DocumentHistory.StorageLocationType locationType, RepositoryKeyInfo repositoryKeyInfo, string location)
        {
            CreateDate = createDate.ToString("yyyy/M/d HH:mm:ss.fff").Replace(" ", "T") + "Z";
            LocationType = locationType;
            RepositoryGroupId = repositoryKeyInfo?.RepositoryGroupId;
            PhysicalRepositoryId = repositoryKeyInfo?.PhysicalRepositoryId;
            Location = location;
        }

        public DocumentHistorySnapshot(string createDate, DocumentHistory.StorageLocationType locationType, RepositoryKeyInfo repositoryKeyInfo, string location)
        {
            CreateDate = createDate;
            LocationType = locationType;
            RepositoryGroupId = repositoryKeyInfo?.RepositoryGroupId;
            PhysicalRepositoryId = repositoryKeyInfo?.PhysicalRepositoryId;
            Location = location;
        }

        public static bool operator ==(DocumentHistorySnapshot me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentHistorySnapshot me, object other) => !me?.Equals(other) == true;
    }
}

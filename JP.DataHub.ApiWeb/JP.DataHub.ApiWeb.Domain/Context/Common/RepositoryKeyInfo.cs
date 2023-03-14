using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RepositoryKeyInfo : IValueObject
    {
        public Guid? RepositoryGroupId { get; }
        public Guid? PhysicalRepositoryId { get; }

        public RepositoryKeyInfo(Guid? repositoryGroupId, Guid? physicalRepositoryId)
        {
            RepositoryGroupId = repositoryGroupId;
            PhysicalRepositoryId = physicalRepositoryId;
        }

        public RepositoryKeyInfo(RepositoryInfo repositoryInfo)
        {
            RepositoryGroupId = repositoryInfo?.RepositoryGroupId;
            PhysicalRepositoryId = repositoryInfo?.PhysicalRepositoryInfoList?.Where(x => x.Isfull == false).FirstOrDefault()?.PhysicalRepositoryId;
        }

        public static bool operator ==(RepositoryKeyInfo me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RepositoryKeyInfo me, object other) => !me?.Equals(other) == true;
    }
}

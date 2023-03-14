using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class PhysicalRepositoryGroupService : IPhysicalRepositoryGroupService
    {
        private Lazy<IPhysicalRepositoryGroupRepository> _lazyRepository = new Lazy<IPhysicalRepositoryGroupRepository>(() => UnityCore.Resolve<IPhysicalRepositoryGroupRepository>());
        private IPhysicalRepositoryGroupRepository _physicalRepositoryGroupRepository { get => _lazyRepository.Value; }

        public List<PhysicalRepositoryResultModel> GetPhysicalRepository(Guid repositoryGroupId)
        {
            return _physicalRepositoryGroupRepository.GetPhysicalRepository(repositoryGroupId);
        }
    }
}

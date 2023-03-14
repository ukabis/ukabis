using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Service.Impl
{
    internal class ResourceGroupService : IResourceGroupService
    {
        private Lazy<IResourceGroupRepository> _lazyResourceGroupRepository = new Lazy<IResourceGroupRepository>(() => UnityCore.Resolve<IResourceGroupRepository>());
        private IResourceGroupRepository _resourceGroupRepository { get => _lazyResourceGroupRepository.Value; }

        public IList<ResourceGroupModel> GetList()
            => _resourceGroupRepository.GetList();

        public string Register(ResourceGroupModel model)
        {
            if (string.IsNullOrEmpty(model.ResourceGroupId))
            {
                model.ResourceGroupId = Guid.NewGuid().ToString();
            }
            return _resourceGroupRepository.Register(model);
        }

        public void Delete(string resource_group_id)
            => _resourceGroupRepository.Delete(resource_group_id);
    }
}

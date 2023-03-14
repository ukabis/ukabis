using JP.DataHub.ApiWeb.Domain.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    public interface IResourceGroupRepository
    {
        IList<ResourceGroupModel> GetList();
        string Register(ResourceGroupModel model);
        void Delete(string resource_group_id);
    }
}

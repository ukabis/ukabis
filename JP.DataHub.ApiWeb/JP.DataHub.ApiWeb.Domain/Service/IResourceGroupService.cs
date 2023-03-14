using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Service
{
    public interface IResourceGroupService
    {
        IList<ResourceGroupModel> GetList();
        string Register(ResourceGroupModel model);
        void Delete(string resource_group_id);
    }
}

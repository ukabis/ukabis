using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IPhysicalRepositoryGroupRepository
    {
        List<PhysicalRepositoryResultModel> GetPhysicalRepository(Guid repositoryGroupId);
    }
}

using JP.DataHub.ApiWeb.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    public interface IPhysicalRepositoryGroupRepository
    {
        List<PhysicalRepositoryResultModel> GetPhysicalRepository(string repositoryGroupId);
    }
}

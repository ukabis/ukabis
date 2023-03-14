using JP.DataHub.Batch.Revoke.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke.Api
{
    [WebApiResource("/Manage/ResourceGroup", typeof(ResourceGroupModel))]
    public interface IResourceGroupResource : ICommonResource<ResourceGroupModel>
    {
        [WebApiGet("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ResourceGroupModel>> GetList();
    }
}

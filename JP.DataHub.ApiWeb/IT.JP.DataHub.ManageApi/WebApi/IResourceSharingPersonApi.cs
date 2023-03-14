using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/ResourceSharingPerson", typeof(ResourceSharingPersonModel))]
    public interface IResourceSharingPersonApi
    {
        [WebApi("GetResourceSharingPerson?resourceSharingRuleId={resourceSharingRuleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceSharingPersonModel> GetResourceSharingPerson(string resourceSharingRuleId);

        [WebApi("GetResourceSharingListByResourcePath?resourcePath={resourcePath}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ResourceSharingPersonModel>> GetResourceSharingListByResourcePath(string resourcePath);

        [WebApiPost("RegisterResourceSharingPerson")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceSharingPersonModel> RegisterResourceSharingPerson(RegisterResourceSharingPersonModel model);

        [WebApiPost("UpdateResourceSharingPerson")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceSharingPersonModel> UpdateResourceSharingPerson(ResourceSharingPersonModel model);

        [WebApiDelete("DeleteResourceSharingPerson?resourceSharingRuleId={resourceSharingRuleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteResourceSharingPerson(string resourceSharingRuleId);
    }
}

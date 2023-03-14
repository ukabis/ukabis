using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/ResourceSharing", typeof(ResourceSharingModel))]
    public interface IResourceSharingApi
    {
        [WebApi("GetResourceSharing?resourceSharingRuleId={resourceSharingRuleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceSharingModel> GetResourceSharing(string resourceSharingRuleId);

        [WebApi("GetResourceSharingList?vendorId={vendorId}&systemId={systemId}&apiId={apiId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ResourceSharingModel>> GetResourceSharingList(string vendorId, string systemId, string apiId);

        [WebApiPost("RegisterResourceSharing")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceSharingModel> RegisterResourceSharing(ResourceSharingModel model);

        [WebApiPost("UpdateResourceSharing")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceSharingModel> UpdateResourceSharing(ResourceSharingModel model);

        [WebApiDelete("DeleteResourceSharing?resourceSharingRuleId={resourceSharingRuleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteResourceSharing(string resourceSharingRuleId);
    }
}

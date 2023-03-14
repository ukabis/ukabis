using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/ApiMailTemplate", typeof(ApiMailTemplateModel))]
    public interface IApiMailTemplateApi
    {
        [WebApi("GetApiMailTemplate?apiMailTemplateId={apiMailTemplateId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiMailTemplateModel> GetApiMailTemplate(string apiMailTemplateId);

        [WebApi("GetApiMailTemplateList?apiId={apiId}&vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiMailTemplateModel>> GetApiMailTemplateList(string apiId, string vendorId = null);

        [WebApiPost("RegisterApiMailTemplate")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiMailTemplateModel> RegisterApiMailTemplate(RegisterApiMailTemplateModel model);

        [WebApiPost("UpdateApiMailTemplate")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiMailTemplateModel> UpdateApiMailTemplate(ApiMailTemplateModel model);

        [WebApiDelete("DeleteApiMailTemplate?apiMailTemplateId={apiMailTemplateId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteApiMailTemplate(string apiMailTemplateId);
    }
}

using IT.JP.DataHub.ManageApi.WebApi.Models.ApiWebhook;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/ApiWebhook", typeof(ApiWebhookModel))]
    public interface IApiWebhookApi
    {
        [WebApi("GetApiWebhook?apiWebhookId={apiWebhookId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiWebhookModel> GetApiWebhook(string apiWebhookId);

        [WebApi("GetApiWebhookList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiWebhookModel>> GetApiWebhookList(string vendorId);

        [WebApiPost("RegisterApiWebhook")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiWebhookRegisterResponseModel> RegisterApiWebhook(ApiWebhookRegisterModel model);

        [WebApiPost("UpdateApiWebhook")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiWebhookRegisterResponseModel> UpdateApiWebhook(ApiWebhookUpdateModel model);

        [WebApiDelete("DeleteApiWebhook?apiWebhookId={apiWebhookId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteApiWebhook(string apiWebhookId);
    }
}

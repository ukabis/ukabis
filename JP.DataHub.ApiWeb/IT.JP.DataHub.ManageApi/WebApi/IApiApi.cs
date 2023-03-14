using IT.JP.DataHub.ManageApi.WebApi.Models.Api;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/Api", typeof(ApiVendorLinkModel))]
    public interface IApiApi
    {
        [WebApi("GetVendorLink")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiVendorLinkModel> GetVendorLink();

        [WebApi("GetSystemLink")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiDescriptionSystemLinkModel>> GetSystemLink();

        [WebApi("GetSchemaDescription")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SchemaDescriptionModel>> GetSchemaDescription();

        [WebApi("GetCategory")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<CategoryModel>> GetCategory();

        [WebApiPost("RegisterStaticApi")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterStaticApiResponseModel> RegisterStaticApi(RegisterStaticApiRequestModel requestModel);

        [WebApiPost("RegisterAllStaticApi")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterStaticApiResponseModel>> RegisterAllStaticApi();

        [WebApiPost("CreateStaticApiModel?controllerId={controllerId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel CreateStaticApiModel(string controllerId);
    }
}

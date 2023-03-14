using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ReferenceNotifyOData", typeof(ReferenceNotifyODataModel))]
    public interface IReferenceNotifyODataApi : ICommonResource<ReferenceNotifyODataModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(ReferenceNotifyODataModel model);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistEx(ReferenceNotifyODataModelEx model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<ReferenceNotifyODataModel> model);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> UpdateEx(string key, ReferenceNotifyODataModelEx model);
    }
}

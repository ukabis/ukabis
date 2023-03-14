using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ReferenceNotify", typeof(ReferenceNotifyModel))]
    public interface IReferenceNotifyApi : ICommonResource<ReferenceNotifyModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(ReferenceNotifyModel model);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistEx(ReferenceNotifyModelEx model);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistEx2(ReferenceNotifyModelEx2 model);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistEx3(ReferenceNotifyModelEx3 model);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistAsString(string value);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistListEx(List<ReferenceNotifyModelEx> model);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateEx(string key, ReferenceNotifyModelEx model);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateEx2(string key, ReferenceNotifyModelEx2 model);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateEx3(string key, ReferenceNotifyModelEx3 model);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateAsString(string key, string value);
    }
}

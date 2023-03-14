using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ReferenceSource", typeof(ReferenceNotifySourceModel))]
    public interface IReferenceSourceApi : ICommonResource<ReferenceNotifySourceModel>
    {
        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(string value);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateAsString(string key, string value);
    }
}

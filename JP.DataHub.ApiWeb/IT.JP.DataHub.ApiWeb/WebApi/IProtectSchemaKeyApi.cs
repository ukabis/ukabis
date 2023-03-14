using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ProtectSchemaKey", typeof(ReferenceNotifySourceModel))]
    public interface IProtectSchemaKeyApi : ICommonResource<ReferenceNotifySourceModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ReferenceNotifySourceModel>> GetAll();

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<ReferenceNotifySourceModel> model);
    }
}

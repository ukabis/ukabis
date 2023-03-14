using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/InternalApi", typeof(AreaUnitModel))]
    public interface IInternalApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApiPost("RegistInternal")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistInternal(AreaUnitModel model);

        [WebApiPost("RegistInternalKey")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistInternalKey(AreaUnitModel model);

        [WebApiPost("RegistInternalPassedRoslyn")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistInternalPassedRoslyn(AreaUnitModel model);

        [WebApiPost("RegistInternalKeyPassedRoslyn")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistInternalKeyPassedRoslyn(AreaUnitModel model);

        [WebApiPost("RegistInternalKeyFailRoslyn")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistInternalKeyFailRoslyn(AreaUnitModel model);
    }
}

using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ContainerDynamicSeparation/PersonPrivate", typeof(AreaUnitModel))]
    public interface IContainerDynamicSeparationPersonPrivateApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApi("GetAllOverPartition")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAllOverPartition();

        [WebApi("GetOverPartition/{areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetOverPartition(string areaUnitCode);

        [WebApi("GetByODataQueryOverPartition")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByODataQueryOverPartition();

        [WebApi("GetByApiQueryOverPartition")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByApiQueryOverPartition();

        [WebApi("GetByAggregateOverPartition")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByAggregateOverPartition();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModel> model);
    }
}

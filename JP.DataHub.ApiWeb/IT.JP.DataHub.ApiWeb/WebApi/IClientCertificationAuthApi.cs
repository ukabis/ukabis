using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ClientCertificationAuthTest", typeof(AreaUnitModel))]
    public interface IClientCertificationAuthApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetByClientCert")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByClientCert();

        [WebApi("GetByNormal")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByNormal();

        [WebApi("RoslynCall")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> RoslynCall();
    }
}

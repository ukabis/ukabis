using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/UserResourceShare", typeof(AreaUnitModel))]
    public interface IUserResourceShareApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("Query")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> Query();

        [WebApi("TableJoin")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> TableJoin();
    }
}

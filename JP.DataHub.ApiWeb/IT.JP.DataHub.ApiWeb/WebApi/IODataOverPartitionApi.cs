using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataOverPartition", typeof(AreaUnitModel))]
    public interface IODataOverPartitionApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("ODataOverPartition?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> ODataOverPartition(string querystring = null);
    }
}

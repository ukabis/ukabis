using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Private/TraceManage", typeof(TraceManageModel))]
    public interface ITraceManageApi : ICommonResource<TraceManageModel>
    {
        [WebApiPatch("Update/{productCode}")]
        [AutoGenerateReturnModel]
        new WebApiRequestModel<string> Update(string productCode, TraceManagePasswordModel model);
    }
}

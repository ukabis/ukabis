using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Private/Trace", typeof(TraceModel))]
    public interface ITraceApi : ICommonResource<TraceModel>
    {
        [WebApiGet("GetTraceByProductCode?ProductCode={productCode}")]
        [AutoGenerateReturnModel]
        new WebApiRequestModel<List<TraceModel>> GetTraceByProductCode(string productCode);

    }
}

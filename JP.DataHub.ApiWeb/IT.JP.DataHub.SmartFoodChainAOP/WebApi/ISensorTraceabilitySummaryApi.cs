using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Public/SensorTraceability/Summary", typeof(SensorTraceabilitySummaryModel))]
    public interface ISensorTraceabilitySummaryApi
    {
        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResultModel> Register(SensorTraceabilitySummaryModel model);
    }
}

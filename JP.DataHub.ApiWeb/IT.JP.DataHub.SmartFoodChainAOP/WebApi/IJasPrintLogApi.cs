using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/JasFreshnessManagement/V3/Private/JasPrintLog", typeof(JasPrintModel))]
    public interface IJasPrintLogApi
    {
        [WebApi("ODataCertifiedApplication?$filter=ProductCode eq '{ProductCode}' and LastGln  eq '{GlnCode}'")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<JasPrintModel>> OData(string ProductCode, string GlnCode);
    }
}

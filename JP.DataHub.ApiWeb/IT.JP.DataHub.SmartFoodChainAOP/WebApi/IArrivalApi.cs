using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Private/Arrival", typeof(ArrivalModel))]
    public interface IArrivalApi : ICommonResource<ArrivalModel>
    {
        [WebApiDelete("ODataDelete?$filter=InvoiceCode eq '{InvoiceCode}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> ODataDelete(string? InvoiceCode);

        [WebApi("OData?$filter=InvoiceCode eq '{InvoiceCode}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<ArrivalModel>> OData(string? InvoiceCode);
    }
}

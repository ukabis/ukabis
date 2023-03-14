using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/JasFreshnessManagement/V3/Public/JasPrint", typeof(JasPrintModel))]
    public interface IJasPrintApi
    {
        [WebApiPost("RegisterPrintLog")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> RegisterPrintLog(JasPrintRequestModel requestModel);
        
        [WebApiPost("RegisterRePrintLog")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> RegisterRePrintLog(JasRePrintRequestModel requestModel);
        
        [WebApi("GetPrintableCount?product={ProductCode}&gln={GlnCode}&arrivalId={ArrivalId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GetPrintableCountResultModel> GetPrintableCount(string ProductCode, string GlnCode, string ArrivalId);

        [WebApi("OData?$filter=ProductCode eq '{ProductCode}' and LastGln  eq '{GlnCode}'")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<JasPrintModel>> OData(string ProductCode, string GlnCode);

        [WebApi("Get/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<JasPrintModel> Get(string PK);

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<JasPrintModel>> GetList();

        [WebApi("Exists/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Exists(string PK);

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Register(JasPrintModel requestModel);

        [WebApiPost("RegisterList")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<string>> RegisterList(List<JasPrintModel> requestModel);

        [WebApiDelete("Delete/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Delete(string PK);
        
        [WebApiDelete("DeleteAll")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> DeleteAll();

        [WebApiPatch("Update/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Update(string PK, JasPrintModel requestModel);
    }
}

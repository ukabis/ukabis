using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/JasFreshnessManagement/V3/public/JudgmentHistory", typeof(JudgmentHistoryModel))]
    public interface IJudgmentHistoryApi 
    {
        [WebApiDelete("ODataDelete?$filter=GlnCode eq '{GlnCode}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> ODataDelete(string GlnCode);

        [WebApi("Get/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<JudgmentHistoryModel> Get(string PK);

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<JudgmentHistoryModel>> GetList();

        [WebApi("Exists/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Exists(string PK);

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Register(JudgmentHistoryModel requestModel);

        [WebApiPost("RegisterList")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<string>> RegisterList(List<JudgmentHistoryModel> requestModel);

        [WebApiDelete("Delete/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Delete(string PK);
        
        [WebApiDelete("DeleteAll")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> DeleteAll();

        [WebApiPatch("Update/{PK}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Update(string PK, JudgmentHistoryModel requestModel);
    }
}

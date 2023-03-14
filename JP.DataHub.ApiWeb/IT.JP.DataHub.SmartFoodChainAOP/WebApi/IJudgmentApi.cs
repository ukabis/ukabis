using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/SmartFoodChain/JasFreshnessManagement/V2/Public/Judgment", typeof(JasFreshnessJudgmentResultModel))]
    public interface IJudgmentApi : ICommonResource<JasFreshnessJudgmentResultModel>
    {
        [WebApi("JasFreshnessJudgment?product={product}&gln={gln}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<JasFreshnessJudgmentResultModel> JasFreshnessJudgment(string product, string gln);
    }
}

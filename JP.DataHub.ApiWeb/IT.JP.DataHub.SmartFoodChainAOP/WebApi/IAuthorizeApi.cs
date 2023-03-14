using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/ApplicationAuthorization/Public/Authorize", typeof(AuthorizeResultModel))]
    public interface IAuthorizeApi: ICommonResource<AuthorizeResultModel>
    {
       
        [WebApi("IsAuthorize?app={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<AuthorizeResultModel> IsAuthorize(string applicationId);
    }
}

using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/ApplicationAuthorization/Public/Application", typeof(ApplicationModel))]
    public interface IApplicationApi: ICommonResource<ApplicationModel>
    {
       
        [WebApi("IsManager")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<ApplicationIsManagerModel>> IsManager();
    }
}

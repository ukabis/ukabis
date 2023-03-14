using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Users", typeof(OpenIdUserModel))]
    public interface IOpenIdUserApi : ICommonResource<OpenIdUserModel>
    {
        [WebApi("GetFullAccess?userId={userId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<OpenIdUserModel> GetFullAccess(string userId);
    }
}

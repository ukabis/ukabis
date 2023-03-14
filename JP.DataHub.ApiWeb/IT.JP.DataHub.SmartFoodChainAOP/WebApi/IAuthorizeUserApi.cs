using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    // TODO interfaceを他と合わせるとVendorSystemFilterTestが通らなくなる
    [WebApiResource("/API/ApplicationAuthorization/Private/AuthorizeUser")]
    public interface IAuthorizeUserApi : IDataHubApi<AuthorizeUserModel>
    {
        [WebApi("Get/{authorizeUserId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AuthorizeUserModel> Get(string authorizeUserId, string applicationId);

        [WebApiPost("Register?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResultModel> Register(AuthorizeUserModel model, string applicationId);

        [WebApiDelete("Delete/{authorizeUserId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VoidModel> Delete(string authorizeUserId, string applicationId);
    }

    [WebApiResource("/API/ApplicationAuthorization/Private/AuthorizeUser")]
    public class AuthorizeUserApi : DataHubApi<AuthorizeUserModel>, IAuthorizeUserApi
    {
        public AuthorizeUserApi()
            : base()
        {
        }
        public AuthorizeUserApi(string serverUrl)
            : base(serverUrl)
        {
        }
        public AuthorizeUserApi(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }

        [WebApi("Get/{authorizeUserId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<AuthorizeUserModel> Get(string authorizeUserId, string applicationId) => null;

        [WebApiPost("Register?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<RegisterResultModel> Register(AuthorizeUserModel model, string applicationId) => null;

        [WebApiDelete("Delete/{authorizeUserId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<VoidModel> Delete(string authorizeUserId, string applicationId) => null;
    }
}

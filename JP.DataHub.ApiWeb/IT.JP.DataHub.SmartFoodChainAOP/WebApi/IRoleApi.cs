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
    [WebApiResource("/API/ApplicationAuthorization/Private/Role")]
    public interface IRoleApi : IDataHubApi<RoleModel>
    {
        [WebApi("Get/{privateRoleId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RoleModel> Get(string privateRoleId, string applicationId);

        [WebApiPost("Register?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResultModel> Register(RoleModel model, string applicationId);

        [WebApiDelete("Delete/{privateRoleId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VoidModel> Delete(string privateRoleId, string applicationId);
    }

    [WebApiResource("/API/ApplicationAuthorization/Private/Role")]
    public class RoleApi : DataHubApi<RoleModel>, IRoleApi
    {
        public RoleApi()
            : base()
        {
        }
        public RoleApi(string serverUrl)
            : base(serverUrl)
        {
        }
        public RoleApi(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }

        [WebApi("Get/{privateRoleId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<RoleModel> Get(string privateRoleId, string applicationId) => null;

        [WebApiPost("Register?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<RegisterResultModel> Register(RoleModel model, string applicationId) => null;

        [WebApiDelete("Delete/{privateRoleId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<VoidModel> Delete(string privateRoleId, string applicationId) => null;
    }
}

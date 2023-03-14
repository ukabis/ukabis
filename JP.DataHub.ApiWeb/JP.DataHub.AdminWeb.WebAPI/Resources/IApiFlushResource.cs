
using JP.DataHub.AdminWeb.WebAPI.Models.Api;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.AdminWeb.WebAPI.Resources
{

    [WebApiResource("/Api/Cache", typeof(ApiResourceModel))]
    public interface IApiFlushResource : IResource
    {
        [WebApiDelete("ClearStaticCache")]
        [AutoGenerateReturnModel]
        WebApiRequestModel ApiFlush();
    }

    [DomainUrl(DomainType.DynamicApi)]
    public class ApiFlushResource : Resource, IApiFlushResource
    {
        public ApiFlushResource()
        {
        }

        public ApiFlushResource(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        public ApiFlushResource(IServerEnvironment serverEnvironment)
        {
            ServerEnvironment = serverEnvironment;
            ServerUrl = GetDomainUrl();
        }

        public WebApiRequestModel ApiFlush() => MakeApiRequestModel<WebApiRequestModel>();
    }
}

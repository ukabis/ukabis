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
    [WebApiResource("/API/ApplicationAuthorization/Public/Function")]
    public interface IFunctionApi : IDataHubApi<FunctionModel>
    {
        [WebApi("Get/{functionId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<FunctionModel> Get(string functionId, string applicationId);

        [WebApiPost("Register?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResultModel> Register(FunctionModel model, string applicationId);

        [WebApiPost("RegisterList?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResultModel>> RegisterList(List<FunctionModel> model, string applicationId);

        [WebApiDelete("Delete/{functionId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VoidModel> Delete(string functionId, string applicationId);
    }

    [WebApiResource("/API/ApplicationAuthorization/Public/Function")]
    public class FunctionApi : DataHubApi<FunctionModel>, IFunctionApi
    {
        public FunctionApi()
            : base()
        {
        }
        public FunctionApi(string serverUrl)
            : base(serverUrl)
        {
        }
        public FunctionApi(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }

        [WebApi("Get/{functionId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<FunctionModel> Get(string functionId, string applicationId) => null;

        [WebApiPost("Register?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<RegisterResultModel> Register(FunctionModel model, string applicationId) => null;

        [WebApiPost("RegisterList?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<RegisterResultModel>> RegisterList(List<FunctionModel> model, string applicationId) => null;

        [WebApiDelete("Delete/{functionId}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<VoidModel> Delete(string functionId, string applicationId) => null;
    }
}

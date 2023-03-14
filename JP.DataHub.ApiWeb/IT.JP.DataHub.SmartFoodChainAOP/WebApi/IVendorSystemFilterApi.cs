using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    // TODO interfaceを他と合わせるとVendorSystemFilterTestが通らなくなる
    [WebApiResource("/API/IntegratedTest/VendorSystemFilter")]
    public interface IVendorSystemFilterApi : IDataHubApi<VendorSystemFilterModel>
    {
        [WebApi("Get/{code}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorSystemFilterModel> Get(string code, string applicationId);
    }

    [WebApiResource("/API/IntegratedTest/VendorSystemFilter")]
    public class VendorSystemFilterApi : DataHubApi<VendorSystemFilterModel>, IVendorSystemFilterApi
    {
        public VendorSystemFilterApi()
            : base()
        {
        }
        public VendorSystemFilterApi(string serverUrl)
            : base(serverUrl)
        {
        }
        public VendorSystemFilterApi(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }

        [WebApi("Get/{code}?applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<VendorSystemFilterModel> Get(string code, string applicationId) => null;
    }
}

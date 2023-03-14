using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Sensing/V3/Private/SensorDevice", typeof(SensorDeviceModel))]
    public interface ISensorDeviceApi
    {
        [WebApi("OData?$filter=id eq '{id}'")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SensorDeviceModel>> OData(string id);

        [WebApiDelete("ODataDelete?$filter={filter}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> ODataDelete(string filter);

        [WebApiPost("RegisterEx")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResultModel> RegisterEx(SensorDeviceRegisterExModel model);

        [WebApiPost("CreateThings")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SensorDeviceCreateThingsResponseModel> CreateThings(SensorDeviceCreateThingsModel model);

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResultModel> Register(SensorDeviceModel model);
        
        [WebApi("Get/{id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SensorDeviceModel>> Get(string id);
    }
}

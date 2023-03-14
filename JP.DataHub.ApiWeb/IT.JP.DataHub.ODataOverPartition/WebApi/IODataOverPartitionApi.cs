using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.ODataOverPartition.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ODataOverPartition.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataOverPartition", typeof(IntegratedTestSimpleDataModel))]
    public interface IODataOverPartitionApi: ICommonResource<IntegratedTestSimpleDataModel>
    {
        [WebApi("ODataOverPartition?$filter=AreaUnitCode eq '{code}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> ODataOverPartition(string code);
        
        [WebApi("OData?$filter=AreaUnitCode eq '{code}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> OData(string code);
        
        [WebApiDelete("ODataDelete?$filter=AreaUnitCode eq '{code}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> ODataDelete(string code);
    }
}

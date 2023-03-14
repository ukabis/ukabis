using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.ODataOverPartition.WebApi.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.ODataOverPartition.WebApi
{
    [WebApiResource("/API/SmartFoodChain/V2/Private/PartyProduct", typeof(PartyProductModel))]
    public interface IPartyProductApi : ICommonResource<PartyProductModel>
    {
        [WebApi("ODataFullAccess?$filter=GtinCode eq '{code}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<PartyProductModel>> ODataFullAccess(string code);
    }
}

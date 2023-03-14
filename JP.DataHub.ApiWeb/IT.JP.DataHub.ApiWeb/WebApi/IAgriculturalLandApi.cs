using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AgriculturalLand", typeof(AgriculturalLandModel))]
    public interface IAgriculturalLandApi : ICommonResource<AgriculturalLandModel>
    {
        [WebApi("SearchByDistance?Latitude={latitude}&Longitude={longitude}&Distance={distance}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AgriculturalLandModel> SearchByDistanceSingle(string latitude, string longitude, string distance);

        [WebApi("SearchByDistance?Latitude={latitude}&Longitude={longitude}&Distance={distance}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AgriculturalLandModel>> SearchByDistance(string latitude, string longitude, string distance);
    }
}

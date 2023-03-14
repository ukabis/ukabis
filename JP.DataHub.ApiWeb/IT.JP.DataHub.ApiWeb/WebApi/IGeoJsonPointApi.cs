using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/GeoJsonPointTest", typeof(GeoJsonPointDataModel))]
    public interface IGeoJsonPointApi : ICommonResource<GeoJsonPointDataModel>
    {
        [WebApi("Get/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GeoJsonPointModel> GetAsGeoJson(string key);

        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GeoJsonPointModel> GetAllAsGeoJson();

        [WebApi("OData")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GeoJsonPointModel> ODataAsGeoJson();
    }
}

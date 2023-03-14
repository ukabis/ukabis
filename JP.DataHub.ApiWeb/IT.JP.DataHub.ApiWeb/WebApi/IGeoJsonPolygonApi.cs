using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/GeoJsonPolygonTest", typeof(GeoJsonPolygonDataModel))]
    public interface IGeoJsonPolygonApi : ICommonResource<GeoJsonPolygonDataModel>
    {
        [WebApi("Get/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GeoJsonPolygonModel> GetAsGeoJson(string key);

        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GeoJsonPolygonModel> GetAllAsGeoJson();

        [WebApi("OData")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GeoJsonPolygonModel> ODataAsGeoJson();
    }
}

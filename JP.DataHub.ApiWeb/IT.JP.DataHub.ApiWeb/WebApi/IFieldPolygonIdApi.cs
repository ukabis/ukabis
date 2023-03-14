using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/FieldPolygonID", typeof(FieldPolygonIdModel))]
    public interface IFieldPolygonIdApi : ICommonResource<FieldPolygonIdModel>
    {
        [WebApi("Get?lat={lat}&lng={lng}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<FieldPolygonIdModel> GetByLatLang(string lat, string lng);

        [WebApi("GetArea?latl={latl}&lngl={lngl}&lath={lath}&lngh={lngh}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<FieldPolygonIdModel>> GetArea(string latl, string lngl, string lath, string lngh);

        [WebApi("GetDistance?lat={lat}&lng={lng}&Distance={distance}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<FieldPolygonIdModel>> GetDistance(string lat, string lng, string distance);
    }
}

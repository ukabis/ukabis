using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/MongoDbQuery", typeof(MongoDbQueryModel))]
    public interface IMongoDbQueryApi : ICommonResource<MongoDbQueryModel>
    {
        [WebApi("GetByAggregate")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<MongoDbQueryModel>> GetByAggregate();

        [WebApi("SearchDecimalByApiQuery?ConversionSquareMeters={conversionSquareMeters}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<MongoDbQueryModel>> SearchDecimalByApiQuery(string conversionSquareMeters);

        [WebApi("SearchDecimalByAggregate?ConversionSquareMeters={conversionSquareMeters}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<MongoDbQueryModel>> SearchDecimalByAggregate(string conversionSquareMeters);
    }
}

using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/TimeSeriesInsightsVendorPrivateTest", typeof(ObservationModel))]
    public interface ITimeSeriesInsightsVendorPrivateApi : ICommonResource<ObservationModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ObservationModel>> GetAll();

        [WebApi("GetEvents?thingId={thingId}&datastreamId={datastreamId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ObservationModel>> GetEvents(string thingId, string datastreamId);

        [WebApi("AggregateSeries?thingId={thingId}&datastreamId={datastreamId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<TsiAggregateSeriesModel>> AggregateSeries(string thingId, string datastreamId);
    }
}

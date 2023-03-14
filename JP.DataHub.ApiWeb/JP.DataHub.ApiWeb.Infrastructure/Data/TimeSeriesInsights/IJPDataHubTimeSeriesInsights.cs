using Newtonsoft.Json.Linq;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights
{
    public interface IJPDataHubTimeSeriesInsights
    {
        TimeSeriesInsightsSetting Setting { get; set; }

        JObject QueryDocument(string query, string continuationToken);
    }
}

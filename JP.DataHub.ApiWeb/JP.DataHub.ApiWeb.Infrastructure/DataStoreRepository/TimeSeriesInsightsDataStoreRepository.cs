using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights;
using JP.DataHub.ApiWeb.Infrastructure.Data.EventHub;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class TimeSeriesInsightsDataStoreRepository : NewAbstractDynamicApiDataStoreRepository
    {
        private const string QueryTypeAggregateSeries = "aggregateSeries";
        private const string QueryTypeGetEvents = "getEvents";
        private const string QueryTypeGetSeries = "getSeries";

        private const string QueryResultTimestampsPropertyName = "timestamps";
        private const string OutputTimestampPropertyName = "timestamp";

        private static ConcurrentDictionary<string, TimeSeriesInsightsSetting> s_settingCache = new ConcurrentDictionary<string, TimeSeriesInsightsSetting>();
        private static readonly string[] s_queryTypes = new string[]
        {
            QueryTypeAggregateSeries,
            QueryTypeGetEvents,
            QueryTypeGetSeries
        };


        public override bool CanOptimisticConcurrency => false;
        public override bool CanQuery => true;
        public override bool CanVersionControl => false;

#if Oracle
        private IEventHubStreamingService _eventHub = UnityCore.Resolve<IEventHubStreamingService>("LoggingStreamingService");
#else
        private IJPDataHubEventHub _eventHub = UnityCore.Resolve<IJPDataHubEventHub>();
#endif

        #region Query

        public override JsonDocument QueryOnce(QueryParam param)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<JsonDocument> QueryEnumerable(QueryParam param)
        {
            throw new NotImplementedException();
        }

        public override IList<JsonDocument> Query(QueryParam param, out XResponseContinuation xResponseContinuation)
        {
            xResponseContinuation = null;
            return Query(param.ToSingle()).Select(x => new JsonDocument(x)).ToList();
        }


        private IEnumerable<JToken> Query(QueryParam param)
        {
            (var queryType, var query) = BuildQuery(param);
            var tsi = UnityCore.Resolve<IJPDataHubTimeSeriesInsights>();
            tsi.Setting = GetTimeSeriesInsightsSetting(RepositoryInfo.ConnectionString);

            var data = new List<JToken>();
            string continuationToken = null;
            do
            {
                var result = tsi.QueryDocument(query, continuationToken);

                // 時刻別のドキュメントに形式変換
                // FROM: { "timestamps": [ "2021-11-08T03:34:00Z", "2021-11-08T03:35:00Z"], "properties": [ { "name": "hoge", "type": "Long", "values": [ 1, 2 ] }, { "name": "fuga", "type": "Long", "values": [ 1, 2 ] } ] }
                // TO  : [ { "timestamps": "2021-11-08T03:34:00Z", "hoge": 1, "fuga": 1 }, { "timestamps": "2021-11-08T03:35:00Z", "hoge": 2, "fuga": 2 } ]
                var count = result[QueryResultTimestampsPropertyName].Children().Count();
                var timestampName = (queryType != QueryTypeGetEvents) ? OutputTimestampPropertyName : (tsi.Setting.TimestampName ?? OutputTimestampPropertyName);
                for (var i = 0; i < count; i++)
                {
                    var json = new JObject();
                    json.Add(timestampName, result[QueryResultTimestampsPropertyName][i]);

                    foreach (var propety in result["properties"])
                    {
                        var name = propety["name"].Value<string>();
                        json.Add(name, propety["values"][i]);
                    }
                    data.Add(json);
                }

                continuationToken = result["continuationToken"]?.Value<string>();
            }
            while (!string.IsNullOrEmpty(continuationToken));

            return data;
        }

        #endregion

        #region Register

        /// <remarks>
        /// TimeSeriesInsightsに直接データを登録する方法はないため、
        /// イベントソースに設定したEventHub経由でデータを登録する。
        /// </remarks>
        public override RegisterOnceResult RegisterOnce(RegisterParam param)
        {
            var connectionString = GetTimeSeriesInsightsSetting(RepositoryInfo.ConnectionString);
            _eventHub.ConnectionString = connectionString.EventSource;

            var prtition = EventHubPartitionKey.CreateRegisterPartition(param.PartitionKey, param.IsVendor, param.VendorId, param.SystemId, param.Json);
            _ = _eventHub.SendMessageAsync(param.Json, prtition?.Value ?? "").Result;
            return null;
        }

        #endregion

        #region Delete

        public override void DeleteOnce(DeleteParam param)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> Delete(DeleteParam param)
        {
            throw new NotImplementedException();
        }

        #endregion


        private TimeSeriesInsightsSetting GetTimeSeriesInsightsSetting(string connectionString)
        {
            if (s_settingCache.ContainsKey(connectionString))
            {
                return s_settingCache[connectionString];
            }

            var setting = new TimeSeriesInsightsSetting(connectionString);
            s_settingCache.TryAdd(connectionString, setting);
            return setting;
        }

        private (string, string) BuildQuery(QueryParam queryParam)
        {
            var query = queryParam?.ApiQuery?.Value;
            if (string.IsNullOrWhiteSpace(queryParam?.ApiQuery?.Value))
            {
                throw new QuerySyntaxErrorException("Queries for Time series insights should have their base query set to ApiQuery.");
            }

            // パラメータ埋め込み
            if (queryParam?.QueryString != null)
            {
                foreach (var queryString in queryParam.QueryString.Dic)
                {
                    query = query.Replace($"{{{queryString.Key.Value}}}", queryString.Value.Value);
                }
            }

            // 形式チェック
            JObject jsonQuery;
            try
            {
                jsonQuery = JObject.Parse(query);
            }
            catch
            {
                throw new QuerySyntaxErrorException("Queries for Time series insights should be json format.");
            }

            // クエリ種別判定
            var queryType = s_queryTypes.FirstOrDefault(x => jsonQuery[x] != null);
            if (queryType == null)
            {
                throw new QuerySyntaxErrorException("No query type specified.");
            }

            // 依存設定
            var isResourceSharingWith =
                queryParam.XResourceSharingWith != null &&
                queryParam.ApiResourceSharing != null &&
                queryParam.ApiResourceSharing.ResourceSharingRuleList.Any();
            var isResourceSharingPerson =
                !string.IsNullOrEmpty(queryParam.XResourceSharingPerson?.Value) &&
                queryParam.ResourceSharingPersonRules.Any();

            // ベンダー依存
            if (queryParam.IsVendor?.Value == true && queryParam.IsOverPartition?.Value != true)
            {
                var vendorId = isResourceSharingWith ? queryParam.XResourceSharingWith["VendorId"] : queryParam.VendorId.Value;
                var systemId = isResourceSharingWith ? queryParam.XResourceSharingWith["SystemId"] : queryParam.SystemId.Value;

                var expression = $"$event.{JsonPropertyConst.VENDORID}.String='{vendorId}' AND $event.{JsonPropertyConst.SYSTEMID}.String='{systemId}'";
                var filterValue = jsonQuery[queryType]["filter"]?["tsx"]?.Value<string>();
                var newFilterValue = string.IsNullOrWhiteSpace(filterValue) ? expression : $"({filterValue}) AND {expression}";
                jsonQuery[queryType]["filter"] = JToken.Parse($"{{ \"tsx\": \"{newFilterValue}\" }}");
            }

            // 個人ユーザ依存
            if (queryParam.IsPerson?.Value == true && queryParam.IsOverPartition?.Value != true)
            {
                var ownerId = isResourceSharingPerson ? queryParam.XResourceSharingPerson?.Value : queryParam.OpenId.Value;

                var expression = $"$event.{JsonPropertyConst.OWNERID}.String='{ownerId}'";
                var filterValue = jsonQuery[queryType]["filter"]?["tsx"]?.Value<string>();
                var newFilterValue = string.IsNullOrWhiteSpace(filterValue) ? expression : $"({filterValue}) AND {expression}";
                jsonQuery[queryType]["filter"] = JToken.Parse($"{{ \"tsx\": \"{newFilterValue}\" }}");
            }

            return (queryType, jsonQuery.ToString());
        }
    }
}

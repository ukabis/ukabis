using System.Collections.Concurrent;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using Polly;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Infrastructure.Database.Data;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights
{
    class JPDataHubTimeSeriesInsights : IJPDataHubTimeSeriesInsights
    {
        private static readonly ConcurrentDictionary<string, string> AccessTokenCache = new ConcurrentDictionary<string, string>();

        public TimeSeriesInsightsSetting Setting { get; set; }

        private HttpClient HttpClient => _httpClient.Value;
        private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() =>
        {
            var factory = UnityCore.Resolve<IJPDataHubHttpClientFactory>();
            return factory.CreateClient();
        });


        /// <summary>
        /// Time Series Insighsへのクエリを実行する。
        /// </summary>
        public JObject QueryDocument(string query, string continuationToken)
        {
            var request = CreateRequest(query, continuationToken, true);
            var response = Policy.HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.BadRequest)
                .WaitAndRetry(Setting.MaxAttempts, i => TimeSpan.FromMilliseconds(Setting.RetryIntervalMsec), (DelegateResult<HttpResponseMessage> result, TimeSpan timeSpan) =>
                {
                    // Unauthorizedの場合はトークン再取得
                    request = CreateRequest(query, continuationToken, (result.Result?.StatusCode != HttpStatusCode.Unauthorized));
                })
                .Execute(() => HttpClient.SendAsync(request).Result);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new QuerySyntaxErrorException(response.Content.ReadAsStringAsync().Result);
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to query Time Series Insights. (StatusCode={response.StatusCode}, Message={response.Content.ReadAsStringAsync().Result})");
            }
            return JObject.Parse(response.Content.ReadAsStringAsync().Result);
        }


        private HttpRequestMessage CreateRequest(string query, string continuationToken, bool useAccessTokenCache)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Setting.ApiUrl);
            request.Headers.Add("Authorization", $"Bearer {GetAccessToken(useAccessTokenCache)}");
            if (!string.IsNullOrEmpty(continuationToken))
            {
                request.Headers.Add("x-ms-continuation", continuationToken);
            }
            request.Content = new StringContent(query, Encoding.UTF8);

            return request;
        }

        private string GetAccessToken(bool useCache)
        {
            if (useCache)
            {
                if (AccessTokenCache.TryGetValue(Setting.OriginalConnectionString, out var cachedToken))
                {
                    return cachedToken;
                }
            }

            var response = Policy.HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.Unauthorized)
                .WaitAndRetry(Setting.MaxAttempts, i => TimeSpan.FromMilliseconds(Setting.RetryIntervalMsec))
                .Execute(() => HttpClient.SendAsync(CreateAccessTokenRequest()).Result);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get access token for Time Series Insights. (StatusCode={response.StatusCode}, Message={response.Content.ReadAsStringAsync().Result})");
            }

            var token = (string)JObject.Parse(response.Content.ReadAsStringAsync().Result)["access_token"];
            AccessTokenCache.AddOrUpdate(Setting.OriginalConnectionString, token, (key, oldValue) => { return token; });
            return token;
        }

        private HttpRequestMessage CreateAccessTokenRequest()
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", Setting.ClientId },
                { "client_secret", Setting.ClientSecret },
                { "resource", "https://api.timeseries.azure.com/" },
                { "grant_type", "client_credentials" },
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Setting.LoginUrl);
            request.Content = new FormUrlEncodedContent(parameters);

            return request;
        }
    }
}

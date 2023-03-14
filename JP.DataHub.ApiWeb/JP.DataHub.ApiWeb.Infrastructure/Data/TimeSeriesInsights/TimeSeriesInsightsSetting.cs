using JP.DataHub.Com.Transaction;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights
{
    public class TimeSeriesInsightsSetting
    {
        public string OriginalConnectionString { get; }

        public string EventSource { get; set; }
        public string LoginUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TimestampName { get; set; }

        public int MaxAttempts { get; set; } = 3;
        public int RetryIntervalMsec { get; set; } = 1000;


        public TimeSeriesInsightsSetting(string connectionString)
        {
            OriginalConnectionString = connectionString;

            // TSIリポジトリの接続文字列形式
            // LoginUrl={ログインURL};ApiUrl={API URL};ClientId={クライアントID};ClientSecret={クライアントシークレット};TimestampName={タイムスタンプの項目名};Endpoint={EventHub接続文字列};EntityPath={EventHub名}
            // e.g.
            // ログインURL: https://login.microsoftonline.com/{テナントID}/oauth2/token
            // API URL    : https://{データアクセスFQDN}/timeseries/query?api-version=2020-07-31

            var parsedString = new ParseConnectionString(connectionString);
            EventSource = string.Join(";", parsedString.Parameters
                .Where(x => x.Key == "Endpoint" || x.Key == "SharedAccessKeyName" || x.Key == "SharedAccessKey" || x.Key == "EntityPath")
                .Select(x => $"{x.Key}={x.Value}"));
            LoginUrl = parsedString["LoginUrl"];
            ApiUrl = parsedString["ApiUrl"];
            ClientId = parsedString["ClientId"];
            ClientSecret = parsedString["ClientSecret"];
            TimestampName = parsedString["TimestampName"];

            if (int.TryParse(parsedString["MaxAttempts"], out var maxAttempts))
            {
                MaxAttempts = maxAttempts;
            }
            if (int.TryParse(parsedString["RetryIntervalMsec"], out var retryIntervalMsec))
            {
                RetryIntervalMsec = retryIntervalMsec;
            }
        }
    }
}

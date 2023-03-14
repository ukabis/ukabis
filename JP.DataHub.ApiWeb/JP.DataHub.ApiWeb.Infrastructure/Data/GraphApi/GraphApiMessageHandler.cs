using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.GraphApi
{
    /// <summary>
    /// GraphApiのHttpClientのメッセージハンドラー
    /// </summary>
    internal class GraphApiMessageHandler : HttpClientHandler
    {
        private Lazy<IConfiguration> _lazyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        private IConfiguration Configuration => _lazyConfiguration.Value;
        private string Tenant => Configuration.GetValue<string>("ida:GraphApiTenant") ?? Configuration.GetValue<string>("OpenId:Tenant");
        private string ClientId => Configuration.GetValue<string>("ida:GraphApiAppClientId");
        private string ClientSecret => Configuration.GetValue<string>("ida:GraphApiAppClientSecret");

        private DateTime _expiration = DateTime.MinValue;
        private AuthenticationResult _authResult;


        /// <summary>
        /// Httpリクエスト送信時にヘッダーにアクセストークンを設定します。
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>レスポンス</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // アクセストークンを取得
            if (DateTime.UtcNow > _expiration)
            {
                var app = ConfidentialClientApplicationBuilder
                    .Create(ClientId)
                    .WithAuthority(new Uri($"https://login.microsoftonline.com/{Tenant}"))
                    .WithClientSecret(ClientSecret)
                    .Build();

                // アクセストークンを取得
                _authResult = await app.AcquireTokenForClient(new[] { "https://graph.windows.net/.default" }).ExecuteAsync();
                // 有効期限を取得
                _expiration = _authResult.ExpiresOn.DateTime.AddSeconds(-120);
            }

            // ヘッダーにアクセストークンを設定
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authResult.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

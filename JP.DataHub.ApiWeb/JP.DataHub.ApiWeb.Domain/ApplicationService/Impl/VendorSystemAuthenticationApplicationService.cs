using Microsoft.IdentityModel.Tokens;
using Unity;
using JP.DataHub.Api.Core.Authentication;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    internal class VendorSystemAuthenticationApplicationService : IVendorSystemAuthenticationApplicationService
    {
        [Dependency]
        public IAccessTokenCache AccessTokenCache { get; set; }

        [Dependency]
        public IVendorSystemAccessTokenClientRepository AccessTokenClientRepository { get; set; }

        public VendorSystemAuthenticationApplicationService()
        {
        }

        public VendorSystemAuthenticationResult AuthenticateJwtToken(VendorSystemAccessToken accessToken, ClientIpaddress clientIpaddress, ApiRelativeUrl apiRelativeUrl)
        {
            VendorSystemAuthenticationResult result;
            try
            {
                // ベンダー/システム認証のアクセストークンを検証
                var principal = accessToken.Validate();
                if (!principal.Claims.Any(c => c.Type == "AccessTokenId"))
                {
                    throw new SecurityTokenException(DynamicApiMessages.VendorSystemAccessTokenIdMissing);
                }

                // アクセストークンの情報をキャッシュから取得
                var accessTokenId = principal.FindFirst("AccessTokenId").Value;
                var accessTokenData = AccessTokenCache.Get(accessTokenId);
                if (accessTokenData != null)
                {
                    // クライアントの情報を取得
                    var hit = AccessTokenClientRepository.GetByClientId(accessTokenData.ClientId.ToClientId()).FirstOrDefault();
                    if (hit == null)
                    {
                        result = new VendorSystemAuthenticationResult(ErrorCodeMessage.Code.E02405.GetRFC7807());
                    }
                    else
                    {
                        var client = Client.Create(new ClientId(hit.client_id), new ClientSecret(hit.client_secret), new SystemId(hit.system_id.ToString()),
                            new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(hit.accesstoken_expiration_timespan)), new VendorId(hit.vendor_id.ToString()));
                        result = new VendorSystemAuthenticationResult(client.VendorId, client.SystemId);
                    }
                }
                else
                {
                    result = new VendorSystemAuthenticationResult(ErrorCodeMessage.Code.E02406.GetRFC7807());
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                result = new VendorSystemAuthenticationResult(ErrorCodeMessage.Code.E02407.GetRFC7807());
                result.Detail = ex.Message;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is SecurityTokenException || ex is SecurityTokenInvalidSignatureException)
            {
                result = new VendorSystemAuthenticationResult(ErrorCodeMessage.Code.E02408.GetRFC7807());
                result.Detail = ex.Message;
            }

            return result;
        }

        public VendorSystemAuthenticateResult AuthenticateClientId(ClientId clientId, ClientSecretVO clientSecret)
        {
            var result = AccessTokenClientRepository.GetByClientId(clientId).FirstOrDefault();
            if (result == null)
            {
                return new VendorSystemAuthenticateResult(false, null, TimeSpan.Zero);
            }

            var client = Client.Create(
                    new ClientId(result.client_id),
                    new ClientSecret(result.client_secret),
                    new SystemId(result.system_id.ToString()),
                    new AccessTokenExpirationTimeSpan(TimeSpan.FromSeconds(result.accesstoken_expiration_timespan)),
                    new VendorId(result.vendor_id.ToString()));

            //認証OKかチェックをする。
            if (client.VerificationClientSecret(clientSecret))
            {
                //認証OKの場合AccesTokenオブジェクトを発行し永続化
                var accessToken = client.CreateAccessToken();
                AccessTokenCache.Store(accessToken);
                return new VendorSystemAuthenticateResult(true, accessToken.AccessTokenId, client.AccessTokenExpirationTimeSpan.Value);
            }
            else
            {
                return new VendorSystemAuthenticateResult(false, null, TimeSpan.Zero);
            }
        }

        public JwtSecurityTokenValue CreateJwtSecurityToken(Context.Common.CreateJwtSecurityTokenCommand createJwtSecurityTokenCommand)
        {
            return createJwtSecurityTokenCommand.CreateJwtSecurityToken();
        }

        public VendorSystemBaseResult GetVendorSystem(ClientId clientId, ClientSecretVO clientSecret)
        {
            var client = AccessTokenClientRepository.GetByClientId(clientId).FirstOrDefault();
            if (client != null && client.client_secret == clientSecret.Value)
            {
                // ベンダー/システムの情報を返却
                return new VendorSystemBaseResult(client.vendor_id, null, client.system_id, null);
            }

            return null;
        }
    }
}

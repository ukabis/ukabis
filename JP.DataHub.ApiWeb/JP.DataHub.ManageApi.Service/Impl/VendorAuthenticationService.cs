using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Unity;
using Microsoft.IdentityModel.Tokens;
using JP.DataHub.Com.Cryptography;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Authentication;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class VendorAuthenticationService : IVendorAuthenticationService
    {
        [Dependency]
        public IVendorAuthenticationRepository VendorAuthenticationRepository { get; set; }

        [Dependency]
        public IAccessTokenCache AccessTokenCache { get; set; }

        public AuthenticateJwtTokenResult AuthenticateJwtToken(VendorAccessToken accessToken, string clientIpaddress, string apiRelativeUrl)
        {
            try
            {
                // ベンダー/システム認証のアクセストークンを検証
                var principal = accessToken.Validate();
                if (!principal.Claims.Any(c => c.Type == "AccessTokenId"))
                {
                    throw new SecurityTokenException();
                }

                // アクセストークンの情報をキャッシュから取得
                string accessTokenId = principal.FindFirst("AccessTokenId").Value;
                AccessToken accessTokenData = AccessTokenCache.Get(accessTokenId);
                if (accessTokenData != null)
                {
                    // クライアントの情報を取得
                    var hit = VendorAuthenticationRepository.GetByClientId(accessTokenData.ClientId).FirstOrDefault();
                    if (hit == null)
                    {
                        throw new Rfc7807Exception(ErrorCodeMessage.Code.E02405.GetRFC7807(apiRelativeUrl));
                    }
                    else
                    {
                        return new AuthenticateJwtTokenResult(hit.vendor_id, hit.system_id);
                    }
                }
                else
                {
                    throw new Rfc7807Exception(ErrorCodeMessage.Code.E02406.GetRFC7807(apiRelativeUrl));
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw new Rfc7807Exception(ErrorCodeMessage.Code.E02407.GetRFC7807(apiRelativeUrl));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is SecurityTokenException || ex is SecurityTokenInvalidSignatureException)
            {
                throw new Rfc7807Exception(ErrorCodeMessage.Code.E02408.GetRFC7807(apiRelativeUrl));
            }
            return null;
        }

        public VendorAccessTokenResult AuthenticateClientId(string clientId, string clientSecret)
        {
            // clientIdに合致したものが存在するか？。存在する場合、clientSecretは合っているか？
            var cs = new ClientSecret(clientId, clientSecret);
            var clientVendorSystem = VendorAuthenticationRepository.GetByClientId(clientId).FirstOrDefault();
            if (clientVendorSystem == null || clientVendorSystem.client_secret != cs.Value)
            {
                return null;
            }
            // キャッシュに保存
            var client = new AccessTokenClient(clientVendorSystem.client_id, clientVendorSystem.client_secret, clientVendorSystem.vendor_id, clientVendorSystem.system_id, TimeSpan.FromSeconds(clientVendorSystem.accesstoken_expiration_timespan));
            if (client.VerificationClientSecret(cs) == false)
            {
                return null;
            }
            var accessToken = client.CreateAccessToken();
            AccessTokenCache.Store(accessToken);

            var notBefore = DateTime.UtcNow.AddMinutes(0);
            var exprires = notBefore.Add(accessToken.ExpirationTimeSpan);
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(accessToken.AccessTokenId))
            {
                claims.Add(new Claim("AccessTokenId", accessToken.AccessTokenId));
            }
            var key = UnityCore.Resolve<VendorAuthenticationJwtKeyConfig>();
            var token = CreateJwtSecurityToken(key.Issuer, key.Audience, claims, notBefore, exprires, key.AudienceSecret);
            return new VendorAccessTokenResult() { AccessToken = token, TokenType = "Bearer", ExpiresIn = clientVendorSystem.accesstoken_expiration_timespan };
        }

        public string CreateJwtSecurityToken(string issuer, string audience, IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null, string audienceSecret = null)
        {
            return new CreateJwtSecurityTokenCommand(issuer, audience, claims, notBefore, expires, audienceSecret).CreateJwtSecurityToken();
        }
    }
}

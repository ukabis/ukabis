using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unity;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    internal class VendorSystemAuthenticationInterface : UnityAutoInjection, IVendorSystemAuthenticationInterface, IVendorSystemAuthenticationAccessTokenInterface
    {
        [Dependency]
        public IVendorSystemAuthenticationApplicationService VendorSystemAuthentication { get; set; }

        public VendorSystemAuthenticationInterface()
        {
        }

        //public VendorSystemAuthenticateResultModel AuthenticateClientId(string clientId, string clientSecret)
        //{
        //    var ret = VendorSystemAuthenticationApplicationService.AuthenticateClientId(new ClientId(clientId), new ClientSecret(clientId, clientSecret));
        //    return new VendorSystemAuthenticateResultModel { AuthenticationSuccess = ret.AuthenticationSuccess, AccessTokenId = ret.AccessTokenId, ExpirationDate = ret.ExpirationDate };
        //}

        public VendorSystemAccessTokenModel AuthenticateClientId(string clientId, string clientSecret)
        {
            var ret = VendorSystemAuthentication.AuthenticateClientId(new ClientId(clientId), new ClientSecretVO(clientId, clientSecret));
            if (ret.AccessTokenId == null)
            {
                return null;
            }
            var notBefore = DateTime.UtcNow.AddMinutes(0);
            var exprires = notBefore.Add(ret.ExpirationDate);
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(ret.AccessTokenId))
            {
                claims.Add(new Claim("AccessTokenId", ret.AccessTokenId));
            }
            var key = UnityCore.Resolve<VendorAuthenticationJwtKeyConfig>();
            var token = CreateJwtSecurityToken(key.Issuer, key.Audience, claims, notBefore, exprires, key.AudienceSecret);
            return new VendorSystemAccessTokenModel { AccessToken = token, TokenType = "bearer", ExpiresIn = (long)ret.ExpirationDate.TotalSeconds };
        }

        public string CreateJwtSecurityToken(string issuer, string audience, IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null, string audienceSecret = null)
        {
            return VendorSystemAuthentication.CreateJwtSecurityToken(new CreateJwtSecurityTokenCommand(issuer, audience, claims, notBefore, expires, audienceSecret)).Value;
        }

        public VendorSystemAuthenticationAccessTokenResultModel AuthenticateJwtToken(string accessToken, string ipAddress, string url)
        {
            var result = VendorSystemAuthentication.AuthenticateJwtToken(new VendorSystemAccessToken(accessToken), new ClientIpaddress(ipAddress), new ApiRelativeUrl(url));
            return new VendorSystemAuthenticationAccessTokenResultModel
            {
                IsSuccess = result.IsSuccess,
                Title = result.Title,
                Detail = result.Detail,
                Status = (HttpStatusCode)result.Status,
                ErrorCode = result.ErrorCode,
                Instance = result.Instance,
                VendorId = result.VendorId,
                SystemId = result.SystemId
            };
        }

        public VendorSystemModel GetVendorSystem(string clientId, string clientSecret)
        {
            var vendorSystem = VendorSystemAuthentication.GetVendorSystem(new ClientId(clientId), new ClientSecretVO(clientId, clientSecret));
            if (vendorSystem != null)
            {
                return new VendorSystemModel { VendorId = vendorSystem.VendorId, SystemId = vendorSystem.SystemId };
            }
            return null;
        }
    }
}

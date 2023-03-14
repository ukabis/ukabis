using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        [Dependency]
        public IOpenIdAuthentication OpenIdAuthentication { get; set; } = null;

        [Dependency]
        public IVendorAuthentication VendorAuthentication { get; set; } = null;

        public AuthenticationService()
        {
            this.AutoInjection();
            if (OpenIdAuthentication == null)
            {
                OpenIdAuthentication = new OpenIdAuthentication();
            }
            if (VendorAuthentication == null)
            {
                VendorAuthentication = new VendorAuthentication();
            }
        }

        public virtual IAuthenticationResult Authentication(IServerEnvironment serverEnvironment, IAuthenticationInfo authenticationInfo)
        {
            List<IAuthenticationServerInfo> serverAuthentications = serverEnvironment.ServerAuthenticationList.Values.ToList();
            if (authenticationInfo == null)
            {
                return null;
            }
            if (authenticationInfo is AuthenticationInfo auth)
            {
                var result = new AuthenticationResult();

                // OpenId
                if (auth.OpenId != null)
                {
                    var openidServerAuthentication = serverAuthentications?.Where(x => x?.Type == AuthenticationServerType.OpenId)?.FirstOrDefault() as OpenIdAuthenticationServerInfo;
                    if (openidServerAuthentication == null || string.IsNullOrEmpty(openidServerAuthentication.Url) || string.IsNullOrEmpty(openidServerAuthentication.Scope) || string.IsNullOrEmpty(openidServerAuthentication.ClientId))
                    {
                        result.IsSuccessfull = false;
                        result.Error = "OpenId認証サーバーの設定がありません";
                        return result;
                    }
                    result.OpenId = OpenIdAuthentication.Authentication(openidServerAuthentication, auth.OpenId.Account, auth.OpenId.Password);
                    if (result.OpenId.IsSuccessStatusCode == false)
                    {
                        result.IsSuccessfull = false;
                        result.Error = "OpenId認証に失敗しました";
                        return result;
                    }
                    else
                    {
                        result.OpenIdJwt = new JP.DataHub.Com.Authentication.Jwt(result.OpenId.Result.access_token);
                    }
                    result.IsSuccessfull = true;
                }

                // Vendor
                if (auth.Vendor != null)
                {
                    var vendorServerAuthentication = serverAuthentications?.Where(x => x?.Type == AuthenticationServerType.Vendor)?.FirstOrDefault() as VendorAuthenticationServerInfo;
                    if (vendorServerAuthentication == null || string.IsNullOrEmpty(vendorServerAuthentication.Url))
                    {
                        result.IsSuccessfull = false;
                        result.Error = "ベンダー認証サーバーの設定がありません";
                        return result;
                    }
                    result.Vendor = VendorAuthentication.Authentication(vendorServerAuthentication, auth.Vendor.ClientId, auth.Vendor.ClientSecret);
                    if (result.Vendor.IsSuccessStatusCode == false)
                    {
                        result.IsSuccessfull = false;
                        result.Error = "Vendor認証に失敗しました";
                        return result;
                    }
                    result.IsSuccessfull = true;
                    result.Info = authenticationInfo;
                }
                return result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

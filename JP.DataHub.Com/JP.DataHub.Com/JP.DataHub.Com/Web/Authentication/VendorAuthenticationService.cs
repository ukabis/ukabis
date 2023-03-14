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
    class VendorAuthenticationService : UnityAutoInjection, IAuthenticationService
    {
        [Dependency]
        public IVendorAuthentication VendorAuthentication { get; set; } = null;

        [Dependency]
        public IOpenIdAuthentication OpenIdAuthentication { get; set; } = null;

        public VendorAuthenticationService()
        {
            this.AutoInjection();
            if (VendorAuthentication == null)
            {
                VendorAuthentication = new VendorAuthentication();
            }
        }

        public IAuthenticationResult Authentication(IServerEnvironment serverEnvironment, IAuthenticationInfo authenticationInfo)
        {
            var serverAuthentications = serverEnvironment.ServerAuthenticationList.Values.ToList();
            if (authenticationInfo == null)
            {
                return null;
            }
            if (authenticationInfo is VendorAuthenticationInfo auth)
            {
                var result = new AuthenticationResult();

                // Vendor
                var vendorServerAuthentication = serverAuthentications?.Where(x => x?.Type == AuthenticationServerType.Vendor)?.FirstOrDefault() as VendorAuthenticationServerInfo;
                if (vendorServerAuthentication == null || string.IsNullOrEmpty(vendorServerAuthentication.Url))
                {
                    result.IsSuccessfull = false;
                    result.Error = "ベンダー認証サーバーの設定がありません";
                    return result;
                }
                result.Vendor = VendorAuthentication.Authentication(vendorServerAuthentication, auth.Vendor.ClientId, auth.Vendor.ClientSecret, auth.Vendor.Url);
                if (result.Vendor.IsSuccessStatusCode == false)
                {
                    result.IsSuccessfull = false;
                    result.Error = "Vendor認証に失敗しました";
                    return result;
                }
                result.IsSuccessfull = true;
                result.Info = authenticationInfo;
                return result;
            }
            else if (authenticationInfo is AuthenticationInfo wauth)
            {
                var result = new AuthenticationResult();

                // Vendor
                var vendorServerAuthentication = serverAuthentications?.Where(x => x?.Type == AuthenticationServerType.Vendor)?.FirstOrDefault() as VendorAuthenticationServerInfo;
                if (vendorServerAuthentication == null || string.IsNullOrEmpty(vendorServerAuthentication.Url))
                {
                    result.IsSuccessfull = false;
                    result.Error = "ベンダー認証サーバーの設定がありません";
                    return result;
                }
                if (wauth.Vendor == null)
                {
                    throw new Exception("認証タイプがVendor認証+OpenId認証であるが、Vendor認証の情報がありません。Vendor認証+OpenId認証の場合はVendor認証、OpenId認証の情報が必要です");
                }
                result.Vendor = VendorAuthentication.Authentication(vendorServerAuthentication, wauth.Vendor.ClientId, wauth.Vendor.ClientSecret);
                if (result.Vendor.IsSuccessStatusCode == false)
                {
                    result.IsSuccessfull = false;
                    result.Error = "Vendor認証に失敗しました";
                    return result;
                }

                var openidServerAuthentication = serverAuthentications?.Where(x => x?.Type == AuthenticationServerType.OpenId)?.FirstOrDefault() as OpenIdAuthenticationServerInfo;
                if (wauth.OpenId == null)
                {
                    throw new Exception("認証タイプがVendor認証+OpenId認証であるが、OpenId認証の情報がありません。Vendor認証+OpenId認証の場合はVendor認証、OpenId認証の情報が必要です");
                }
                result.OpenId = OpenIdAuthentication.Authentication(openidServerAuthentication, wauth.OpenId.Account, wauth.OpenId.Password);
                if (result.OpenId.IsSuccessStatusCode == false)
                {
                    result.IsSuccessfull = false;
                    result.Error = "OpenId認証に失敗しました";
                    return result;
                }

                result.IsSuccessfull = true;
                result.Info = authenticationInfo;
                return result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

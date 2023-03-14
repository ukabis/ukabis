using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi
{
    public class ManageApiIntegratedTestClient : DynamicApiClient
    {
        public VendorAuthenticationServerInfo.VendorSystemInfo VendorSystemInfo { get; set; }

        public ManageApiIntegratedTestClient() : base()
        {
        }
        public ManageApiIntegratedTestClient(string nameAccount) : base(nameAccount)
        {
            var vendorAuthServerInfo = (VendorAuthenticationServerInfo)ServerEnvironment.ServerAuthenticationList[AuthenticationServerType.Vendor];
            VendorSystemInfo = vendorAuthServerInfo.VendorSystemList.FirstOrDefault(x => x.ClientId == vendorAuthServerInfo.ClientId);
        }

        public ManageApiIntegratedTestClient(string nameAccount = null, string vendorSystemAuthName = null,bool dynamicApi = false)
        {
            ServerEnvironment = UnityCore.Resolve<IServerEnvironment>();
            if(dynamicApi)
            {
                ServerEnvironment.Url = ServerEnvironment.Url2;
            }
            WebRequestManager = new WebRequestManager();

            // OpenID認証情報取得
            IAuthenticationInfo openIdAuthInfo = null;
            if (!string.IsNullOrEmpty(nameAccount))
            {
                var accountManager = UnityCore.Resolve<IAuthenticationManager>(AuthenticationManager.AccountJsonFileName.ToCI());
                openIdAuthInfo = accountManager.Find(nameAccount);
            }

            // ベンダーシステム認証情報取得
            IAuthenticationInfo vendorAuthInfo = null;
            if (!string.IsNullOrEmpty(vendorSystemAuthName))
            {
                var vendorAuthServerInfo = (VendorAuthenticationServerInfo)ServerEnvironment.ServerAuthenticationList[AuthenticationServerType.Vendor];
                VendorSystemInfo = vendorAuthServerInfo.VendorSystemList.First(x => x.Name == vendorSystemAuthName);
                vendorAuthInfo = new VendorAuthenticationInfo()
                {
                    Vendor = new VendorInfo()
                    {
                        ClientId = VendorSystemInfo.ClientId,
                        ClientSecret = VendorSystemInfo.ClientSecret,
                        Url = VendorSystemInfo.Url
                    }
                };
            }

            // 認証情報を設定
            if (openIdAuthInfo != null && vendorAuthInfo != null)
            {
                AuthenticationInfo = openIdAuthInfo.Merge(vendorAuthInfo);
            }
            else if (openIdAuthInfo != null)
            {
                AuthenticationInfo = openIdAuthInfo;
            }
            else if (vendorAuthInfo != null)
            {
                AuthenticationInfo = vendorAuthInfo;
            }
            else
            {
                AuthenticationInfo = new NothingAuthenticationInfo();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using JP.DataHub.Com.Authentication;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP
{
    public enum Accept
    {
        Json,
        Xml,
        Csv,
        GeoJson
    }


    public class IntegratedTestClient : DynamicApiClient
    {
        public VendorAuthenticationServerInfo.VendorSystemInfo VendorSystemInfo { get; set; }

        protected RetryPolicy<HttpResponseMessage> retryStatusPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return true;
                }

                var status = JsonConvert.DeserializeObject<GetStatusResponseModel>(r.Content.ReadAsStringAsync().Result);
                status.Status.IsNot("Error");
                return status.Status != "End";
            })
            .WaitAndRetry(9, i => TimeSpan.FromSeconds(20));


        public IntegratedTestClient()
            : base()
        {
        }

        public IntegratedTestClient(string nameAccount)
            : base(nameAccount)
        {
            var vendorAuthServerInfo = (VendorAuthenticationServerInfo)ServerEnvironment.ServerAuthenticationList[AuthenticationServerType.Vendor];
            VendorSystemInfo = vendorAuthServerInfo.VendorSystemList.FirstOrDefault(x => x.ClientId == vendorAuthServerInfo.ClientId);
        }

        public IntegratedTestClient(string nameAccount = null, string vendorSystemAuthName = null, string clientCertificatePath = null)
        {
            ServerEnvironment = UnityCore.Resolve<IServerEnvironment>();
            if (string.IsNullOrEmpty(clientCertificatePath))
            {
                WebRequestManager = new WebRequestManager();
            }
            else
            {
                var clientCertificate = new X509Certificate2(File.ReadAllBytes(clientCertificatePath), (string)null, X509KeyStorageFlags.MachineKeySet);
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(clientCertificate);
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;

                WebRequestManager = new WebRequestManager(clientCertificatePath, handler);
            }

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

        public string GetOpenId()
        {
            // 認証が取れていない場合は取得する
            if (AuthenticationResult == null)
            {
                var service = AuthenticationInfo?.GetAuthenticationService();
                AuthenticationResult = service?.Authentication(this.ServerEnvironment, AuthenticationInfo);
            }

            var authResults = new List<IAuthenticationResult>();
            if (AuthenticationResult is CombinationAuthenticationResult combinationResult)
            {
                authResults.AddRange(combinationResult);
            }
            else
            {
                authResults.Add(AuthenticationResult);
            }

            HttpResponseResult<TokenInfo> openIdAuthResult = null;
            foreach (var authResult in authResults.Where(x => x.IsSuccessfull))
            {
                if (authResult is AuthenticationResult aResult && aResult.OpenId != null)
                {
                    openIdAuthResult = aResult.OpenId;
                }
                else if (authResult is OpenIdAuthenticationResult openIdResult && openIdResult.OpenId != null)
                {
                    openIdAuthResult = openIdResult.OpenId;
                }
            }

            if (openIdAuthResult != null)
            {
                return openIdAuthResult.Result.access_token.ParseJwt().oid;
            }

            return null;
        }
    }
}

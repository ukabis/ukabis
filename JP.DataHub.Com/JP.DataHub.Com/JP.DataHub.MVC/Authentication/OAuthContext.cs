using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Microsoft.Identity.Web;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.MVC.Authentication
{
    public class OAuthContext : IOAuthContext
    {
        [Dependency]
        public IWebRequestManager WebRequestManager { get; set; }
        protected AzureAdB2CSettings Settings { get; set; } = UnityCore.Resolve<AzureAdB2CSettings>();

        public string OpenIdAccessToken { get; }
        public ITokenAcquisition TokenAcquisition { get; set; }

        /// <summary>
        /// OpenIDアクセストークンを取得
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public string GetOpenIdToken(string scopes = null)
        {
            var token = TokenAcquisition?.GetAccessTokenForUserAsync((scopes ?? Settings.ApiScopes)?.Split(' '))?.Result;
            return token == null ? null : $"Bearer {token}";
        }
    }
}

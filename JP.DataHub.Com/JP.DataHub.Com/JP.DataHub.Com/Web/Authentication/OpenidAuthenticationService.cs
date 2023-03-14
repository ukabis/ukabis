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
    public class OpenIdAuthenticationService : IAuthenticationService
    {
        [Dependency]
        public IOpenIdAuthentication OpenIdAuthentication { get; set; } = null;

        public OpenIdAuthenticationService()
        {
            this.AutoInjection();
            if (OpenIdAuthentication == null)
            {
                OpenIdAuthentication = new OpenIdAuthentication();
            }
        }

        public IAuthenticationResult Authentication(IServerEnvironment serverEnvironment, IAuthenticationInfo authenticationInfo)
        {
            var serverAuthentications = serverEnvironment.ServerAuthenticationList.Values.ToList();
            if (authenticationInfo == null)
            {
                return null;
            }
            if (authenticationInfo is AuthenticationInfo auth)
            {
                var result = new AuthenticationResult();

                // OpenId
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
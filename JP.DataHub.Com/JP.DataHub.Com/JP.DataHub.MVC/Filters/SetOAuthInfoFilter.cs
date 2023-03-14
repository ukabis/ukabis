using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Web;
using Unity;

namespace JP.DataHub.MVC.Filters
{
    public class SetOAuthInfoFilter : UnityAutoInjection, IAuthorizationFilter
    {
        [Dependency]
        public IOAuthContext Context;
        private readonly ITokenAcquisition _tokenAcquisition;

        /// <summary>
        /// インスタンスを初期化します
        /// </summary>
        /// <param name="tokenAcquisition"></param>
        /// <param name="configuration"></param>
        /// <param name="userContext"></param>
        public SetOAuthInfoFilter(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        /// <summary>
        /// OAuthContextのセットアップをします
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Context.TokenAcquisition = _tokenAcquisition;
        }
    }
}

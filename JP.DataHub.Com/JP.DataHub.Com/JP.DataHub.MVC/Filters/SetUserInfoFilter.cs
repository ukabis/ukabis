using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Web;
using Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Authentication;
using JP.DataHub.MVC.Session;

namespace JP.DataHub.MVC.Filters
{
    public class SetUserInfoFilter : UnityAutoInjection, IAuthorizationFilter
    {
        [Dependency]
        public IOAuthContext Context;
        private readonly ITokenAcquisition _tokenAcquisition;

        [Dependency]
        public IGroupSessionManager GroupSessionManager;

        /// <summary>
        /// インスタンスを初期化します
        /// </summary>
        /// <param name="tokenAcquisition"></param>
        /// <param name="configuration"></param>
        /// <param name="userContext"></param>
        public SetUserInfoFilter(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        /// <summary>
        /// OAuthContextのセットアップをします
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            HasGroupSession(context);
            Context.TokenAcquisition = _tokenAcquisition;
        }

        /// <summary>
        /// セッションの確認をします
        /// </summary>
        /// <param name="context"></param>
        private void HasGroupSession(AuthorizationFilterContext context)
        {
            string[] ignorePaths = { "Group", "Account", "Authorization/Get" };

            if (!GroupSessionManager.IsGroupSession() && !ignorePaths.Any(x => context.HttpContext.Request.Path.Value.Contains(x)))
            {
                // ブラウザのリロードボタンで更新されるとこれだと機能しない。
                // セッション切れメッセージ表示のために、直接Redirectにしない
                context.Result = new JsonResult(new WebApiResponseResult { StatusCode = System.Net.HttpStatusCode.Redirect, });
                //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "/" }));
            }
        }
    }
}

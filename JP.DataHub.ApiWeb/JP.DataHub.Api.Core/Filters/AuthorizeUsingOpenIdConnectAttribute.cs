using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.DataContainer;

namespace JP.DataHub.Api.Core.Filters
{

    /// <summary>
    /// configのOpenIdConnectMustBeUsedの設定がtrueの場合、OpenID認証が行われていなければ認証エラーとする。
    /// </summary>
    public class AuthorizeUsingOpenIdConnectAttribute : ActionFilterAttribute
    {
        private bool _openIdConnectMustBeUsed = true;

        public AuthorizeUsingOpenIdConnectAttribute()
        {
            _openIdConnectMustBeUsed = UnityCore.Resolve<IConfiguration>().GetValue<bool>("AppConfig:OpenIdConnectMustBeUsed", false);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_openIdConnectMustBeUsed)
            {
                return;
            }

            var dataContainer = DataContainerUtil.ResolveDataContainer() as IApiDataContainer;
            if (string.IsNullOrEmpty(dataContainer?.OpenId))
            {
                context.Result = new ObjectResult(null) { StatusCode = (int)HttpStatusCode.Unauthorized };
            }
        }
    }
}
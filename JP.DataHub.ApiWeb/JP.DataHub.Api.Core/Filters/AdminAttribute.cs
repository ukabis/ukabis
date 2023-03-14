using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Api.Core.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AdminAttribute : ActionFilterAttribute
    {
        private const string XADMIN = "X-Admin";

        private static Lazy<IConfigurationSection> s_lazyPasswordConfig = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("Password"));
        private static IConfigurationSection s_passwordConfig { get => s_lazyPasswordConfig.Value; }

        private string _passwordKey = null;


        public AdminAttribute(string passwordKey = null)
        {
            _passwordKey = passwordKey;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (IsValid(context.HttpContext.Request.Headers) == false)
            {
                context.Result = new ObjectResult(null) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
        }

        private bool IsValid(IHeaderDictionary headers)
        {
            string admin = headers.ContainsKey(XADMIN) ? headers[XADMIN].FirstOrDefault() : null;
            string password = s_passwordConfig.GetValue<string>(_passwordKey ?? "default");
            return password == admin;
        }
    }
}

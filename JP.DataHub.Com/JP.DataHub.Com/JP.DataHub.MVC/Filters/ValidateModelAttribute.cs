using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.MVC.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute, IResultFilter
    {
        private Lazy<IHttpContextAccessor> _lazyHttpContextAccessor = new Lazy<IHttpContextAccessor>(() => UnityCore.Resolve<IHttpContextAccessor>());
        private HttpContext _httpContext => _lazyHttpContextAccessor.Value?.HttpContext;

        public bool IsNullable { get; set; } = false;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                //context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
            else if (!IsNullable && context.ActionArguments.Any(x => x.Value == null))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new ObjectResult("Request parameters or body must not be null.");
            }
            //context.ActionArguments.ToList().ForEach(x =>
            //{
            //    ValidatorEx.ExceptionValidateObject(x);
            //});
        }
    }
}

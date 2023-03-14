using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.MVC.Filters
{
    public class GlobalControllerExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var url = filterContext.HttpContext.Request.GetDisplayUrl();
            var logger = new JPDataHubLogger(typeof(GlobalControllerExceptionFilter));

            Exception ex = filterContext.Exception;
            logger.Error(string.Format("Unhandled exception has occurred. URL: {0}", url), ex);

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult(ex.Message);
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}

using JP.DataHub.Com.RFC7807;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JP.DataHub.ManageApi.Filters
{
    public class RFC7807ExceptionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null && context.Exception is Rfc7807Exception rfc7807Exception)
            {
                context.Result = new ObjectResult(rfc7807Exception.Rfc7807) { StatusCode = rfc7807Exception.Rfc7807.Status };
                context.ExceptionHandled = true;
            }
        }
    }
}

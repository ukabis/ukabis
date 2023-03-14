using System;
using System.Net;
using System.Net.Http;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.RFC7807;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JP.DataHub.Com.Unity;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace JP.DataHub.MVC.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) 
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null && context.ExceptionHandled == false)
            {
                if (context.Exception is Rfc7807Exception exception)
                {
                    context.Result = new ObjectResult(new ExceptionResponseBody(exception.Rfc7807)) { StatusCode = exception.Rfc7807.Status };
                    context.ExceptionHandled = true;
                }
                else
                {
                    context.Result = new ObjectResult(new ExceptionResponseBody() { Title = "内部エラーが発生しました", Status = (int)HttpStatusCode.InternalServerError }) { StatusCode = (int)HttpStatusCode.InternalServerError };
                }
            }
        }

        public class ExceptionResponseBody
        {
            private Lazy<HttpContext> _lazyHttpContext = new Lazy<HttpContext>(() => UnityCore.Resolve<IHttpContextAccessor>().HttpContext);
            private HttpContext _httpContext { get => _lazyHttpContext.Value; }

            public int? Status { get; set; }
            [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
            public string Title { get; set; }
            [JsonProperty(PropertyName = "detail", NullValueHandling = NullValueHandling.Ignore)]
            public string Detail { get; set; }
            [JsonProperty(PropertyName = "instance", NullValueHandling = NullValueHandling.Ignore)]
            public string Instance { get; set; }
            [JsonProperty(PropertyName = "error_code", NullValueHandling = NullValueHandling.Ignore)]
            public string ErrorCode { get; set; }
            [JsonProperty(PropertyName = "errors", NullValueHandling = NullValueHandling.Ignore)]
            public IDictionary<string, string[]> Errors { get; set; }

            public ExceptionResponseBody()
            {
                Instance = GetRelativePath();
            }

            public ExceptionResponseBody(RFC7807ProblemDetail rfc = null)
            {
                if (rfc != null)
                {
                    Status = (int)rfc.Status;
                    Title = rfc.Title;
                    Detail = rfc.Detail;
                    Instance = rfc.Instance?.ToString() ?? GetRelativePath();
                    ErrorCode = rfc.ErrorCode;
                    Errors = rfc.Errors;
                }
                else
                {
                    Instance = GetRelativePath();
                }
            }

            private string GetRelativePath()
                => _httpContext.Request.Path.Value + _httpContext.Request.QueryString.Value;
        }
    }
}

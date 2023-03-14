using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Http;
using System.Security.Cryptography.X509Certificates;

namespace JP.DataHub.MVC.Filters
{
    public class AdditionalResponseHeaderAttribute : IResultFilter
    {
        private static Lazy<List<AdditionalHttpResponseHeader>> s_lasyAddResponseHeader = new Lazy<List<AdditionalHttpResponseHeader>>(() => UnityCore.Resolve<IOptions<List<AdditionalHttpResponseHeader>>>()?.Value);
        private static List<AdditionalHttpResponseHeader> s_addResponseHeader = s_lasyAddResponseHeader.Value;

        public void OnResultExecuting(ResultExecutingContext context)
        {
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            s_addResponseHeader?.ForEach(x =>
            {
                if (context.HttpContext.Response.Headers.ContainsKey(x.Name))
                {
                    context.HttpContext.Response.Headers[x.Name] = new string[] { x.Value };
                }
                else 
                {
                    context.HttpContext.Response.Headers.Add(x.Name, new string[] { x.Value });
                }
            });
        }
    }
}

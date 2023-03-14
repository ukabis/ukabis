using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.MVC.Unity
{
    public static class HttpContextAccessor
    {
        public static HttpContext Current {
            get
            {
                try
                {
                    return UnityCore.Resolve<IHttpContextAccessor>()?.HttpContext;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}

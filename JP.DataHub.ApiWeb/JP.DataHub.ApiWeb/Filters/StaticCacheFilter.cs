using System;
using Microsoft.AspNetCore.Mvc.Filters;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Interface;

namespace JP.DataHub.ApiWeb.Filters
{
    /// <summary>
    /// StaticCacheが有効な場合、条件を満たせばStaticCacheをリフレッシュする。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class StaticCacheFilter : ActionFilterAttribute
    {
        protected ICacheInterface CacheInterface => _lazyCacheInterface.Value;
        protected Lazy<ICacheInterface> _lazyCacheInterface = new Lazy<ICacheInterface>(() => UnityCore.Resolve<ICacheInterface>());


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (CacheInterface.IsStaticCache)
            {
                CacheInterface.CheckStaticCacheTime();
            }
        }
    }
}
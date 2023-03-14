using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.MVC.Filters
{
    public class DisableCacheFilter : ActionFilterAttribute, IResultFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.ContainsKey("X-Cache"))
            {
                var cacheConfig = UnityCore.Resolve<IConfiguration>().GetSection(CacheConstValue.ConfigSection);
                var defaultSectionName = cacheConfig.GetSection(CacheConstValue.ConfigDefaultSection).Get<string>();

                var defaultSection = cacheConfig.GetSection(defaultSectionName);
                if (!defaultSection.GetValue<bool>(CacheConstValue.ConfigImmutable, false))
                {
                    UnityCore.Resolve<ICache>().IsFlash = true;
                }

                foreach (var section in cacheConfig.GetChildren().Where(x => x.Key != CacheConstValue.ConfigDefaultSection))
                {
                    if (UnityCore.UnityContainer.IsRegistered(typeof(ICache), section.Key) &&
                        !section.GetValue<bool>(CacheConstValue.ConfigImmutable, false))
                    {
                        UnityCore.Resolve<ICache>(section.Key).IsFlash = true;
                    }
                }
            }
        }
    }
}

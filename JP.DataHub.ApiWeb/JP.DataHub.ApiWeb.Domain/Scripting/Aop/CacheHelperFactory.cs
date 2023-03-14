using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;
using JP.DataHub.Aop;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Aop
{
    // .NET6
    internal class CacheHelperFactory : ICacheHelperFactory
    {
        public IAopCacheHelper Create(string type)
            => UnityCore.Resolve<IAopCacheHelper>(new ParameterOverride("keyPrefix", type.ToConstructorInjection()));
    }
}

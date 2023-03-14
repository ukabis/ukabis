using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Net.Http;
using Unity.Interception.PolicyInjection.Pipeline;

namespace JP.DataHub.Service.Core.Impl
{
    public class AdaptResourceSchemaCacheKey : AbstractTypeCacheAttribute
    {
        private string keyName;

        public override void Parse(MethodInfo method, object target, IParameterCollection arguments, List<object> inputs)
        {
            var hit = method.GetGenericArguments().Where(x => x.GetInterfaces().Contains(typeof(IResource))).FirstOrDefault();
            keyName = hit == null ? null : $"AdaptResourceSchema-{hit.Name}";
        }

        public override string Rebuild()
        {
            return keyName;
        }
    }
}

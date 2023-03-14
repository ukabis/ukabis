using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity.Interception.PolicyInjection.Pipeline;

namespace JP.DataHub.Com.Cache
{
    public abstract class AbstractTypeCacheAttribute : ITypeCacheAttribute
    {
        public string Prefix { get; set; }
        public Dictionary<string, string> Param { get; set; } = new();
        public Dictionary<string, string> Header { get; set; } = new();
        public Dictionary<string, string> DataContainer { get; set; } = new Dictionary<string, string>();

        public abstract void Parse(MethodInfo method, object target, IParameterCollection arguments, List<object> inputs);
        public abstract string Rebuild();
    }
}

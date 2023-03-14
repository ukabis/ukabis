using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity.Interception.PolicyInjection.Pipeline;

namespace JP.DataHub.Com.Cache
{
    public interface ITypeCacheAttribute
    {
        string Prefix { get; set; }
        Dictionary<string, string> Param { get; set; }
        Dictionary<string, string> Header { get; set; }
        Dictionary<string, string> DataContainer { get; set; }

        void Parse(MethodInfo method, object target, IParameterCollection arguments, List<object> inputs);
        string Rebuild();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace JP.DataHub.Com.Cache.Attributes
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class CacheRequestHeaderAttribute : Attribute
    {
        public IReadOnlyCollection<string> RequestHeaderName { get; private set; }
        private List<string> list = new List<string>();

        public CacheRequestHeaderAttribute(params string[] headerNames)
        {
            if (headerNames != null)
            {
                list = new List<string>(headerNames);
                RequestHeaderName = list;
            }
        }
    }
}

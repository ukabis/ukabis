using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    public enum DomainType
    {
        DynamicApi,
        ManageApi,
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DomainUrlAttribute : Attribute
    {
        public string Name { get; set; }

        public DomainUrlAttribute()
        {
            Name = "Url";
        }

        public DomainUrlAttribute(DomainType domain)
        {
            Name = domain.ToString() + "Url";
        }

        public DomainUrlAttribute(string name)
        {
            Name = name;
        }
    }
}

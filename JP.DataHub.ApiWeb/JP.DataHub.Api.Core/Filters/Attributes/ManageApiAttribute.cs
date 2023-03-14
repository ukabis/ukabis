using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Filters.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ManageApiAttribute : Attribute
    {
        public string Id { get; set; }

        public ManageApiAttribute(string id = null)
        {
            Id = id;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Filters.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ManageActionAttribute : Attribute
    {
        public string Id { get; set; }

        public ManageActionAttribute(string id = null)
        {
            Id = id;
        }
    }
}

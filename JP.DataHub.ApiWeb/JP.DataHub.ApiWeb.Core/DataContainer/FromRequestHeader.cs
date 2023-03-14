using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Core.DataContainer
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FromRequestHeaderAttribute : Attribute
    {
        public FromRequestHeaderAttribute(string headerName)
        {
            HeaderName = headerName;
        }

        public string HeaderName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    public class WebApiResourceAttribute : Attribute
    {
        public string ResourceName { get; private set; }
        public Type Model { get; private set; }

        public WebApiResourceAttribute(string resourceName = null, Type model = null)
        {
            ResourceName = resourceName;
            Model = model;
        }
    }
}

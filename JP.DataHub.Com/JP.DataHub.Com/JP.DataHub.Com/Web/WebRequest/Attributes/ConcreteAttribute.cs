using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.WebRequest.Attributes
{
    public class ConcreteAttribute : Attribute
    {
        public Type Type { get; private set; }

        public ConcreteAttribute(Type type)
        {
            Type = type;
        }
    }
}

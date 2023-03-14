using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    public class RequestInterceptAttribute : Attribute
    {
        public RequestInterceptAttribute(Type type)
        {
        }
    }
}

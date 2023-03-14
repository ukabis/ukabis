using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace JP.DataHub.Com.Resources
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class MessageDefinitionAttribute : Attribute
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        public MessageDefinitionAttribute(string msg, string detail, HttpStatusCode httpstatuscode)
        {
            Message = msg;
            Detail = detail;
            HttpStatusCode = httpstatuscode;
        }
    }
}

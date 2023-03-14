using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    public class WebApiAttribute : Attribute
    {
        public string ActionName { get; protected set; }

        public HttpMethod HttpMethod { get; protected set; }

        public bool IsNoUrlEncoding { get; set; }

        public WebApiAttribute(bool isNoUrlEncoding = false)
        {
            HttpMethod = HttpMethod.Get;
            IsNoUrlEncoding = isNoUrlEncoding;
        }

        public WebApiAttribute(string actionName, bool isNoUrlEncoding = false)
            : this(isNoUrlEncoding)
        {
            ActionName = actionName;
        }
    }
}

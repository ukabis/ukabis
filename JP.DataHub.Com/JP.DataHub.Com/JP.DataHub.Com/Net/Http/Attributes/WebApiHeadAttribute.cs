using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    public class WebApiHeadAttribute : WebApiAttribute
    {
        public WebApiHeadAttribute(bool isNoUrlEncoding = false)
            : base(isNoUrlEncoding)
        {
            HttpMethod = HttpMethod.Head;
        }

        public WebApiHeadAttribute(string actionName, bool isNoUrlEncoding = false)
            : this(isNoUrlEncoding)
        {
            ActionName = actionName;
        }
    }
}

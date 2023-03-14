using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    public class WebApiOptionsAttribute : WebApiAttribute
    {
        public WebApiOptionsAttribute(bool isNoUrlEncoding = false)
            : base(isNoUrlEncoding)
        {
            HttpMethod = HttpMethod.Options;
        }

        public WebApiOptionsAttribute(string actionName, bool isNoUrlEncoding = false)
            : this(isNoUrlEncoding)
        {
            ActionName = actionName;
        }
    }
}

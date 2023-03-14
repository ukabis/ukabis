using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    public class WebApiPatchAttribute : WebApiAttribute
    {
        public WebApiPatchAttribute(bool isNoUrlEncoding = false)
            : base(isNoUrlEncoding)
        {
            HttpMethod = new System.Net.Http.HttpMethod("Patch");
        }

        public WebApiPatchAttribute(string actionName, bool isNoUrlEncoding = false)
            : this(isNoUrlEncoding)
        {
            ActionName = actionName;
        }
    }
}

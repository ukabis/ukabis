using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public class NothingAuthenticationResult : AbstractAuthenticationResult
    {
        public override void AddHeader(HttpRequestHeaders headers)
        {
        }
    }
}

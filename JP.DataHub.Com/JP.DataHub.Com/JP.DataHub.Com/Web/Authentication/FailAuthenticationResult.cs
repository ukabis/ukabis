using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public class FailAuthenticationResult : AbstractAuthenticationResult
    {
        public FailAuthenticationResult()
        {
            IsSuccessfull = false;
        }

        public override void AddHeader(HttpRequestHeaders headers)
        {
        }
    }
}

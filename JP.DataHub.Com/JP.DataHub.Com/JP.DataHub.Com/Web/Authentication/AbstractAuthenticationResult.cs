using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public abstract class AbstractAuthenticationResult : IAuthenticationResult
    {
        public IAuthenticationInfo Info { get; set; }

        public bool IsSuccessfull { get; set; }
        public string Error { get; set; }

        public override string ToString() => null;

        public abstract void AddHeader(HttpRequestHeaders headers);
    }
}

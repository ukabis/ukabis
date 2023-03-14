using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public class CombinationAuthenticationResult : List<IAuthenticationResult>, IAuthenticationResult
    {
        public IAuthenticationInfo Info { get; set; }
        public bool IsSuccessfull { get; }
        public string Error { get; }
        public void AddHeader(HttpRequestHeaders headers)
        {
            this.ForEach(x => x.AddHeader(headers));
        }
    }
}

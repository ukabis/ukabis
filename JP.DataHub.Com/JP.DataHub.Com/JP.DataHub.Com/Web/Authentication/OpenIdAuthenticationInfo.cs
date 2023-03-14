using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public class OpenIdAuthenticationInfo : AbstractAuthenticationInfo
    {
        public OpenIdAuthenticationInfo()
        {
            Type = typeof(OpenIdAuthenticationInfo).Name;
        }

        public OpenIdInfo OpenId { get; set; }

        public override void Setup(IServerEnvironment serverEnvironment)
        {
            throw new NotImplementedException();
        }
    }
}

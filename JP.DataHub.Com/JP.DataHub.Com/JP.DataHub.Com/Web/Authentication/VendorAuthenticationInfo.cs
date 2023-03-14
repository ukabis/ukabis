using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public class VendorAuthenticationInfo : AbstractAuthenticationInfo
    {
        public VendorAuthenticationInfo()
        {
            Type = typeof(VendorAuthenticationInfo).Name;
        }

        public VendorInfo Vendor { get; set; }

        public override void Setup(IServerEnvironment serverEnvironment)
        {
            var hit = serverEnvironment.ServerAuthenticationList.Where(x => x.Key == AuthenticationServerType.Vendor).Select(x => x.Value).FirstOrDefault();
            if (hit is VendorAuthenticationServerInfo info)
            {
                Vendor = new VendorInfo() { ClientId = info.ClientId, ClientSecret = info.ClientSecret };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public enum AuthenticationType
    {
        openidandvendor,  // OpenId認証とVendor認証を実施する
        openid, // OpenId認証のみを実施する
        vendor, // Vendor認証のみを実施する
        nothing,
        combination,
    }
}

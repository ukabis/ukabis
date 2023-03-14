using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    public interface IVendorAuthenticationService
    {
        AuthenticateJwtTokenResult AuthenticateJwtToken(VendorAccessToken accessToken, string clientIpaddress, string apiRelativeUrl);
        VendorAccessTokenResult AuthenticateClientId(string clientId, string clientSecret);
    }
}

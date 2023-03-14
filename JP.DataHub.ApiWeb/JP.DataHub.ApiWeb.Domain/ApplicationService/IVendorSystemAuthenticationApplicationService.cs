using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Com.Validations.Attributes;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    [Log]
    [TransactionScope]
    [ArgumentValidator]
    public interface IVendorSystemAuthenticationApplicationService
    {
        VendorSystemAuthenticationResult AuthenticateJwtToken(VendorSystemAccessToken accessToken, ClientIpaddress clientIpaddress, ApiRelativeUrl apiRelativeUrl);

        VendorSystemAuthenticateResult AuthenticateClientId(ClientId clientId, ClientSecretVO clientSecret);

        JwtSecurityTokenValue CreateJwtSecurityToken(CreateJwtSecurityTokenCommand createJwtSecurityTokenCommand);

        VendorSystemBaseResult GetVendorSystem(ClientId clientId, ClientSecretVO clientSecret);
    }
}

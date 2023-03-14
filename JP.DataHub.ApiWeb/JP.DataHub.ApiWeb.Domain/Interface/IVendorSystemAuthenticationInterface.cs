using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    public interface IVendorSystemAuthenticationInterface
    {
        string CreateJwtSecurityToken(string issuer, string audience, IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null, string audienceSecret = null);

        VendorSystemAccessTokenModel AuthenticateClientId(string clientId, string clientSecret);
    }
}

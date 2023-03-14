using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    public interface IVendorSystemAuthenticationAccessTokenInterface
    {
        VendorSystemAuthenticationAccessTokenResultModel AuthenticateJwtToken(string accessToken, string ipAddress, string url);
        VendorSystemModel GetVendorSystem(string clientId, string clientSecret);
    }
}

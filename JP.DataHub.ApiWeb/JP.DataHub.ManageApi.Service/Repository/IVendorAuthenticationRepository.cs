using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Repository.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IVendorAuthenticationRepository
    {
        IEnumerable<ClientVendorSystem> GetByClientId(string clientId);
    }
}

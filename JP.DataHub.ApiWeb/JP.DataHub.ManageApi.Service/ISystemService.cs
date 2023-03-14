using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Attributes;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    internal interface ISystemService
    {
        SystemModel GetSystem(string systemId, SystemItem getItem);
        IList<SystemModel> GetSystemListByVendorId(string vendorId);
        SystemModel RegisterSystem(SystemModel model, SystemItem getItem);
        SystemModel UpdateSystem(SystemModel model, SystemItem getItem);
        void DeleteSystem(string systemId);

        SystemAdminModel GetAdminBySystemId(string systemId);
        SystemAdminModel RegisterAdmin(SystemAdminModel model);
        void DeleteAdminBySystemId(string systemId);

        IList<SystemLinkModel> GetSystemLinkListBySystemId(string systemId);
        SystemLinkModel GetSystemLink(string systemLinkId);
        IList<SystemLinkModel> UpsertSystemLink(IList<SystemLinkModel> model);
        void DeleteSystemLink(string systemLinkId);

        IList<ClientModel> GetClientListBySystemId(string systemId);
        ClientModel GetClient(string clientId);
        ClientModel RegisterClient(RegisterClientModel model);
        ClientModel UpdateClient(UpdateClientModel model);
        void DeleteClient(string clientId);
    }
}

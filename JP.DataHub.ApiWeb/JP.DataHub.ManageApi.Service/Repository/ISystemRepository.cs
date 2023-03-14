using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface ISystemRepository
    {
        SystemModel GetSystem(string systemId);
        bool IsSystemExists(string systemId);

        SystemModel RegisterSystem(SystemModel model);
        SystemModel UpdateSystem(SystemModel model);
        void DeleteSystem(string systemId);
        IList<SystemModel> GetSystemListByVendorId(string vendorId);

        IList<FunctionNodeModel> GetFunctionBySystemId(string systemId);
        IList<string> RegisterFunction(string systemId, IList<FunctionNodeModel> list);

        IList<ClientModel> GetClientBySystemId(string systemId);

        IList<SystemLinkModel> GetLinkBySystemId(string systemId);
        IList<string> RegisterLink(string systemId, IList<SystemLinkModel> list);

        SystemAdminModel GetAdminBySystemId(string systemId);
        SystemAdminModel RegisterAdmin(SystemAdminModel model);
        IList<string> DeleteAdminBySystemId(string systemId);

        IList<SystemLinkModel> GetSystemLinkListBySystemId(string systemId);
        SystemLinkModel GetSystemLink(string systemLinkId);
        IList<SystemLinkModel> UpsertSystemLink(IList<SystemLinkModel> model);
        void DeleteSystemLink(string systemLinkId);

        IList<ClientModel> GetClientListBySystemId(string systemId);
        IList<ClientModel> GetClientListBySystemIds(IList<string> systemIds);
        ClientModel GetClient(string clientId);
        ClientModel RegisterClient(UpdateClientModel model);
        ClientModel UpdateClient(UpdateClientModel model);
        void DeleteClient(string clientId);
    }
}

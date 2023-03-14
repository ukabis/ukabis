using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.System;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/System", typeof(SystemModel))]
    public interface ISystemApi
    {
        [WebApi("GetSystem?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> GetSystem(string systemId, bool includeClient = false, bool includeLink = false, bool includeAdmin = false);

        [WebApi("GetList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SystemModel>> GetList(string vendorId);

        [WebApiPost("RegisterSystem")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterSystemResultModel> RegisterSystem(RegisterSystemModel model);

        [WebApiPost("UpdateSystem")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<UpdateSystemModel> UpdateSystem(UpdateSystemModel model);

        [WebApiDelete("DeleteSystem?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteSystem(string systemId);


        [WebApiDelete("DeleteSystemAdmin?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteSystemAdmin(string systemId);

        [WebApiPost("RegisterSystemAdmin")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SystemAdminModel>> RegisterSystemAdmin(SystemAdminModel model);

        [WebApi("GetSystemAdmin?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemAdminModel> GetSystemAdmin(string systemId);

        [WebApiPost("RegisterSystemLink")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SystemLinkModel>> RegisterSystemLink(List<SystemLinkModel> model);

        [WebApiDelete("DeleteSystemLink?systemLinkId={systemLinkId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteSystemLink(string systemLinkId);

        [WebApi("GetSystemLinkList?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SystemLinkModel>> GetSystemLinkList(string systemId);

        [WebApi("GetSystemLink?systemLinkId={systemLinkId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemLinkModel> GetSystemLink(string systemLinkId);

        [WebApiPost("RegisterClient")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ClientModel> RegisterClient(RegisterClientModel model);

        [WebApiPost("UpdateClient")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ClientModel> UpdateClient(UpdateClientModel model);

        [WebApiDelete("DeleteClient?clientId={clientId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteClient(string clientId);

        [WebApi("GetClientList?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ClientModel>> GetClientList(string systemId);

        [WebApi("GetClient?clientId={clientId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ClientModel> GetClient(string clientId);
    }
}

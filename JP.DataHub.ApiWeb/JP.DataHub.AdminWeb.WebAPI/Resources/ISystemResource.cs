using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Resources
{
    [WebApiResource("/Manage/System", typeof(SystemModel))]
    public interface ISystemResource : ICommonResource<SystemModel>
    {
        // 取得
        [WebApiGet("GetSystem?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> GetSystem(Guid systemId);

        [WebApiGet("GetClientList?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ClientModel>> GetClientList(Guid systemId);

        [WebApiGet("GetSystemLinkList?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SystemLinkModel>> GetSystemLinkList(Guid systemId);

        [WebApiGet("GetSystemAdmin?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemAdminModel> GetSystemAdmin(Guid systemId);

        // 登録・更新
        [WebApiPost("RegisterSystem")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> RegisterSystem(SystemModel model);

        [WebApiPost("UpdateSystem")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> UpdateSystem(SystemModel model);

        [WebApiPost("UpdateClient")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ClientModel> UpdateClient(ClientModel model);

        [WebApiPost("RegisterClient")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ClientModel> RegisterClient(ClientModel model);

        [WebApiPost("RegisterSystemLink")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SystemLinkModel>> RegisterSystemLink(List<SystemLinkModel> model);

        [WebApiPost("RegisterSystemAdmin")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemAdminModel> RegisterSystemAdmin(SystemAdminModel model);

        // 削除
        [WebApiDelete("DeleteSystem?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> DeleteSystem(string systemId);

        [WebApiDelete("DeleteClient?clientId={clientId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> DeleteClient(string clientId);

        [WebApiDelete("DeleteSystemLink?systemLinkId={systemLinkId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> DeleteSystemLink(string systemLinkId);

        [WebApiDelete("DeleteSystemAdmin?systemId={systemId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SystemModel> DeleteSystemAdmin(string systemId);
    }
}

using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/Role", typeof(RoleModel))]
    public interface IRoleApi : IAttachFileResource
    {
        [WebApi("GetRole?roleId={roleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RoleModel> GetRole(string roleId);

        [WebApi("GetRoleList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RoleModel>> GetRoleList();

        [WebApiPost("RegisterRole")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RoleModel> RegisterRole(RoleModel model);

        [WebApiPost("UpdateRole")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RoleModel> UpdateRole(RoleModel model);

        [WebApiDelete("DeleteRole?roleId={roleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteRole(string roleId);
    }
}

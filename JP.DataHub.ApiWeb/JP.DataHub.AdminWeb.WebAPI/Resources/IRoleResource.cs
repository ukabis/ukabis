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
    [WebApiResource("/Manage/Role", typeof(RoleModel))]
    public interface IRoleResource : ICommonResource<RoleModel>
    {
        [WebApiGet("GetRoleList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RoleModel>> GetRoleList();
    }
}

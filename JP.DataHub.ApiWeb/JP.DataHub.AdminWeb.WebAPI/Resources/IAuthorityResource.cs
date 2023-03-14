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
    [WebApiResource("/Manage/Authority", typeof(RoleDetailModel))]
    public interface IAuthorityResource : ICommonResource<RoleDetailModel>
    {
        [WebApiGet("GetRoleDetail")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RoleDetailModel>> GetRoleDetail();

        [WebApiGet("GetRoleDetailEx?openId={openId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RoleDetailModel>> GetRoleDetailEx(string openId);

        [WebApiGet("IsOperatingVendor?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<bool> IsOperatingVendor(string vendorId);
    }
}

using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/AdminInfo", typeof(AdminInfoModel))]
    public interface IAdminInfoApi
    {
        [WebApi("GetAdminInfo?adminFuncRoleId={adminFuncRoleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AdminInfoModel> GetAdminInfo(string adminFuncRoleId);

        [WebApi("GetAdminInfoList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AdminFuncRoleInfomationModel>> GetAdminInfoList();

        [WebApiPost("RegisterAdminInfo")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AdminInfoModel> RegisterAdminInfo(AdminInfoModel model);

        [WebApiPost("UpdateAdminInfo")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AdminInfoModel> UpdateAdminInfo(AdminInfoModel model);

        [WebApiDelete("DeleteAdminInfo?adminFuncRoleId={adminFuncRoleId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteAdminInfo(string adminFuncRoleId);
    }
}

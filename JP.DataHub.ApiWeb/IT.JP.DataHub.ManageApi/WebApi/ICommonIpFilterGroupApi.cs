using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/CommonIpFilterGroup", typeof(CommonIpFilterGroupModel))]
    public interface ICommonIpFilterGroupApi
    {
        [WebApi("GetCommonIpFilterGroup?commonIpFilterGroupId={commonIpFilterGroupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CommonIpFilterGroupModel> GetCommonIpFilterGroup(string commonIpFilterGroupId);

        [WebApi("GetCommonIpFilterGroupList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<CommonIpFilterGroupModel>> GetCommonIpFilterGroupList();

        [WebApiPost("RegisterCommonIpFilterGroup")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CommonIpFilterGroupModel> RegisterCommonIpFilterGroup(CommonIpFilterGroupModel model);

        [WebApiPost("UpdateCommonIpFilterGroup")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CommonIpFilterGroupModel> UpdateCommonIpFilterGroup(CommonIpFilterGroupModel model);

        [WebApiDelete("DeleteCommonIpFilterGroup?commonIpFilterGroupId={commonIpFilterGroupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteCommonIpFilterGroup(string commonIpFilterGroupId);
    }
}

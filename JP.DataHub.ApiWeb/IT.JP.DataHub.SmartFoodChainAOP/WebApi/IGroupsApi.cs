using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Global/Private/Groups", typeof(GroupsModel))]
    public interface IGroupsApi : ICommonResource<GroupsModel>
    {
        [WebApi("IsGroupMember?groupId={groupId}&scope={scope}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<IsGroupMemberModel> IsGroupMember(string groupId, string scope);

        [WebApi("GetGroupList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GroupsModel>> GetGroupList();

        [WebApi("GetGroupList/{scope}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GroupsModel>> GetGroupListWithScope(string scope);


        [WebApiDelete("DeleteEx/{groupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteEx(string groupId);
    }
}

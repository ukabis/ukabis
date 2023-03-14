using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/IntegratedTest/GroupFilter", typeof(GroupFilterModel))]
    public interface IGroupFilterApi : ICommonResource<GroupFilterModel>
    {
        [WebApi("Get/{code}?groupId={groupId}")]
        [AutoGenerateReturnModel]
        new WebApiRequestModel<GroupFilterModel> Get(string code, string groupId);

        [WebApiDelete("DeleteAll?groupId={groupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> DeleteAllByGroup(string groupId);

        [WebApiPost("Register?groupId={groupId}")]
        [AutoGenerateReturnModel]
        new WebApiRequestModel<string> Register(GroupFilterModel model, string groupId);
    }
}

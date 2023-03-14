using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;

namespace JP.DataHub.AdminWeb.WebAPI.Resources
{
    [WebApiResource("/Manage/RepositoryGroup", typeof(RepositoryGroupModel))]
    public interface IRepositoryGroupResource : ICommonResource<RepositoryGroupModel>
    {
        [WebApiGet("GetRepositoryGroup?repositoryGroupId={repositoryGroupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RepositoryGroupModel> GetRepositoryGroup(string repositoryGroupId);

        [WebApiGet("GetRepositoryGroupList?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RepositoryGroupModel>> GetRepositoryGroupList(string querystring = null);

        [WebApiGet("GetRepositoryGroupTypeList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RepositoryTypeModel>> GetRepositoryGroupTypeList(string querystring = null);

        [WebApiDelete("DeleteRepositoryGroup?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteRepositoryGroup(string key, string querystring = null);

        [WebApiPost("RegisterRepositoryGroup")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RepositoryGroupModel> RegisterRepositoryGroup(RepositoryGroupModel requestModel, string querystring = null);
    }
}

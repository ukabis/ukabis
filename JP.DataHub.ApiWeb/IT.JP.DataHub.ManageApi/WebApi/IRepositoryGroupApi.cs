using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/RepositoryGroup", typeof(RepositoryGroupModel))]
    public interface IRepositoryGroupApi
    {
        [WebApi("GetRepositoryGroup?repositoryGroupId={repositoryGroupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RepositoryGroupModel> GetRepositoryGroup(string repositoryGroupId);

        [WebApi("GetRepositoryGroupList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RepositoryGroupModel>> GetRepositoryGroupList();

        [WebApi("GetRepositoryGroupTypeList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RepositoryGroupModel>> GetRepositoryGroupTypeList();

        [WebApiPost("RegisterRepositoryGroup")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RepositoryGroupModel> RegisterRepositoryGroup(RepositoryGroupModel model);

        [WebApiDelete("DeleteRepositoryGroup?repositoryGroupId={repositoryGroupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteRepositoryGroup(string repositoryGroupId);
    }
}

using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/PartialStopTest", typeof(AcceptDataModel))]
    public interface IPartialStopTestApi : ICommonResource<AcceptDataModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AcceptDataModel model);


        [WebApi("GetRepositorySuccess")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AcceptDataModel> GetRepositorySuccess();

        [WebApi("GetRepositoryError")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AcceptDataModel> GetRepositoryError();


        [WebApi("GetSecondaryRepositorySuccess")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AcceptDataModel> GetSecondaryRepositorySuccess();

        [WebApi("GetRepositoryError")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AcceptDataModel> GetSecondaryRepositoryError();   
    }
}

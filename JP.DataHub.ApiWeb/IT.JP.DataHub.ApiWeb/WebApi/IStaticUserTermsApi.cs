using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("IgnoreOverride:/Manage/UserTerms", typeof(UserTermsModel))]
    public interface IStaticUserTermsApi
    {
        [WebApi("Get?user_terms_id={user_terms_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<UserTermsModel> Get(string user_terms_id);

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<UserTermsModel>> GetList();

    }
}

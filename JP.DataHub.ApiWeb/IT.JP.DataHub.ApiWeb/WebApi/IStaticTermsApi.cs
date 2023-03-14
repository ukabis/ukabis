using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("IgnoreOverride:/Manage/Terms", typeof(TermsModel))]
    public interface IStaticTermsApi : IResource
    {
        [WebApi("Get?terms_id={terms_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<TermsModel> Get(string terms_id);

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<TermsModel>> GetList();

        [WebApi("GetListByTermGroupCode?terms_group_code={terms_group_code}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<TermsModel>> GetListByTermGroupCode(string terms_group_code);

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<TermsRegisterResponseModel> Register(TermsModel model);

        [WebApiDelete("Delete?terms_id={terms_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string terms_id);

        [WebApiPost("Agreement?terms_id={terms_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Agreement(string terms_id);

        [WebApiPost("Revoke?terms_id={terms_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Revoke(string terms_id);

    }
}

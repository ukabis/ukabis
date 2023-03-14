using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("IgnoreOverride:/Manage/TermsGroup", typeof(TermsGroupModel))]
    public interface IStaticTermsGroupApi : IResource
    {
        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<TermsGroupModel>> GetList();

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<TermsGroupRegisterResponseModel> Register(TermsGroupModel model);

        [WebApiDelete("Delete?terms_group_code={terms_group_code}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string terms_group_code);
    }
}

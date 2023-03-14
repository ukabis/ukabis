using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("IgnoreOverride:/Manage/CertifiedApplication", typeof(CertifiedApplicationModel))]
    public interface IStaticCertifiedApplicationApi: IResource
    {
        [WebApi("Get?certified_application_id={certified_application_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CertifiedApplicationModel> Get(string certified_application_id);

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<CertifiedApplicationModel>> GetList();

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CertifiedApplicationRegisterResponseModel> Register(CertifiedApplicationModel model);

        [WebApiDelete("Delete?certified_application_id={certified_application_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string certified_application_id);

    }
}

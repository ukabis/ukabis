using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AttachfileNotImple", typeof(AttachFileBase64Model))]
    public interface IAttachfileNotImpleApi : ICommonResource<AttachFileBase64Model>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistBase64(AttachFileBase64Model model);
    }
}

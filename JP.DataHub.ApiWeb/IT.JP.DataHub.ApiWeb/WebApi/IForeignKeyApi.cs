using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ForeignKey", typeof(ForeignKeyModel))]
    public interface IForeignKeyApi : ICommonResource<ForeignKeyModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(ForeignKeyModel model);

        [WebApiPost("RegistErrors")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistErrors(ForeignKeyModel model);
    }
}

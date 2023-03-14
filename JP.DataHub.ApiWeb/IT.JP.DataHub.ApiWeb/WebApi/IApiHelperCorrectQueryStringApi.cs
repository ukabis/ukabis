using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ApiHelper/CorrectQueryString", typeof(AcceptDataModel))]
    public interface IApiHelperCorrectQueryStringApi : ICommonResource<AcceptDataModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AcceptDataModel model);

        [WebApi("GetQuery/{param}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AcceptDataModel> GetQuery(string param);

        [WebApi("GetUrl/{param}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AcceptDataModel> GetUrl(string param);
    }
}

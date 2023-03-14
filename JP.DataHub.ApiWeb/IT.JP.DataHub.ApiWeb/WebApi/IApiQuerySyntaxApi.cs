using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ApiQuerySyntax", typeof(ApiQuerySyntaxModel))]
    public interface IApiQuerySyntaxApi : ICommonResource<ApiQuerySyntaxModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiQuerySyntaxModel>> GetAll();

        [WebApi("GetSuppressError/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiQuerySyntaxModel>> GetSuppressError(string key);
        
        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<ApiQuerySyntaxModel> model);
    }
}

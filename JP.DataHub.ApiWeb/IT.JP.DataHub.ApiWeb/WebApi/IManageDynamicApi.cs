using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/Manage/DynamicApi", typeof(ApiModel))]
    public interface IManageDynamicApi : ICommonResource<ApiModel>
    {
        [WebApi("GetApiResourceFromUrl?url={url}&isTransparent={isTransparent}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiModel> GetApiResourceFromUrl(string url, bool isTransparent = false);

        [WebApiPost("UseOpenIdAuthorizeMethod?methodId={methodId}&use={use}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel UseOpenIdAuthorizeMethod(string methodId, bool use);

        [WebApiPost("AddOpenIdAuthorizeMethod?methodId={methodId}&openId={openId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel AddOpenIdAuthorizeMethod(string methodId, string openId);

        [WebApiPost("RegisterApi")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterApiResultModel> RegisterApi(ApiModel model);


        [WebApi("GetApiMethod?methodId={methodId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<MethodModel> GetApiMethod(string methodId);

        [WebApiPost("RegisterMethod")]
        [AutoGenerateReturnModel]
        WebApiRequestModel RegisterMethod(RegisterMethodModel model);


        [WebApi("GetSchemas")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SchemaModel>> GetSchemas();

        [WebApiPost("RegisterSchema", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel RegisterSchema(SchemaModel model);


        [WebApiDelete("DeleteApiFromUrl?url={url}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteApiFromUrl(string url);
    }
}

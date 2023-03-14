using JP.DataHub.AdminWeb.WebAPI.Models.Api;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;

namespace JP.DataHub.AdminWeb.WebAPI.Resources
{
    [WebApiResource("/Manage/DynamicApi", typeof(ApiResourceModel))]
    public interface IApiResource : ICommonResource<ApiResourceModel>
    {
        [WebApiGet("GetApiResourceMethodSimpleList?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ResourceApiSimpleModel>> GetApiResourceMethodSimpleList(string querystring = null);

        [WebApiGet("GetSchemas?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SchemaModel>> GetSchemas(string querystring = null);

        [WebApiGet("GetApiResourceFromApiId?apiId={apiId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiResourceModel> GetApiResourceFromApiId(string apiId);

        [WebApiGet("GetApiResourceLight?apiId={apiId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiResourceLightModel> GetApiResourceLight(string apiId);

        [WebApiGet("GetApiResourceHeader?vendorId={vendorId}&apiId={apiId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiResourceModel> GetApiResourceHeaderFromVendorIdApiId(string vendorId, string apiId);

        [WebApiGet("GetApiMethod?methodId={methodId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiModel> GetApiMethod(string methodId);

        [WebApiGet("GetSchemaById?schemaId={schemaId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SchemaModel> GetSchemaById(string schemaId);

        [WebApiGet("GetOpenIdCaList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<OpenIdCaModel>> GetOpenIdCaList();

        [WebApiGet("GetActionTypes")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ActionTypeModel>> GetActionTypes(string querystring = null);

        [WebApiGet("GetHttpMethods")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<HttpMethodTypeModel>> GetHttpMethods(string querystring = null);

        [WebApiGet("GetLanguageList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<LanguageModel>> GetLanguageList(string querystring = null);

        [WebApiGet("GetQueryTypeList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<QueryTypeModel>> GetQueryTypeList(string querystring = null);

        [WebApiGet("GetScriptTypeList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ScriptTypeModel>> GetScriptTypeList(string querystring = null);

        [WebApiPost("RegisterApi")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResourceApiModel> RegisterApi(RegisterResourceApiModel requestModel);

        [WebApiDelete("DeleteApi?apiId={apiId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteApi(string apiId);

        [WebApiPost("IsDuplicateController")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<bool> IsDuplicateController(RegisterResourceApiModel requestModel);

        [WebApiPost("RegisterMethod")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiModel> RegisterMethod(RegisterApiModel requestModel);

        [WebApiDelete("DeleteMethod?methodId={methodId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteMethod(string methodId);

        [WebApiPost("RegisterSchema")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SchemaModel> RegisterSchema(SchemaModel requestModel);

        [WebApiPost("RegisterUriOrResponseSchema")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SchemaModel> RegisterUriOrResponseSchema(SchemaModel requestModel);

        [WebApiDelete("DeleteSchema?schemaId={schemaId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteSchema(string schemaId);

        [WebApiGet("ExistsSameSchemaName?schemaName={schemaName}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<bool> ExistsSameSchemaName(string schemaName);

        [WebApiPost("IsDuplicateMethod")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<bool> IsDuplicateMethod(IsDuplicateMethodModel requestModel);

        [WebApiGet("IsExecutableApiMethod?actionType={actionType}&methodType={methodType}&repositoryGroupId={repositoryGroupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<bool> IsExecutableApiMethod(string actionType, string methodType, string repositoryGroupId);

        [WebApiPost("GetScriptSyntaxCheckMessage")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResponseMessageModel> GetScriptSyntaxCheckMessage(ScriptModel requestModel);
    }
}

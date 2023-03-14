using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;
using JP.DataHub.Com.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Service.Interface
{
    public interface IAdminApiService
    {
        WebApiResponseResult<List<ResourceApiSimpleModel>> GetApiResourceMethodSimpleList(string querystring = null);
        Task<WebApiResponseResult<List<ResourceApiSimpleModel>>> GetApiResourceMethodSimpleListAsync(string querystring = null);

        WebApiResponseResult<List<SchemaModel>> GetSchemas(string querystring = null);
        Task<WebApiResponseResult<List<SchemaModel>>> GetSchemasAsync(string querystring = null);

        WebApiResponseResult<ApiResourceModel> GetApiResourceFromApiId(string apiId);
        Task<WebApiResponseResult<ApiResourceModel>> GetApiResourceFromApiIdAsync(string apiId);

        WebApiResponseResult<ApiResourceLightModel> GetApiResourceLight(string apiId);
        Task<WebApiResponseResult<ApiResourceLightModel>> GetApiResourceLightAsync(string apiId);

        WebApiResponseResult<ApiResourceHeaderModel> GetApiResourceHeaderFromVendorIdApiId(string vendorId, string apiId);
        Task<WebApiResponseResult<ApiResourceHeaderModel>> GetApiResourceHeaderFromVendorIdApiIdAsync(string vendorId, string apiId);

        WebApiResponseResult<ApiModel> GetApiMethod(string methodId);
        Task<WebApiResponseResult<ApiModel>> GetApiMethodAsync(string methodId);

        WebApiResponseResult<SchemaModel> GetSchemaById(string schemaId);
        Task<WebApiResponseResult<SchemaModel>> GetSchemaByIdAsync(string schemaId);

        WebApiResponseResult<List<ActionTypeModel>> GetActionTypes();
        Task<WebApiResponseResult<List<ActionTypeModel>>> GetActionTypesAsync();

        WebApiResponseResult<List<HttpMethodTypeModel>> GetHttpMethods();
        Task<WebApiResponseResult<List<HttpMethodTypeModel>>> GetHttpMethodsAsync();

        WebApiResponseResult<List<LanguageModel>> GetLanguageList();
        Task<WebApiResponseResult<List<LanguageModel>>> GetLanguageListAsync();

        WebApiResponseResult<List<QueryTypeModel>> GetQueryTypeList();
        Task<WebApiResponseResult<List<QueryTypeModel>>> GetQueryTypeListAsync();

        WebApiResponseResult<List<ScriptTypeModel>> GetScriptTypeList();
        Task<WebApiResponseResult<List<ScriptTypeModel>>> GetScriptTypeListAsync();

        WebApiResponseResult<RegisterResourceApiModel> RegisterApi(object data);
        Task<WebApiResponseResult<RegisterResourceApiModel>> RegisterApiAsync(object data);

        WebApiResponseResult DeleteApi(string apiId);
        Task<WebApiResponseResult> DeleteApiAsync(string apiId);

        WebApiResponseResult<bool> IsDuplicateController(object data);
        Task<WebApiResponseResult<bool>> IsDuplicateControllerAsync(object data);

        WebApiResponseResult<RegisterApiModel> RegisterMethod(object data);
        Task<WebApiResponseResult<RegisterApiModel>> RegisterMethodAsync(object data);
        WebApiResponseResult DeleteMethod(string methodId);
        Task<WebApiResponseResult> DeleteMethodAsync(string methodId);

        WebApiResponseResult<SchemaModel> RegisterSchema(object data);
        Task<WebApiResponseResult<SchemaModel>> RegisterSchemaAsync(object data);
        WebApiResponseResult<SchemaModel> RegisterUriOrResponseSchema(object data);
        Task<WebApiResponseResult<SchemaModel>> RegisterUriOrResponseSchemaAsync(object data);

        WebApiResponseResult DeleteSchema(string schemaId);
        Task<WebApiResponseResult> DeleteSchemaAsync(string schemaId);

        WebApiResponseResult<bool> ExistsSameSchemaName(string schemaName);
        Task<WebApiResponseResult<bool>> ExistsSameSchemaNameAsync(string schemaName);

        WebApiResponseResult<bool> IsDuplicateMethod(object data);
        Task<WebApiResponseResult<bool>> IsDuplicateMethodAsync(object data);

        WebApiResponseResult<bool> IsExecutableApiMethod(string actionType, string methodType, string repositoryGroupId);
        Task<WebApiResponseResult<bool>> IsExecutableApiMethodAsync(string actionType, string methodType, string repositoryGroupId);

        WebApiResponseResult<ResponseMessageModel> GetScriptSyntaxCheckMessage(object data);
        Task<WebApiResponseResult<ResponseMessageModel>> GetScriptSyntaxCheckMessageAsync(object data);

        WebApiResponseResult ApiFlush();
        Task<WebApiResponseResult> ApiFlushAsync();

        WebApiResponseResult<object> AdaptResourceSchema(string resourceUrl);
        Task<WebApiResponseResult<object>> AdaptResourceSchemaAsync(string resourceUrl);
    }
}

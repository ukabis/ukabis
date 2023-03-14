using JP.DataHub.AdminWeb.Service.Interface;
using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;
using JP.DataHub.AdminWeb.WebAPI.Resources;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Service.Core.Impl;

namespace JP.DataHub.AdminWeb.Service.Impl
{
    public class AdminApiService : CommonCrudService, IAdminApiService
    {
        public WebApiResponseResult<List<ResourceApiSimpleModel>> GetApiResourceMethodSimpleList(string querystring = null)
            => BaseRepository.GetList<ResourceApiSimpleModel>(GetDaoS<ILoginUser, IApiResource>(), querystring);
        public Task<WebApiResponseResult<List<ResourceApiSimpleModel>>> GetApiResourceMethodSimpleListAsync(string querystring = null)
            => Task.Run(() => GetApiResourceMethodSimpleList(querystring));

        public WebApiResponseResult<List<SchemaModel>> GetSchemas(string querystring = null)
            => BaseRepository.GetList<SchemaModel>(GetDaoS<ILoginUser, IApiResource>(), querystring);
        public Task<WebApiResponseResult<List<SchemaModel>>> GetSchemasAsync(string querystring = null)
            => Task.Run(() => GetSchemas(querystring));

        public WebApiResponseResult<ApiResourceModel> GetApiResourceFromApiId(string apiId)
            => BaseRepository.Get<ApiResourceModel>(GetDaoS<ILoginUser, IApiResource>(), apiId);
        public Task<WebApiResponseResult<ApiResourceModel>> GetApiResourceFromApiIdAsync(string apiId)
            => Task.Run(() => GetApiResourceFromApiId(apiId));

        public WebApiResponseResult<ApiResourceLightModel> GetApiResourceLight(string apiId)
            => BaseRepository.Get<ApiResourceLightModel>(GetDaoS<ILoginUser, IApiResource>(), apiId);
        public Task<WebApiResponseResult<ApiResourceLightModel>> GetApiResourceLightAsync(string apiId)
            => Task.Run(() => GetApiResourceLight(apiId));

        public WebApiResponseResult<ApiResourceHeaderModel> GetApiResourceHeaderFromVendorIdApiId(string vendorId, string apiId)
            => BaseRepository.Get<ApiResourceHeaderModel>(GetDaoS<ILoginUser, IApiResource>(), vendorId, apiId);
        public Task<WebApiResponseResult<ApiResourceHeaderModel>> GetApiResourceHeaderFromVendorIdApiIdAsync(string vendorId, string apiId)
            => Task.Run(() => GetApiResourceHeaderFromVendorIdApiId(vendorId, apiId));

        
        public WebApiResponseResult<ApiModel> GetApiMethod(string methodId)
            => BaseRepository.Get<ApiModel>(GetDaoS<ILoginUser, IApiResource>(), methodId);
        public Task<WebApiResponseResult<ApiModel>> GetApiMethodAsync(string methodId)
            => Task.Run(() => GetApiMethod(methodId));

        public WebApiResponseResult<SchemaModel> GetSchemaById(string schemaId)
            => BaseRepository.Get<SchemaModel>(GetDaoS<ILoginUser, IApiResource>(), schemaId);
        public Task<WebApiResponseResult<SchemaModel>> GetSchemaByIdAsync(string schemaId)
            => Task.Run(() => GetSchemaById(schemaId));

        public WebApiResponseResult<List<ActionTypeModel>> GetActionTypes()
            => BaseRepository.GetList<ActionTypeModel>(GetDaoS<ILoginUser, IApiResource>());
        public Task<WebApiResponseResult<List<ActionTypeModel>>> GetActionTypesAsync()
            => Task.Run(() => GetActionTypes());

        public WebApiResponseResult<List<HttpMethodTypeModel>> GetHttpMethods()
            => BaseRepository.GetList<HttpMethodTypeModel>(GetDaoS<ILoginUser, IApiResource>());
        public Task<WebApiResponseResult<List<HttpMethodTypeModel>>> GetHttpMethodsAsync()
            => Task.Run(() => GetHttpMethods());

        public WebApiResponseResult<List<LanguageModel>> GetLanguageList()
            => BaseRepository.GetList<LanguageModel>(GetDaoS<ILoginUser, IApiResource>());
        public Task<WebApiResponseResult<List<LanguageModel>>> GetLanguageListAsync()
            => Task.Run(() => GetLanguageList());

        public WebApiResponseResult<List<QueryTypeModel>> GetQueryTypeList()
            => BaseRepository.GetList<QueryTypeModel>(GetDaoS<ILoginUser, IApiResource>());
        public Task<WebApiResponseResult<List<QueryTypeModel>>> GetQueryTypeListAsync()
            => Task.Run(() => GetQueryTypeList());

        public WebApiResponseResult<List<ScriptTypeModel>> GetScriptTypeList()
            => BaseRepository.GetList<ScriptTypeModel>(GetDaoS<ILoginUser, IApiResource>());
        public Task<WebApiResponseResult<List<ScriptTypeModel>>> GetScriptTypeListAsync()
            => Task.Run(() => GetScriptTypeList());

        public WebApiResponseResult<RegisterResourceApiModel> RegisterApi(object data)
            => BaseRepository.Access<RegisterResourceApiModel>(GetDaoS<ILoginUser, IApiResource>(), data);
        public Task<WebApiResponseResult<RegisterResourceApiModel>> RegisterApiAsync(object data)
            => Task.Run(() => RegisterApi(data));

        public WebApiResponseResult DeleteApi(string apiId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, IApiResource, string>(), apiId);
        public Task<WebApiResponseResult> DeleteApiAsync(string apiId)
            => Task.Run(() => DeleteApi(apiId));

        public WebApiResponseResult<bool> IsDuplicateController(object data)
            => BaseRepository.Access<bool>(GetDaoS<ILoginUser, IApiResource, bool>(), data);
        public Task<WebApiResponseResult<bool>> IsDuplicateControllerAsync(object data)
            => Task.Run(() => IsDuplicateController(data));

        public WebApiResponseResult<RegisterApiModel> RegisterMethod(object data)
            => BaseRepository.Access<RegisterApiModel>(GetDaoS<ILoginUser, IApiResource>(), data);
        public Task<WebApiResponseResult<RegisterApiModel>> RegisterMethodAsync(object data)
            => Task.Run(() => RegisterMethod(data));

        public WebApiResponseResult DeleteMethod(string methodId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, IApiResource, string>(), methodId);
        public Task<WebApiResponseResult> DeleteMethodAsync(string methodId)
            => Task.Run(() => DeleteMethod(methodId));

        public WebApiResponseResult<SchemaModel> RegisterSchema(object data)
            => BaseRepository.Access<SchemaModel>(GetDaoS<ILoginUser, IApiResource>(), data);
        public Task<WebApiResponseResult<SchemaModel>> RegisterSchemaAsync(object data)
            => Task.Run(() => RegisterSchema(data));

        public WebApiResponseResult<SchemaModel> RegisterUriOrResponseSchema(object data)
            => BaseRepository.Access<SchemaModel>(GetDaoS<ILoginUser, IApiResource>(), data);
        public Task<WebApiResponseResult<SchemaModel>> RegisterUriOrResponseSchemaAsync(object data)
            => Task.Run(() => RegisterUriOrResponseSchema(data));

        public WebApiResponseResult DeleteSchema(string schemaId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, IApiResource, string>(), schemaId);
        public Task<WebApiResponseResult> DeleteSchemaAsync(string schemaId)
            => Task.Run(() => DeleteSchema(schemaId));

        public WebApiResponseResult<bool> ExistsSameSchemaName(string schemaName)
            =>BaseRepository.Get<bool>(GetDaoS<ILoginUser, IApiResource, bool>(), schemaName);
        public Task<WebApiResponseResult<bool>> ExistsSameSchemaNameAsync(string schemaName)
            => Task.Run(() => ExistsSameSchemaName(schemaName));

        public WebApiResponseResult<bool> IsDuplicateMethod(object data)
            => BaseRepository.Access<bool>(GetDaoS<ILoginUser, IApiResource, bool>(), data);
        public Task<WebApiResponseResult<bool>> IsDuplicateMethodAsync(object data)
            => Task.Run(() => IsDuplicateMethod(data));

        public WebApiResponseResult<bool> IsExecutableApiMethod(string actionType, string methodType, string repositoryGroupId)
            => BaseRepository.Get<bool>(GetDaoS<ILoginUser, IApiResource, bool>(), actionType, methodType, repositoryGroupId);
        public Task<WebApiResponseResult<bool>> IsExecutableApiMethodAsync(string actionType, string methodType, string repositoryGroupId)
            => Task.Run(() => IsExecutableApiMethod(actionType, methodType, repositoryGroupId));

        public WebApiResponseResult<ResponseMessageModel> GetScriptSyntaxCheckMessage(object data)
            => BaseRepository.Access<ResponseMessageModel>(GetDaoS<ILoginUser, IApiResource>(), data);
        public Task<WebApiResponseResult<ResponseMessageModel>> GetScriptSyntaxCheckMessageAsync(object data)
            => Task.Run(() => GetScriptSyntaxCheckMessage(data));

        public WebApiResponseResult ApiFlush()
            => BaseRepository.DeleteAll(GetDaoS<ILoginUser, IApiFlushResource, ApiResourceModel>());
        public Task<WebApiResponseResult> ApiFlushAsync()
            => Task.Run(() => ApiFlush());

        public WebApiResponseResult<object> AdaptResourceSchema(string resourceUrl)
        {
            var dao = GetDaoS<ILoginUser, IDynamicApiResource, ApiResourceModel>();
            dao.Headers = new Dictionary<string, string[]>() { { "X-Cache", new string[] { "x" } } };
            return BaseRepository.Access<object>(dao, resourceUrl);
        }
        public Task<WebApiResponseResult<object>> AdaptResourceSchemaAsync(string resourceUrl)
            => Task.Run(() => AdaptResourceSchema(resourceUrl));
    }
}

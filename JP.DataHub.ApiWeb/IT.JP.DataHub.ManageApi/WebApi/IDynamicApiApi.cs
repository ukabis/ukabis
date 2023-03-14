using IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/DynamicApi", typeof(ApiModel))]
    public interface IDynamicApiApi
    {
        [WebApi("GetCategories")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DynamicApiCategoryModel>> GetCategories();
        
        [WebApi("GetFields")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DynamicApiFieldModel>> GetFields();
        
        [WebApi("GetTags")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DynamicApiTagModel>> GetTags();
        
        [WebApi("GetRepositoryGroups")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DynamicApiRepositoryGroupModel>> GetRepositoryGroups();
 
        [WebApi("GetAttachFileBlobStorage")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DynamicApiAttachFileStorageModel>> GetAttachFileBlobStorage();

        [WebApi("GetAttachFileMetaStorage")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DynamicApiAttachFileStorageModel>> GetAttachFileMetaStorage();

        [WebApi("GetActionTypes")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ActionTypeModel>> GetActionTypes();

        [WebApi("GetHttpMethods")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<HttpMethodTypeModel>> GetHttpMethods();

        [WebApi("GetControllerCommonIpFilterGroupList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiCommonIpFilterGroupModel>> GetControllerCommonIpFilterGroupList();

        [WebApi("GetOpenIdCaList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DynamicApiOpenIdCaModel>> GetOpenIdCaList();


        [WebApi("GetLanguageList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<LanguageModel>> GetLanguageList();


        [WebApi("GetScriptTypeList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ScriptTypeModel>> GetScriptTypeList();

        [WebApi("GetQueryTypeList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<QueryTypeModel>> GetQueryTypeList();

        [WebApi("GetApiResourceMethodList?isAll={isAll}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiModel>> GetApiResourceMethodList(bool isAll = false);

        [WebApi("GetApiResourceMethodSimpleList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ApiModel>> GetApiResourceMethodSimpleList();

        [WebApi("GetApiResourceFromApiId?apiId={apiId}&isTransparent={isTransparent}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiModel> GetApiResourceFromApiId(string apiId,bool isTransparent = false);

        [WebApi("GetApiResourceFromUrl?url={url}&isTransparent={isTransparent}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ApiModel> GetApiResourceFromUrl(string url,bool isTransparent = false);

        [WebApiPost("RegisterApi")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterApiResponseModel> RegisterApi(RegisterApiRequestModel model);

        [WebApiDelete("DeleteApi?apiId={apiId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteApi(string apiId);

        [WebApiDelete("DeleteApiFromUrl?url={url}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteApiFromUrl(string url);


        [WebApi("GetApiMethod?methodId={methodId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<MethodModel> GetApiMethod(string methodId);

        [WebApi("GetApiMethodFromUrl?url={url}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<MethodModel> GetApiMethodFromUrl(string url);

        [WebApiPost("RegisterMethod")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterMethodResponseModel> RegisterMethod(RegisterMethodRequestModel model);

        [WebApiDelete("DeleteMethod?methodId={methodId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteMethod(string methodId);

        [WebApiDelete("DeleteMethodFromUrl?url={url}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteMethodFromUrl(string url);

        [WebApi("GetSchemas")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<SchemaModel>> GetSchemas();

        [WebApi("GetSchemaById?schemaId={schemaId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SchemaModel> GetSchemaById(string schemaId);

        [WebApiPost("RegisterUriOrResponseSchema")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterSchemaResponseModel> RegisterUriOrResponseSchema(RegisterSchemaRequestModel model);

        [WebApiPost("RegisterSchema")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterSchemaResponseModel> RegisterSchema(RegisterSchemaRequestModel model);

        [WebApiDelete("DeleteSchema?schemaId={schemaId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteSchema(string schemaId);

        [WebApiPost("AddOpenIdAuthorizeMethod?methodId={methodId}&openId={openId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel AddOpenIdAuthorizeMethod(string methodId, string openId);

        [WebApiDelete("DeleteOpenIdAuthorizeMethod?methodId={methodId}&openId={openId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteOpenIdAuthorizeMethod(string methodId, string openId);

        [WebApiPost("UseOpenIdAuthorizeMethod?methodId={methodId}&use={use}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UseOpenIdAuthorizeMethod(string methodId, bool use);

    }
}

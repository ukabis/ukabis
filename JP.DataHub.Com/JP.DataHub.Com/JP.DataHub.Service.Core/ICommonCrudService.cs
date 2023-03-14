using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Com.Validations.Attributes;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Service.Core.Impl;

namespace JP.DataHub.Service.Core
{
    [Log]
    [ArgumentValidator]
    public interface ICommonCrudService
    {
        void SetupController(object controller);

        WebApiResponseResult<TModel> Access<TModel>(params object[] parameter) where TModel : new();
        Task<WebApiResponseResult<TModel>> AccessAsync<TModel>(params object[] parameter) where TModel : new();
        WebApiResponseResult<TModel> Access<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new();
        Task<WebApiResponseResult<TModel>> AccessAsync<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new();
        WebApiResponseResult<TModel> Access<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<TModel>> AccessAsync<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<TModel>> AccessAsync<TResource, TModel>(Action<CallApiOptions> options = null, params object[] parameter) where TResource : IResource where TModel : new();

        WebApiResponseResult<List<TModel>> AccessList<TModel>(params object[] parameter) where TModel : new();
        Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TModel>(params object[] parameter) where TModel : new();
        WebApiResponseResult<List<TModel>> AccessList<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new();
        Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new();
        WebApiResponseResult<List<TModel>> AccessList<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TResource, TModel>(Action<CallApiOptions> options = null, params object[] parameter) where TResource : IResource where TModel : new();

        WebApiResponseResult<int> GetCount(string queryString = null);
        Task<WebApiResponseResult<int>> GetCountAsync(string queryString = null);
        WebApiResponseResult<int> GetCount<TResource>(string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult<int>> GetCountAsync<TResource>(string queryString = null) where TResource : IResource;
        WebApiResponseResult<int> GetCount<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<int>> GetCountAsync<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<int>> GetCountAsync<TResource>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult<List<TModel>> OData<TModel>(string queryString = null) where TModel : new();
        Task<WebApiResponseResult<List<TModel>>> ODataAsync<TModel>(string queryString = null) where TModel : new();
        WebApiResponseResult<List<TModel>> OData<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new();
        Task<WebApiResponseResult<List<TModel>>> ODataAsync<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new();
        WebApiResponseResult<List<TModel>> OData<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<TModel>>> ODataAsync<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<TModel>>> ODataAsync<TResource, TModel>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new();

        WebApiResponseResult<PagingResult<List<TModel>>> OData<TModel>(PagingRequest pagingRequest, string queryString = null) where TModel : new();
        Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TModel>(PagingRequest pagingRequest, string queryString = null) where TModel : new();
        WebApiResponseResult<PagingResult<List<TModel>>> OData<TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new();
        Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new();
        WebApiResponseResult<PagingResult<List<TModel>>> OData<TSelector, TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TSelector, TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TResource, TModel>(PagingRequest pagingRequest, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new();

        WebApiResponseResult ODataDelete(string queryString = null);
        Task<WebApiResponseResult> ODataDeleteAsync(string queryString = null);
        WebApiResponseResult ODataDelete<TResource>(string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult> ODataDeleteAsync<TResource>(string queryString = null) where TResource : IResource;
        WebApiResponseResult ODataDelete<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> ODataDeleteAsync<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> ODataDeleteAsync<TResource>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult<TModel> Get<TModel>(string key, string queryString = null) where TModel : new();
        Task<WebApiResponseResult<TModel>> GetAsync<TModel>(string key, string queryString = null) where TModel : new();
        WebApiResponseResult<TModel> Get<TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new();
        Task<WebApiResponseResult<TModel>> GetAsync<TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new();
        WebApiResponseResult<TModel> Get<TSelector, TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<TModel>> GetAsync<TSelector, TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<TModel>> GetAsync<TResource, TModel>(string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new();

        WebApiResponseResult<List<TModel>> GetList<TModel>(string queryString = null) where TModel : new();
        Task<WebApiResponseResult<List<TModel>>> GetListAsync<TModel>(string queryString = null) where TModel : new();
        WebApiResponseResult<List<TModel>> GetList<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new();
        Task<WebApiResponseResult<List<TModel>>> GetListAsync<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new();
        WebApiResponseResult<List<TModel>> GetList<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<TModel>>> GetListAsync<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<TModel>>> GetListAsync<TResource, TModel>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new();

        WebApiResponseResult<bool> Exists(string key, string queryString = null);
        Task<WebApiResponseResult<bool>> ExistsAsync(string key, string queryString = null);
        WebApiResponseResult<bool> Exists<TResource>(string key, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult<bool>> ExistsAsync<TResource>(string key, string queryString = null) where TResource : IResource;
        WebApiResponseResult<bool> Exists<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<bool>> ExistsAsync<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<bool>> ExistsAsync<TResource>(string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult Register(object data, string queryString = null);
        Task<WebApiResponseResult> RegisterAsync(object data, string queryString = null);
        WebApiResponseResult Register<TResource>(object data, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult> RegisterAsync<TResource>(object data, string queryString = null) where TResource : IResource;
        WebApiResponseResult Register<TSelector, TResource>(object data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> RegisterAsync<TSelector, TResource>(object data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> RegisterAsync<TResource>(object data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult<List<string>> RegisterList(List<object> data, string queryString = null);
        Task<WebApiResponseResult<List<string>>> RegisterListAsync(List<object> data, string queryString = null);
        WebApiResponseResult<List<string>> RegisterList<TResource>(List<object> data, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult<List<string>>> RegisterListAsync<TResource>(List<object> data, string queryString = null) where TResource : IResource;
        WebApiResponseResult<List<string>> RegisterList<TSelector, TResource>(List<object> data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<string>>> RegisterListAsync<TSelector, TResource>(List<object> data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<string>>> RegisterListAsync<TResource>(List<object> data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult Delete(string key, string queryString = null);
        Task<WebApiResponseResult> DeleteAsync(string key, string queryString = null);
        WebApiResponseResult Delete<TResource>(string key, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult> DeleteAsync<TResource>(string key, string queryString = null) where TResource : IResource;
        WebApiResponseResult Delete<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> DeleteAsync<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> DeleteAsync<TResource>(string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult DeleteAll();
        Task<WebApiResponseResult> DeleteAllAsync();
        WebApiResponseResult DeleteAll<TResource>() where TResource : IResource;
        Task<WebApiResponseResult> DeleteAllAsync<TResource>() where TResource : IResource;
        WebApiResponseResult DeleteAll<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> DeleteAllAsync<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> DeleteAllAsync<TResource>(Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult<TModel> Update<TModel>(string key, object data, string queryString = null) where TModel : new();
        Task<WebApiResponseResult<TModel>> UpdateAsync<TModel>(string key, object data, string queryString = null) where TModel : new();
        WebApiResponseResult<TModel> Update<TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new();
        Task<WebApiResponseResult<TModel>> UpdateAsync<TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new();
        WebApiResponseResult<TModel> Update<TSelector, TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<TModel>> UpdateAsync<TSelector, TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<TModel>> UpdateAsync<TResource, TModel>(string key, object data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new();

        #region AttachFile

        WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile(CreateAttachFileRequestModel data, string queryString = null);
        Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync(CreateAttachFileRequestModel data, string queryString = null);
        WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile<TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync<TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource;
        WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile<TSelector, TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync<TSelector, TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync<TResource>(CreateAttachFileRequestModel data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult DeleteAttachFile(string fileId, string queryString = null);
        Task<WebApiResponseResult> DeleteAttachFileAsync(string fileId, string queryString = null);
        WebApiResponseResult DeleteAttachFile<TResource>(string fileId, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult> DeleteAttachFileAsync<TResource>(string fileId, string queryString = null) where TResource : IResource;
        WebApiResponseResult DeleteAttachFile<TSelector, TResource>(string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> DeleteAttachFileAsync<TSelector, TResource>(string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> DeleteAttachFileAsync<TResource>(string fileId, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult<ImageModel> GetAttachFile([Required] string key, string queryString = null);
        Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync([Required] string key, string queryString = null);
        WebApiResponseResult<ImageModel> GetAttachFile<TResource>([Required] string key, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync<TResource>([Required] string key, string queryString = null) where TResource : IResource;
        WebApiResponseResult<ImageModel> GetAttachFile<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync<TResource>([Required] string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta([Required] string key, string queryString = null);
        Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync([Required] string key, string queryString = null);
        WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta<TResource>([Required] string key, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync<TResource>([Required] string key, string queryString = null) where TResource : IResource;
        WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync<TResource>([Required] string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList(string queryString = null);
        Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync(string queryString = null);
        WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList<TResource>(string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync<TResource>(string queryString = null) where TResource : IResource;
        WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync<TResource>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        WebApiResponseResult UploadAttachFile(Stream data, string fileId, string queryString = null);
        Task<WebApiResponseResult> UploadAttachFileAsync(Stream data, string fileId, string queryString = null);
        WebApiResponseResult UploadAttachFile<TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource;
        Task<WebApiResponseResult> UploadAttachFileAsync<TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource;
        WebApiResponseResult UploadAttachFile<TSelector, TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> UploadAttachFileAsync<TSelector, TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> UploadAttachFileAsync<TResource>(Stream data, string fileId, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource;

        #endregion

        WebApiResponseResult AdaptResourceSchema();
        Task<WebApiResponseResult> AdaptResourceSchemaAsync();
        WebApiResponseResult AdaptResourceSchema<TResource>() where TResource : IResource;
        Task<WebApiResponseResult> AdaptResourceSchemaAsync<TResource>() where TResource : IResource;
        WebApiResponseResult AdaptResourceSchema<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> AdaptResourceSchemaAsync<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult> AdaptResourceSchemaAsync<TResource>(Action<CallApiOptions> options = null) where TResource : IResource;

        [Cache(typeof(AdaptResourceSchemaCacheKey))]
        WebApiResponseResultSimple GetResourceSchema();
        Task<WebApiResponseResultSimple> GetResourceSchemaAsync();
        [Cache(typeof(AdaptResourceSchemaCacheKey))]
        WebApiResponseResultSimple GetResourceSchema<TResource>() where TResource : IResource;
        Task<WebApiResponseResultSimple> GetResourceSchemaAsync<TResource>() where TResource : IResource;
        [Cache(typeof(AdaptResourceSchemaCacheKey))]
        WebApiResponseResultSimple GetResourceSchema<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResultSimple> GetResourceSchemaAsync<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector;
        [Cache(typeof(AdaptResourceSchemaCacheKey))]
        WebApiResponseResultSimple GetResourceSchema<TResource>(Action<CallApiOptions> options = null) where TResource : IResource;
        Task<WebApiResponseResultSimple> GetResourceSchemaAsync<TResource>(Action<CallApiOptions> options = null) where TResource : IResource;
    }
}

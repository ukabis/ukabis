using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.Service.Core.Repository;

namespace JP.DataHub.Service.Core.Impl
{
    public class CommonCrudService : ICommonCrudService
    {
        protected object Controller { get; set; }

        [Dependency]
        public ICommonCrudRepository BaseRepository { get; set; }

        public CommonCrudService()
        {
            this.AutoInjection();
        }

        public CommonCrudService(ICommonCrudRepository repository)
        {
            BaseRepository = repository;
        }

        public void SetupController(object controller) => Controller = controller;

        public WebApiResponseResult<int> GetCount(string queryString = null)
            => BaseRepository.GetCount(GetDao(), queryString);
        public Task<WebApiResponseResult<int>> GetCountAsync(string queryString = null)
            => Task.Run(() => BaseRepository.GetCount(GetDao(), queryString));
        public WebApiResponseResult<int> GetCount<TResource>(string queryString = null) where TResource : IResource
            => BaseRepository.GetCount(GetDaoR<TResource>(), queryString);
        public Task<WebApiResponseResult<int>> GetCountAsync<TResource>(string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetCount(GetDaoR<TResource>(), queryString));
        public WebApiResponseResult<int> GetCount<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.GetCount(GetDaoS<TSelector, TResource>(), queryString);
        public Task<WebApiResponseResult<int>> GetCountAsync<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.GetCount(GetDaoS<TSelector, TResource>(), queryString));
        public Task<WebApiResponseResult<int>> GetCountAsync<TResource>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetCount(GetDaoO<TResource>(options), queryString));

        public WebApiResponseResult<List<TModel>> OData<TModel>(string queryString = null) where TModel : new()
            => BaseRepository.OData<TModel>(GetDao(), queryString);
        public Task<WebApiResponseResult<List<TModel>>> ODataAsync<TModel>(string queryString = null) where TModel : new()
            => Task.Run(() => BaseRepository.OData<TModel>(GetDao(), queryString));
        public WebApiResponseResult<List<TModel>> OData<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new()
            => BaseRepository.OData<TModel>(GetDaoR<TResource, TModel>(), queryString);
        public Task<WebApiResponseResult<List<TModel>>> ODataAsync<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.OData<TModel>(GetDaoR<TResource, TModel>(), queryString));
        public WebApiResponseResult<List<TModel>> OData<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => BaseRepository.OData<TModel>(GetDaoS<TSelector, TResource, TModel>(), queryString);
        public Task<WebApiResponseResult<List<TModel>>> ODataAsync<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.OData<TModel>(GetDaoS<TSelector, TResource, TModel>(), queryString));
        public Task<WebApiResponseResult<List<TModel>>> ODataAsync<TResource, TModel>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.OData<TModel>(GetDaoO<TResource, TModel>(options), queryString));

        private WebApiResponseResult<PagingResult<List<TModel>>> _odata<TModel>([Required] PagingRequest pagingRequest, Func<RepositoryDaoInfo> getDaoFunc, string queryString = null) where TModel : new()
        {
            if (pagingRequest.Page <= 0)
            {
                throw new Exception("pagingRequest.Pageは1以上を指定してください");
            }
            var dao = getDaoFunc();
            var countresult = BaseRepository.ODataCount(dao, $"$count=true" + (string.IsNullOrEmpty(GetFilterQeuryString(queryString, out var qs)) ? string.Empty : $"&{qs}"));
            if (countresult.IsSuccessStatusCode == false)
            {
                return null;
            }
            var count = countresult.Result;
            int skip = pagingRequest.DisplayCount * (pagingRequest.Page - 1);
            var odataresult = BaseRepository.OData<TModel>(getDaoFunc(), $"$top={pagingRequest.DisplayCount}&$skip={skip}" + (string.IsNullOrEmpty(queryString) ? string.Empty : $"&{queryString}"));
            var pagingResult = new PagingResult<List<TModel>>() { Page = pagingRequest.Page, RecordCount = count, MaxPageCount = (count + pagingRequest.DisplayCount - 1) / pagingRequest.DisplayCount, Result = odataresult.Result };
            var result = new WebApiResponseResult<PagingResult<List<TModel>>>();
            result.IsSuccessStatusCode = odataresult.IsSuccessStatusCode;
            result.StatusCode = odataresult.StatusCode;
            result.Result = pagingResult;
            result.RawContent = odataresult.RawContent;
            result.RawContentString = odataresult.RawContentString;
            result.RequestMessge = odataresult.RequestMessge;
            result.Error = odataresult.Error;
            return result;
        }

        public WebApiResponseResult<PagingResult<List<TModel>>> OData<TModel>([Required] PagingRequest pagingRequest, string queryString = null) where TModel : new()
            => _odata<TModel>(pagingRequest, () => GetDao(), queryString);
        public Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TModel>([Required] PagingRequest pagingRequest, string queryString = null) where TModel : new()
            => Task.Run(() => OData<TModel>(pagingRequest, queryString));
        public WebApiResponseResult<PagingResult<List<TModel>>> OData<TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new()
            => _odata<TModel>(pagingRequest, () => GetDaoR<TResource, TModel>(), queryString);
        public Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new()
            => Task.Run(() => ODataAsync<TResource,TModel>(pagingRequest, queryString));
        public WebApiResponseResult<PagingResult<List<TModel>>> OData<TSelector, TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => _odata<TModel>(pagingRequest, () => GetDaoS<TSelector, TResource, TModel>(), queryString);
        public Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TSelector, TResource, TModel>(PagingRequest pagingRequest, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => ODataAsync<TSelector,TResource,TModel>(pagingRequest, queryString));
        public Task<WebApiResponseResult<PagingResult<List<TModel>>>> ODataAsync<TResource, TModel>(PagingRequest pagingRequest, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new()
            => Task.Run(() => _odata<TModel>(pagingRequest, () => GetDaoO<TResource, TModel>(options), queryString));

        public WebApiResponseResult ODataDelete(string queryString = null)
            => BaseRepository.ODataDelete(GetDao(), queryString);
        public Task<WebApiResponseResult> ODataDeleteAsync(string queryString = null)
            => Task.Run(() => BaseRepository.ODataDelete(GetDao(), queryString));
        public WebApiResponseResult ODataDelete<TResource>(string queryString = null) where TResource : IResource
            => BaseRepository.ODataDelete(GetDaoR<TResource>(), queryString);
        public Task<WebApiResponseResult> ODataDeleteAsync<TResource>(string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.ODataDelete(GetDaoR<TResource>(), queryString));
        public WebApiResponseResult ODataDelete<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.ODataDelete(GetDaoS<TSelector, TResource>(), queryString);
        public Task<WebApiResponseResult> ODataDeleteAsync<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.ODataDelete(GetDaoS<TSelector, TResource>(), queryString));
        public Task<WebApiResponseResult> ODataDeleteAsync<TResource>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.ODataDelete(GetDaoO<TResource>(options), queryString));

        public WebApiResponseResult<TModel> Get<TModel>([Required] string key, string queryString = null) where TModel : new()
            => BaseRepository.Get<TModel>(GetDao(), key, queryString);
        public Task<WebApiResponseResult<TModel>> GetAsync<TModel>([Required] string key, string queryString = null) where TModel : new()
            => Task.Run(() => BaseRepository.Get<TModel>(GetDao(), key, queryString));
        public WebApiResponseResult<TModel> Get<TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new()
            => BaseRepository.Get<TModel>(GetDaoR<TResource, TModel>(), key, queryString);
        public Task<WebApiResponseResult<TModel>> GetAsync<TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.Get<TModel>(GetDaoR<TResource, TModel>(), key, queryString));
        public WebApiResponseResult<TModel> Get<TSelector, TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => BaseRepository.Get<TModel>(GetDaoS<TSelector, TResource, TModel>(), key, queryString);
        public Task<WebApiResponseResult<TModel>> GetAsync<TSelector, TResource, TModel>(string key, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.Get<TModel>(GetDaoS<TSelector, TResource, TModel>(), key, queryString));
        public Task<WebApiResponseResult<TModel>> GetAsync<TResource, TModel>(string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.Get<TModel>(GetDaoO<TResource, TModel>(options), key, queryString));

        public WebApiResponseResult<List<TModel>> GetList<TModel>(string queryString = null) where TModel : new()
            => BaseRepository.GetList<TModel>(GetDao(), queryString);
        public Task<WebApiResponseResult<List<TModel>>> GetListAsync<TModel>(string queryString = null) where TModel : new()
            => Task.Run(() => BaseRepository.GetList<TModel>(GetDao(), queryString));
        public WebApiResponseResult<List<TModel>> GetList<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new()
            => BaseRepository.GetList<TModel>(GetDaoR<TResource, TModel>(), queryString);
        public Task<WebApiResponseResult<List<TModel>>> GetListAsync<TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.GetList<TModel>(GetDaoR<TResource, TModel>(), queryString));
        public WebApiResponseResult<List<TModel>> GetList<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => BaseRepository.GetList<TModel>(GetDaoS<TSelector, TResource, TModel>(), queryString);
        public Task<WebApiResponseResult<List<TModel>>> GetListAsync<TSelector, TResource, TModel>(string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.GetList<TModel>(GetDaoS<TSelector, TResource, TModel>(), queryString));
        public Task<WebApiResponseResult<List<TModel>>> GetListAsync<TResource, TModel>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.GetList<TModel>(GetDaoO<TResource, TModel>(options), queryString));

        public WebApiResponseResult<bool> Exists(string key, string queryString = null)
            => BaseRepository.Exists(GetDao(), key, queryString);
        public Task<WebApiResponseResult<bool>> ExistsAsync(string key, string queryString = null)
            => Task.Run(() => BaseRepository.Exists(GetDao(), key, queryString));
        public WebApiResponseResult<bool> Exists<TResource>(string key, string queryString = null) where TResource : IResource
            => BaseRepository.Exists(GetDaoR<TResource>(), key, queryString);
        public Task<WebApiResponseResult<bool>> ExistsAsync<TResource>(string key, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.Exists(GetDaoR<TResource>(), key, queryString));
        public WebApiResponseResult<bool> Exists<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.Exists(GetDaoS<TSelector, TResource>(), key, queryString);
        public Task<WebApiResponseResult<bool>> ExistsAsync<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.Exists(GetDaoS<TSelector, TResource>(), key, queryString));
        public Task<WebApiResponseResult<bool>> ExistsAsync<TResource>(string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.Exists(GetDaoO<TResource>(options), key, queryString));

        public WebApiResponseResult Register([Required] object data, string queryString = null)
            => BaseRepository.Register(GetDao(), data, queryString);
        public Task<WebApiResponseResult> RegisterAsync([Required] object data, string queryString = null)
            => Task.Run(() => BaseRepository.Register(GetDao(), data, queryString));
        public WebApiResponseResult Register<TResource>(object data, string queryString = null) where TResource : IResource
            => BaseRepository.Register(GetDaoR<TResource>(), data, queryString);
        public Task<WebApiResponseResult> RegisterAsync<TResource>(object data, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.Register(GetDaoR<TResource>(), data, queryString));
        public WebApiResponseResult Register<TSelector, TResource>(object data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.Register(GetDaoS<TSelector, TResource>(), data, queryString);
        public Task<WebApiResponseResult> RegisterAsync<TSelector, TResource>(object data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.Register(GetDaoS<TSelector, TResource>(), data, queryString));
        public Task<WebApiResponseResult> RegisterAsync<TResource>(object data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.Register(GetDaoO<TResource>(options), data, queryString));

        public WebApiResponseResult<List<string>> RegisterList([Required] List<object> data, string queryString = null)
        {
            if (data.Count() == 0) throw new Exception("CrudService.RegisterListのdataには要素を指定してください");
            return BaseRepository.RegisterList(GetDao(), data, queryString);
        }
        public Task<WebApiResponseResult<List<string>>> RegisterListAsync([Required] List<object> data, string queryString = null)
        {
            if (data.Count() == 0) throw new Exception("CrudService.RegisterListのdataには要素を指定してください");
            return Task.Run(() => BaseRepository.RegisterList(GetDao(), data, queryString));
        }
        public WebApiResponseResult<List<string>> RegisterList<TResource>(List<object> data, string queryString = null) where TResource : IResource
        {
            if (data.Count() == 0) throw new Exception("CrudService.RegisterListのdataには要素を指定してください");
            return BaseRepository.RegisterList(GetDaoR<TResource>(), data, queryString);
        }
        public Task<WebApiResponseResult<List<string>>> RegisterListAsync<TResource>(List<object> data, string queryString = null) where TResource : IResource
        {
            if (data.Count() == 0) throw new Exception("CrudService.RegisterListのdataには要素を指定してください");
            return Task.Run(() => BaseRepository.RegisterList(GetDaoR<TResource>(), data, queryString));
        }
        public WebApiResponseResult<List<string>> RegisterList<TSelector, TResource>(List<object> data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
        {
            if (data.Count() == 0) throw new Exception("CrudService.RegisterListのdataには要素を指定してください");
            return BaseRepository.RegisterList(GetDaoS<TSelector, TResource>(), data, queryString);
        }
        public Task<WebApiResponseResult<List<string>>> RegisterListAsync<TSelector, TResource>(List<object> data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
        {
            if (data.Count() == 0) throw new Exception("CrudService.RegisterListのdataには要素を指定してください");
            return Task.Run(() => BaseRepository.RegisterList(GetDaoS<TSelector, TResource>(), data, queryString));
        }
        public Task<WebApiResponseResult<List<string>>> RegisterListAsync<TResource>(List<object> data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
        {
            if (data.Count() == 0) throw new Exception("CrudService.RegisterListのdataには要素を指定してください");
            return Task.Run(() => BaseRepository.RegisterList(GetDaoO<TResource>(options), data, queryString));
        }

        public WebApiResponseResult Delete([Required] string key, string queryString = null)
            => BaseRepository.Delete(GetDao(), key, queryString);
        public Task<WebApiResponseResult> DeleteAsync([Required] string key, string queryString = null)
            => Task.Run(() => BaseRepository.Delete(GetDao(), key, queryString));
        public WebApiResponseResult Delete<TResource>(string key, string queryString = null) where TResource : IResource
            => BaseRepository.Delete(GetDaoR<TResource>(), key, queryString);
        public Task<WebApiResponseResult> DeleteAsync<TResource>(string key, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.Delete(GetDaoR<TResource>(), key, queryString));
        public WebApiResponseResult Delete<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.Delete(GetDaoS<TSelector, TResource>(), key, queryString);
        public Task<WebApiResponseResult> DeleteAsync<TSelector, TResource>(string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.Delete(GetDaoS<TSelector, TResource>(), key, queryString));
        public Task<WebApiResponseResult> DeleteAsync<TResource>(string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.Delete(GetDaoO<TResource>(options), key, queryString));

        public WebApiResponseResult DeleteAll()
            => BaseRepository.DeleteAll(GetDao());
        public Task<WebApiResponseResult> DeleteAllAsync()
            => Task.Run(() => BaseRepository.DeleteAll(GetDao()));
        public WebApiResponseResult DeleteAll<TResource>() where TResource : IResource
            => BaseRepository.DeleteAll(GetDaoR<TResource>());
        public Task<WebApiResponseResult> DeleteAllAsync<TResource>() where TResource : IResource
            => Task.Run(() => BaseRepository.DeleteAll(GetDaoR<TResource>()));
        public WebApiResponseResult DeleteAll<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.DeleteAll(GetDaoS<TSelector, TResource>());
        public Task<WebApiResponseResult> DeleteAllAsync<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.DeleteAll(GetDaoS<TSelector, TResource>()));
        public Task<WebApiResponseResult> DeleteAllAsync<TResource>(Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.DeleteAll(GetDaoO<TResource>(options)));

        public WebApiResponseResult<TModel> Update<TModel>([Required] string key, [Required] object data, string queryString = null) where TModel : new()
            => BaseRepository.Update<TModel>(GetDao(), key, data, queryString);
        public Task<WebApiResponseResult<TModel>> UpdateAsync<TModel>([Required] string key, [Required] object data, string queryString = null) where TModel : new()
            => Task.Run(() => BaseRepository.Update<TModel>(GetDao(), key, data, queryString));
        public WebApiResponseResult<TModel> Update<TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new()
            => BaseRepository.Update<TModel>(GetDaoR<TResource, TModel>(), key, data, queryString);
        public Task<WebApiResponseResult<TModel>> UpdateAsync<TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.Update<TModel>(GetDaoR<TResource, TModel>(), key, data, queryString));
        public WebApiResponseResult<TModel> Update<TSelector, TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => BaseRepository.Update<TModel>(GetDaoS<TSelector, TResource, TModel>(), key, data, queryString);
        public Task<WebApiResponseResult<TModel>> UpdateAsync<TSelector, TResource, TModel>(string key, object data, string queryString = null) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.Update<TModel>(GetDaoS<TSelector, TResource, TModel>(), key, data, queryString));
        public Task<WebApiResponseResult<TModel>> UpdateAsync<TResource, TModel>(string key, object data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.Update<TModel>(GetDaoO<TResource, TModel>(options), key, data, queryString));

        public WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile([Required] CreateAttachFileRequestModel data, string queryString = null)
            => BaseRepository.CreateAttachFile(GetDao(), data, queryString);
        public Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync([Required] CreateAttachFileRequestModel data, string queryString = null)
            => Task.Run(() => BaseRepository.CreateAttachFile(GetDao(), data, queryString));
        public WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile<TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource
            => BaseRepository.CreateAttachFile(GetDaoR<TResource>(), data, queryString);
        public Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync<TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.CreateAttachFile(GetDaoR<TResource>(), data, queryString));
        public WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile<TSelector, TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.CreateAttachFile(GetDaoS<TSelector, TResource>(), data, queryString);
        public Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync<TSelector, TResource>(CreateAttachFileRequestModel data, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.CreateAttachFile(GetDaoS<TSelector, TResource>(), data, queryString));
        public Task<WebApiResponseResult<CreateAttachFileResponseModel>> CreateAttachFileAsync<TResource>(CreateAttachFileRequestModel data, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.CreateAttachFile(GetDaoO<TResource>(options), data, queryString));

        public WebApiResponseResult UploadAttachFile([Required] Stream data, string fileId, string queryString = null)
            => BaseRepository.UploadAttachFile(GetDao(), data, fileId, queryString);
        public Task<WebApiResponseResult> UploadAttachFileAsync([Required] Stream data, string fileId, string queryString = null)
            => Task.Run(() => BaseRepository.UploadAttachFile(GetDao(), data, fileId, queryString));
        public WebApiResponseResult UploadAttachFile<TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource
            => BaseRepository.UploadAttachFile(GetDaoR<TResource>(), data, fileId, queryString);
        public Task<WebApiResponseResult> UploadAttachFileAsync<TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.UploadAttachFile(GetDaoR<TResource>(), data, fileId, queryString));
        public WebApiResponseResult UploadAttachFile<TSelector, TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.UploadAttachFile(GetDaoS<TSelector, TResource>(), data, fileId, queryString);
        public Task<WebApiResponseResult> UploadAttachFileAsync<TSelector, TResource>(Stream data, string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.UploadAttachFile(GetDaoS<TSelector, TResource>(), data, fileId, queryString));
        public Task<WebApiResponseResult> UploadAttachFileAsync<TResource>(Stream data, string fileId, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.UploadAttachFile(GetDaoO<TResource>(options), data, fileId, queryString));

        public WebApiResponseResult<ImageModel> GetAttachFile([Required] string key, string queryString = null)
            => BaseRepository.GetAttachFile(GetDao(), key, queryString);
        public Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync([Required] string key, string queryString = null)
            => Task.Run(() => BaseRepository.GetAttachFile(GetDao(), key, queryString));
        public WebApiResponseResult<ImageModel> GetAttachFile<TResource>([Required] string key, string queryString = null) where TResource : IResource
            => BaseRepository.GetAttachFile(GetDaoR<TResource>(), key, queryString);
        public Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync<TResource>([Required] string key, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetAttachFile(GetDaoR<TResource>(), key, queryString));
        public WebApiResponseResult<ImageModel> GetAttachFile<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.GetAttachFile(GetDaoS<TSelector, TResource>(), key, queryString);
        public Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.GetAttachFile(GetDaoS<TSelector, TResource>(), key, queryString));
        public Task<WebApiResponseResult<ImageModel>> GetAttachFileAsync<TResource>([Required] string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetAttachFile(GetDaoO<TResource>(options), key, queryString));

        public WebApiResponseResult DeleteAttachFile(string fileId, string queryString = null)
            => BaseRepository.DeleteAttachFile(GetDao(), fileId, queryString);
        public Task<WebApiResponseResult> DeleteAttachFileAsync(string fileId, string queryString = null)
            => Task.Run(() => BaseRepository.DeleteAttachFile(GetDao(), fileId, queryString));
        public WebApiResponseResult DeleteAttachFile<TResource>(string fileId, string queryString = null) where TResource : IResource
            => BaseRepository.DeleteAttachFile(GetDaoR<TResource>(), fileId, queryString);
        public Task<WebApiResponseResult> DeleteAttachFileAsync<TResource>(string fileId, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.DeleteAttachFile(GetDaoR<TResource>(), fileId, queryString));
        public WebApiResponseResult DeleteAttachFile<TSelector, TResource>(string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.DeleteAttachFile(GetDaoS<TSelector, TResource>(), fileId, queryString);
        public Task<WebApiResponseResult> DeleteAttachFileAsync<TSelector, TResource>(string fileId, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.DeleteAttachFile(GetDaoS<TSelector, TResource>(), fileId, queryString));
        public Task<WebApiResponseResult> DeleteAttachFileAsync<TResource>(string fileId, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.DeleteAttachFile(GetDaoO<TResource>(options), fileId, queryString));

        public WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta([Required] string key, string queryString = null)
            => BaseRepository.GetAttachFileMeta(GetDao(), key, queryString);
        public Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync([Required] string key, string queryString = null)
            => Task.Run(() => BaseRepository.GetAttachFileMeta(GetDao(), key, queryString));
        public WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta<TResource>([Required] string key, string queryString = null) where TResource : IResource
            => BaseRepository.GetAttachFileMeta(GetDaoR<TResource>(), key, queryString);
        public Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync<TResource>([Required] string key, string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetAttachFileMeta(GetDaoR<TResource>(), key, queryString));
        public WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.GetAttachFileMeta(GetDaoS<TSelector, TResource>(), key, queryString);
        public Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync<TSelector, TResource>([Required] string key, string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.GetAttachFileMeta(GetDaoS<TSelector, TResource>(), key, queryString));
        public Task<WebApiResponseResult<GetAttachFileResponseModel>> GetAttachFileMetaAsync<TResource>([Required] string key, string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetAttachFileMeta(GetDaoO<TResource>(options), key, queryString));

        public WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList(string queryString = null)
            => BaseRepository.GetAttachFileMetaList(GetDao(), queryString);
        public Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync(string queryString = null)
            => Task.Run(() => BaseRepository.GetAttachFileMetaList(GetDao(), queryString));
        public WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList<TResource>(string queryString = null) where TResource : IResource
            => BaseRepository.GetAttachFileMetaList(GetDaoR<TResource>(), queryString);
        public Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync<TResource>(string queryString = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetAttachFileMetaList(GetDaoR<TResource>(), queryString));
        public WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.GetAttachFileMetaList(GetDaoS<TSelector, TResource>(), queryString);
        public Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync<TSelector, TResource>(string queryString = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.GetAttachFileMetaList(GetDaoS<TSelector, TResource>(), queryString));
        public Task<WebApiResponseResult<List<GetAttachFileResponseModel>>> GetAttachFileMetaListAsync<TResource>(string queryString = null, Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.GetAttachFileMetaList(GetDaoO<TResource>(options), queryString));

        public WebApiResponseResult AdaptResourceSchema()
            => BaseRepository.AdaptResourceSchema(GetDao());
        public Task<WebApiResponseResult> AdaptResourceSchemaAsync()
            => Task.Run(() => BaseRepository.AdaptResourceSchema(GetDao()));
        public WebApiResponseResult AdaptResourceSchema<TResource>() where TResource : IResource
            => BaseRepository.AdaptResourceSchema(GetDaoR<TResource>());
        public Task<WebApiResponseResult> AdaptResourceSchemaAsync<TResource>() where TResource : IResource
            => Task.Run(() => BaseRepository.AdaptResourceSchema(GetDaoR<TResource>()));
        public WebApiResponseResult AdaptResourceSchema<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.AdaptResourceSchema(GetDaoS<TSelector, TResource>());
        public Task<WebApiResponseResult> AdaptResourceSchemaAsync<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.AdaptResourceSchema(GetDaoS<TSelector, TResource>()));
        public Task<WebApiResponseResult> AdaptResourceSchemaAsync<TResource>(Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => BaseRepository.AdaptResourceSchema(GetDaoO<TResource>(options)));

        public WebApiResponseResultSimple GetResourceSchema()
            => BaseRepository.GetResourceSchema(GetDao()).ToSimple();
        public Task<WebApiResponseResultSimple> GetResourceSchemaAsync()
            => Task.Run(() => GetResourceSchema());
        public WebApiResponseResultSimple GetResourceSchema<TResource>() where TResource : IResource
            => BaseRepository.GetResourceSchema(GetDaoR<TResource>()).ToSimple();
        public Task<WebApiResponseResultSimple> GetResourceSchemaAsync<TResource>() where TResource : IResource
            => Task.Run(() => GetResourceSchema<TResource>());
        public WebApiResponseResultSimple GetResourceSchema<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector
            => BaseRepository.GetResourceSchema(GetDaoS<TSelector, TResource>()).ToSimple();
        public Task<WebApiResponseResultSimple> GetResourceSchemaAsync<TSelector, TResource>() where TResource : IResource where TSelector : IDynamicApiClientSelector
            => Task.Run(() => GetResourceSchema<TSelector, TResource>());
        public WebApiResponseResultSimple GetResourceSchema<TResource>(Action<CallApiOptions> options = null) where TResource : IResource
            => BaseRepository.AdaptResourceSchema(GetDaoO<TResource>(options)).ToSimple();
        public Task<WebApiResponseResultSimple> GetResourceSchemaAsync<TResource>(Action<CallApiOptions> options = null) where TResource : IResource
            => Task.Run(() => GetResourceSchema<TResource>(options));

        public WebApiResponseResult<TModel> Access<TModel>(params object[] parameter) where TModel : new()
            => BaseRepository.Access<TModel>(GetDao(), parameter);
        public Task<WebApiResponseResult<TModel>> AccessAsync<TModel>(params object[] parameter) where TModel : new()
            => Task.Run(() => BaseRepository.Access<TModel>(GetDao(), parameter));
        public WebApiResponseResult<TModel> Access<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new()
            => BaseRepository.Access<TModel>(GetDaoR<TResource, TModel>(), parameter);
        public Task<WebApiResponseResult<TModel>> AccessAsync<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.Access<TModel>(GetDaoR<TResource, TModel>(), parameter));
        public WebApiResponseResult<TModel> Access<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => BaseRepository.Access<TModel>(GetDaoS<TSelector, TResource, TModel>(), parameter);
        public Task<WebApiResponseResult<TModel>> AccessAsync<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.Access<TModel>(GetDaoS<TSelector, TResource, TModel>(), parameter));
        public Task<WebApiResponseResult<TModel>> AccessAsync<TResource, TModel>(Action<CallApiOptions> options = null, params object[] parameter) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.Access<TModel>(GetDaoO<TResource, TModel>(options), parameter));

        public WebApiResponseResult<List<TModel>> AccessList<TModel>(params object[] parameter) where TModel : new()
            => BaseRepository.AccessList<TModel>(GetDao(), parameter);
        public Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TModel>(params object[] parameter) where TModel : new()
            => Task.Run(() => BaseRepository.AccessList<TModel>(GetDao(), parameter));
        public WebApiResponseResult<List<TModel>> AccessList<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new()
            => BaseRepository.AccessList<TModel>(GetDaoR<TResource, TModel>(), parameter);
        public Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.AccessList<TModel>(GetDaoR<TResource, TModel>(), parameter));
        public WebApiResponseResult<List<TModel>> AccessList<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => BaseRepository.AccessList<TModel>(GetDaoS<TSelector, TResource, TModel>(), parameter);
        public Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TSelector, TResource, TModel>(params object[] parameter) where TResource : IResource where TModel : new() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => BaseRepository.AccessList<TModel>(GetDaoS<TSelector, TResource, TModel>(), parameter));
        public Task<WebApiResponseResult<List<TModel>>> AccessListAsync<TResource, TModel>(Action<CallApiOptions> options = null, params object[] parameter) where TResource : IResource where TModel : new()
            => Task.Run(() => BaseRepository.AccessList<TModel>(GetDaoO<TResource, TModel>(options), parameter));

        protected RepositoryDaoInfo GetDao(bool isActionNameAutoCollect = true)
        {
            var mb = CrudServiceInjectorAttribute.CrudServiceInjectorHandler.FindControllerFromStackFrame();
            if (mb == null)
            {
                return null;
            }
            var names = CrudServiceInjectorAttribute.CrudServiceInjectorHandler.GetRepositoryInfo(mb);
            if (names.ActionName == null && isActionNameAutoCollect == true)
            {
                var prev = new StackFrame(1, true);
                var prevMethod = prev.GetMethod();
                names.ActionName = prevMethod.Name;
            }
            if (names.ActionName == null || names.RepositoryName == null)
            {
                return null;
            }
            return new RepositoryDaoInfo() { ClassName = names.RepositoryName, ModelName = names.RepositoryModelName, ActionName = names.ActionName, ResultType = names.ResultModel, Headers = names.Headers };
        }

        protected RepositoryDaoInfo GetDaoS<TSelector, TResource, TModel>(string actionName = null)
            => new RepositoryDaoInfo() { ClassType = typeof(TResource), ModelType = typeof(TModel), ActionName = actionName ?? GetActionName(new StackFrame(1, true)), Headers = null/*TODO:now not supported*/, DynamicApiClientSelector = typeof(TSelector) };
        protected RepositoryDaoInfo GetDaoS<TSelector, TResource>(string actionName = null) where TResource : IResource where TSelector : IDynamicApiClientSelector
            => new RepositoryDaoInfo() { ClassType = typeof(TResource), ActionName = actionName ?? GetActionName(new StackFrame(1, true)), Headers = null/*TODO:now not supported*/, DynamicApiClientSelector = typeof(TSelector) };
        protected RepositoryDaoInfo GetDaoR<TResource>(string actionName = null) where TResource : IResource
            => new RepositoryDaoInfo() { ClassType = typeof(TResource), ActionName = actionName ?? GetActionName(new StackFrame(1, true)), Headers = null/*TODO:now not supported*/ };
        protected RepositoryDaoInfo GetDaoR<TResource, TModel>(string actionName = null) where TResource : IResource
            => new RepositoryDaoInfo() { ClassType = typeof(TResource), ModelType = typeof(TModel), ActionName = actionName ?? GetActionName(new StackFrame(1, true)), Headers = null/*TODO:now not supported*/ };
        protected RepositoryDaoInfo GetDaoO<TResource, TModel>(Action<CallApiOptions> options = null, string actionName = null) where TResource : IResource
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            var cao = new CallApiOptions(param);
            if (options != null) options(cao);
            return new RepositoryDaoInfo() { ClassType = typeof(TResource), ModelType = typeof(TModel), ActionName = actionName ?? GetActionName(new StackFrame(1, true)), Headers = null/*TODO:now not supported*/, Param = cao.Param };
        }
        protected RepositoryDaoInfo GetDaoO<TResource>(Action<CallApiOptions> options = null, string actionName = null) where TResource : IResource
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            var cao = new CallApiOptions(param);
            if (options != null) options(cao);
            return new RepositoryDaoInfo() { ClassType = typeof(TResource), ActionName = actionName ?? GetActionName(new StackFrame(1, true)), Headers = null/*TODO:now not supported*/, Param = cao.Param };
        }

        protected string GetFilterQeuryString(string queryString, out string result)
        {
            var param = queryString?.Split('&');
            var filterStr = param?.FirstOrDefault(x => x.Contains("$filter"));
            var groupStr = param?.FirstOrDefault(x => x.Contains("groupId"));
            var tmp = string.Join('&', new string[] { filterStr, groupStr });
            result = tmp.TrimStart('&');
            return result;
        }

        private string GetActionName(StackFrame stackFrame)
        {
            var name = stackFrame.GetMethod().Name;
            if (name?.StartsWith("<") == true)
            {
                name = name.Substring(1);
            }
            int pos = name?.IndexOf("Async>") ?? -1;
            if (pos != -1)
            {
                name = name.Substring(0, pos);
            }
            return name;
        }
    }
}

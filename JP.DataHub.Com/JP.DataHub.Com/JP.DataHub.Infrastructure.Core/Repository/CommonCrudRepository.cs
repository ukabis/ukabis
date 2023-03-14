using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Misc;
using JP.DataHub.Infrastructure.Core.Repository.Attributes;
using JP.DataHub.Service.Core.Repository;

namespace JP.DataHub.Infrastructure.Core.Repository
{
    public class CommonCrudRepository : ICommonCrudRepository
    {
        public CommonCrudRepository()
        {
            this.AutoInjection();
        }

        [CrudRepositoryModel(typeof(GetCountResponseModel))]
        public WebApiResponseResult<int> GetCount(RepositoryDaoInfo api, string queryString = null)
            => api.Call(queryString).ToGeneric<int,GetCountResponseModel>();

        [CrudRepositoryArray]
        public WebApiResponseResult<List<T>> OData<T>(RepositoryDaoInfo api, string queryString = null) where T : new()
            => api.Call(queryString).ToGeneric<List<T>>();

        public WebApiResponseResult<T> ODataNoList<T>(RepositoryDaoInfo api, string queryString = null) where T : new()
            => api.Call(queryString).ToGeneric<T>();

        [CrudRepositoryArray]
        [CrudRepositoryModel(typeof(int))]
        public WebApiResponseResult<int> ODataCount(RepositoryDaoInfo api, string queryString = null)
            => api.Call(queryString).ToGeneric<int>();

        public WebApiResponseResult ODataDelete(RepositoryDaoInfo api, string queryString = null)
            => api.Call(queryString);

        public WebApiResponseResult<T> Get<T>(RepositoryDaoInfo api, params object[] paramters) where T : new()
            => api.Call(paramters).ToGeneric<T>();

        [CrudRepositoryArray]
        public WebApiResponseResult<List<T>> GetList<T>(RepositoryDaoInfo api, string queryString = null) where T : new()
            => api.Call(queryString).ToGeneric<List<T>>();

        public WebApiResponseResult<bool> Exists(RepositoryDaoInfo api, string key, string queryString = null)
            => api.Call(key, queryString).ToGeneric<bool>();

        public WebApiResponseResult Register(RepositoryDaoInfo api, [ConvertRequestModel] object data, string queryString = null)
            => api.Call(data, queryString).NoneGeneric<RegisterResponseModel>();

        public WebApiResponseResult Register(RepositoryDaoInfo api, [ConvertRequestModel] object data)
            => api.Call(data);

        [CrudRepositoryArray]
        public WebApiResponseResult<List<string>> RegisterList(RepositoryDaoInfo api, [ConvertRequestModel] List<object> data, string queryString = null)
            => api.Call(data, queryString).ToGeneric<List<string>, List<RegisterResponseModel>>();
        [CrudRepositoryArray]
        public WebApiResponseResult RegisterList(RepositoryDaoInfo api, [ConvertRequestModel] List<object> data)
            => api.Call(data);
        public WebApiResponseResult Delete(RepositoryDaoInfo api, string key, string queryString = null)
            => api.Call(key, queryString);
        public WebApiResponseResult Delete(RepositoryDaoInfo api, string key)
            => api.Call(key);

        public WebApiResponseResult DeleteAll(RepositoryDaoInfo api)
            => api.Call();

        public WebApiResponseResult<T> Update<T>(RepositoryDaoInfo api, string key, [ConvertRequestModel]object data, string queryString = null) where T : new()
            => api.Call(key, data, queryString).ToGeneric<T>();
        
        public WebApiResponseResult Update(RepositoryDaoInfo api, [ConvertRequestModel] object data)
            => api.Call(data);

        public WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile(RepositoryDaoInfo api, CreateAttachFileRequestModel data, string queryString = null)
            => api.Call(data, queryString).ToGeneric<CreateAttachFileResponseModel>();

        public WebApiResponseResult UploadAttachFile(RepositoryDaoInfo api, Stream data, string fileId, string queryString = null)
            => api.Call(data, fileId, queryString);

        [CrudRepositoryResultNoConvert]
        public WebApiResponseResult<ImageModel> GetAttachFile(RepositoryDaoInfo api, string key, string queryString = null)
            => api.Call(key, queryString).ToGeneric<ImageModel>();

        public WebApiResponseResult DeleteAttachFile(RepositoryDaoInfo api, string fileId, string querystring = null)
            => api.Call(fileId, querystring);

        public WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta(RepositoryDaoInfo api, string fileId, string querystring = null)
            => api.Call().ToGeneric<GetAttachFileResponseModel>();

        [CrudRepositoryArray]
        public WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList(RepositoryDaoInfo api, string querystring = null)
            => api.Call().ToGeneric<List<GetAttachFileResponseModel>>();

        public WebApiResponseResult AdaptResourceSchema(RepositoryDaoInfo api)
            => api.Call();
        public WebApiResponseResult GetResourceSchema(RepositoryDaoInfo api)
            => api.Call();

        public WebApiResponseResult<T> Access<T>(RepositoryDaoInfo api, params object[] parameter) where T : new()
        {
            return api.Call(parameter).ToGeneric<T>();
        }

        [CrudRepositoryArray]
        public WebApiResponseResult<List<T>> AccessList<T>(RepositoryDaoInfo api, params object[] parameter) where T : new()
        {
            return api.Call(parameter).ToGeneric<List<T>>();
        }
    }
}

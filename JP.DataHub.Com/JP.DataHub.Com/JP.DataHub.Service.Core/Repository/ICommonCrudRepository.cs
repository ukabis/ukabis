using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;

namespace JP.DataHub.Service.Core.Repository
{
    public interface ICommonCrudRepository
    {
        WebApiResponseResult<int> GetCount(RepositoryDaoInfo api, string queryString = null);
        WebApiResponseResult<List<T>> OData<T>(RepositoryDaoInfo api, string queryString = null) where T : new();
        WebApiResponseResult<T> ODataNoList<T>(RepositoryDaoInfo api, string queryString = null) where T : new();
        WebApiResponseResult<int> ODataCount(RepositoryDaoInfo api, string queryString = null);
        WebApiResponseResult ODataDelete(RepositoryDaoInfo api, string queryString = null);
        WebApiResponseResult<T> Get<T>(RepositoryDaoInfo api, params object[] paramters) where T : new();
        WebApiResponseResult<List<T>> GetList<T>(RepositoryDaoInfo api, string queryString = null) where T : new();
        WebApiResponseResult<bool> Exists(RepositoryDaoInfo api, string key, string queryString = null);
        WebApiResponseResult Register(RepositoryDaoInfo api, object data, string queryString = null);
        WebApiResponseResult Register(RepositoryDaoInfo api, object data);
        WebApiResponseResult<List<string>> RegisterList(RepositoryDaoInfo api, List<object> data, string queryString = null);
        WebApiResponseResult RegisterList(RepositoryDaoInfo api, List<object> data);
        WebApiResponseResult Delete(RepositoryDaoInfo api, string key, string queryString = null);
        WebApiResponseResult Delete(RepositoryDaoInfo api, string key);
        WebApiResponseResult DeleteAll(RepositoryDaoInfo api);
        WebApiResponseResult<T> Update<T>(RepositoryDaoInfo api, string key, object data, string queryString = null) where T : new();
        WebApiResponseResult Update(RepositoryDaoInfo api, object data);

        WebApiResponseResult<CreateAttachFileResponseModel> CreateAttachFile(RepositoryDaoInfo api, CreateAttachFileRequestModel data, string queryString);
        WebApiResponseResult UploadAttachFile(RepositoryDaoInfo api, Stream data, string fileId, string queryString);
        WebApiResponseResult<ImageModel> GetAttachFile(RepositoryDaoInfo api, string key, string queryString = null);
        WebApiResponseResult DeleteAttachFile(RepositoryDaoInfo api, string fileId, string querystring = null);
        WebApiResponseResult<GetAttachFileResponseModel> GetAttachFileMeta(RepositoryDaoInfo api, string fileId, string querystring = null);
        WebApiResponseResult<List<GetAttachFileResponseModel>> GetAttachFileMetaList(RepositoryDaoInfo api, string querystring = null);

        WebApiResponseResult AdaptResourceSchema(RepositoryDaoInfo api);
        WebApiResponseResult GetResourceSchema(RepositoryDaoInfo api);

        WebApiResponseResult<T> Access<T>(RepositoryDaoInfo api, params object[] parameter) where T : new();
        WebApiResponseResult<List<T>> AccessList<T>(RepositoryDaoInfo api, params object[] parameter) where T : new();
    }
}

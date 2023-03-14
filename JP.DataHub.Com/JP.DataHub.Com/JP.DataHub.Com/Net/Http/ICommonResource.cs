using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;

namespace JP.DataHub.Com.Net.Http
{
    public interface IAttachFileResource : IResource
    {
        #region AttachFile

        [WebApiPost("CreateAttachFile?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CreateAttachFileResponseModel> CreateAttachFile(CreateAttachFileRequestModel data, string queryString = null);

        [WebApiPost("UploadAttachFile/{fileId}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel UploadAttachFile(Stream data, string fileId, string queryString = null);

        [WebApi("GetAttachFile/{fileId}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ImageModel> GetAttachFile(string fileId, string querystring = null);

        [WebApiDelete("DeleteAttachFile/{fileId}?{querystring}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteAttachFile(string fileId, string querystring = null);

        [WebApi("GetAttachFileMeta/{fileId}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GetAttachFileResponseModel> GetAttachFileMeta(string fileId, string querystring = null);

        [WebApi("GetAttachFileMetaList/?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GetAttachFileResponseModel>> GetAttachFileMetaList(string querystring = null);

        #endregion
    }

    public interface ICommonResource<T> : IResource
    {
        #region TRANSPARENT

        [WebApi("GetCount?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GetCountResponseModel> GetCount(string querystring = null);

        [WebApi("OData?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<T>> OData(string querystring = null);

        [WebApiDelete("ODataDelete?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataDelete(string querystring = null);

        [WebApiPost("AdaptResourceSchema", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel AdaptResourceSchema();

        [WebApiGet("GetResourceSchema", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetResourceSchema();

        [WebApiPost("RegisterRawData")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegisterRawData(List<T> requestModel);

        [WebApi("ODataRawData?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<T>> ODataRawData(string querystring = null);

        #endregion

        #region AttachFile

        [WebApiPost("CreateAttachFile?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CreateAttachFileResponseModel> CreateAttachFile(CreateAttachFileRequestModel data, string queryString = null);

        [WebApiPost("UploadAttachFile/{fileId}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel UploadAttachFile(Stream data, string fileId, string queryString = null);

        [WebApi("GetAttachFile/{fileId}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ImageModel> GetAttachFile(string fileId, string querystring = null);

        [WebApiDelete("DeleteAttachFile/{fileId}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteAttachFile(string fileId, string querystring = null);

        [WebApi("GetAttachFileMeta/{fileId}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GetAttachFileResponseModel> GetAttachFileMeta(string fileId, string querystring = null);

        [WebApi("GetAttachFileMetaList/?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GetAttachFileResponseModel>> GetAttachFileMetaList(string querystring = null);

        #endregion

        #region 履歴

        [WebApi("DriveOutDocument/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel DriveOutDocument(string id, string querystring = null);

        [WebApi("ReturnDocument/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ReturnDocument(string id, string querystring = null);

        [WebApiDelete("HistoryThrowAway/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel HistoryThrowAway(string id, string querystring = null);

        [WebApi("GetDocumentVersion/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GetDocumentVersionResponseModel>> GetDocumentVersion(string id, string querystring = null);

        [WebApi("GetDocumentHistory/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<T> GetDocumentHistory(string id, string querystring = null);

        [WebApi("GetDocumentHistory/{id}?version={versionNo}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<T> GetDocumentHistoryWithVersionNo(string id, int versionNo);

        [WebApi("GetDocumentHistory/{id}?version={versionGuid}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<T> GetDocumentHistoryWithVersionGuid(string id, Guid versionGuid);

        #endregion

        #region 履歴＋添付ファイル

        [WebApi("GetAttachFileDocumentVersion/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<GetAttachFileVersionResponseModel>> GetAttachFileDocumentVersion(string id, string querystring = null);

        [WebApi("DriveOutAttachFileDocument/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel DriveOutAttachFileDocument(string id, string querystring = null);

        [WebApi("ReturnAttachFileDocument/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ReturnAttachFileDocument(string id, string querystring = null);

        [WebApi("GetAttachFileDocumentHistory/{id}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetAttachFileDocumentHistory(string id, string querystring = null);

        [WebApi("GetAttachFileDocumentHistory/{id}?version={versionNo}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetAttachFileDocumentHistoryWithVersionNo(string id, int versionNo);

        [WebApi("GetAttachFileDocumentHistory/{id}?version={versionNo}&Key={key}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetAttachFileDocumentHistoryWithVersionNoAndKey(string id, int versionNo, string key);

        [WebApi("GetAttachFileDocumentHistory/{id}?version={versionGuid}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetAttachFileDocumentHistoryWithVersionGuid(string id, Guid versionGuid);

        [WebApi("GetAttachFileDocumentHistory/{id}?version={versionGuid}&Key={key}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetAttachFileDocumentHistoryWithVersionGuidAndKey(string id, Guid versionGuid, string key);

        #endregion

        #region Version

        [WebApi("GetCurrentVersion?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CurrentVersionResponseModel> GetCurrentVersion(string querystring = null);

        [WebApi("GetVersionInfo?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VersionInfoResponseModel> GetVersionInfo(string querystring = null);

        [WebApiPost("CreateRegisterVersion?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterVersionResponseModel> CreateRegisterVersion(string querystring = null);

        [WebApiPost("CompleteRegisterVersion?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VersionInfoResponseModel> CompleteRegisterVersion(string querystring = null);

        [WebApi("GetRegisterVersion?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterVersionResponseModel> GetRegisterVersion(string querystring = null);

        [WebApiPost("SetNewVersion?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<CurrentVersionResponseModel> SetNewVersion(string querystring = null);

        #endregion

        #region Default Generate

        [WebApi("Get/{key}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<T> Get(string key, string querystring = null);

        [WebApi("GetList?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<T>> GetList(string querystring = null);

        [WebApi("Exists/{key}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<bool> Exists(string key, string querystring = null);

        [WebApiPost("Register?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> Register(T requestModel, string querystring = null);

        [WebApiPost("RegisterList?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<string>> RegisterList(List<T> requestModel, string querystring = null);

        [WebApiDelete("Delete/{key}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string key, string querystring = null);

        [WebApiDelete("DeleteAll?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteAll(string querystring = null);

        [WebApiPatch("Update/{key}?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel Update(string key, T requestModel, string querystring = null);

        #endregion
    }

    public class CommonResource<T> : Resource
    {
        public CommonResource()
        {
            ModelType = typeof(T);
        }

        public CommonResource(string url)
            : base(url)
        {
            ModelType = typeof(T);
        }

        public CommonResource(IServerEnvironment env)
             : base(env)
        {
            ModelType = typeof(T);
        }

        public WebApiRequestModel<GetCountResponseModel> GetCount(string querystring = null) => null;

        public WebApiRequestModel<List<T>> OData(string querystring = null) => null;

        public WebApiRequestModel ODataDelete(string querystring = null) => null;

        public WebApiRequestModel<List<RegisterResponseModel>> RegisterRawData(List<T> requestModel) => null;

        public WebApiRequestModel<List<T>> ODataRawData(string querystring = null) => null;

        public WebApiRequestModel<T> Get(string key, string querystring = null) => null;

        public WebApiRequestModel<List<T>> GetList(string querystring = null) => null;

        public WebApiRequestModel<bool> Exists(string key, string querystring = null) => null;

        public WebApiRequestModel<string> Register(T requestModel, string querystring = null) => null;

        public WebApiRequestModel<List<string>> RegisterList(List<T> requestModel, string querystring = null) => null;

        public WebApiRequestModel Delete(string key, string querystring = null) => null;

        public WebApiRequestModel DeleteAll(string querystring = null) => null;

        public WebApiRequestModel Update(string key, T requestModel, string querystring = null) => null;

        public WebApiRequestModel<CreateAttachFileResponseModel> CreateAttachFile(CreateAttachFileRequestModel data, string queryString = null) => null;

        public WebApiRequestModel UploadAttachFile(Stream data, string fileId, string queryString = null) => null;

        public WebApiRequestModel<ImageModel> GetAttachFile(string fileId, string querystring = null) => null;

        public WebApiRequestModel DeleteAttachFile(string fileId, string querystring = null) => null;

        public WebApiRequestModel<GetAttachFileResponseModel> GetAttachFileMeta(string fileId, string querystring = null) => null;

        public WebApiRequestModel<List<GetAttachFileResponseModel>> GetAttachFileMetaList(string querystring = null) => null;

        public WebApiRequestModel AdaptResourceSchema() => null;

        public WebApiRequestModel GetResourceSchema() => null;

        public WebApiRequestModel DriveOutDocument(string id, string querystring = null) => null;

        public WebApiRequestModel ReturnDocument(string id, string querystring = null) => null;

        public WebApiRequestModel HistoryThrowAway(string id, string querystring = null) => null;

        public WebApiRequestModel<List<GetDocumentVersionResponseModel>> GetDocumentVersion(string id, string querystring = null) => null;

        public WebApiRequestModel<T> GetDocumentHistory(string id, string querystring = null) => null;

        public WebApiRequestModel<T> GetDocumentHistoryWithVersionNo(string id, int versionNo) => null;

        public WebApiRequestModel<T> GetDocumentHistoryWithVersionGuid(string id, Guid versionGuid) => null;

        public WebApiRequestModel<List<GetAttachFileVersionResponseModel>> GetAttachFileDocumentVersion(string id, string querystring = null) => null;

        public WebApiRequestModel DriveOutAttachFileDocument(string id, string querystring = null) => null;

        public WebApiRequestModel ReturnAttachFileDocument(string id, string querystring = null) => null;

        public WebApiRequestModel GetAttachFileDocumentHistory(string id, string querystring = null) => null;

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionNo(string id, int versionNo) => null;

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionNoAndKey(string id, int versionNo, string key) => null;

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionGuid(string id, Guid versionGuid) => null;

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionGuidAndKey(string id, Guid versionGuid, string key) => null;

        public WebApiRequestModel<CurrentVersionResponseModel> GetCurrentVersion(string querystring = null) => null;

        public WebApiRequestModel<VersionInfoResponseModel> GetVersionInfo(string querystring = null) => null;

        public WebApiRequestModel<RegisterVersionResponseModel> CreateRegisterVersion(string querystring = null) => null;

        public WebApiRequestModel<VersionInfoResponseModel> CompleteRegisterVersion(string querystring = null) => null;

        public WebApiRequestModel<RegisterVersionResponseModel> GetRegisterVersion(string querystring = null) => null;

        public WebApiRequestModel<CurrentVersionResponseModel> SetNewVersion(string querystring = null) => null;
    }

    public class CommonResourceInheritence<T> : Resource
    {
        public CommonResourceInheritence()
        {
        }

        public CommonResourceInheritence(string url)
            : base(url)
        {
        }

        public CommonResourceInheritence(IServerEnvironment env)
             : base(env)
        {
        }

        public WebApiRequestModel<GetCountResponseModel> GetCount(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<GetCountResponseModel>>();

        public WebApiRequestModel<List<T>> OData(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<List<T>>>();

        public WebApiRequestModel ODataDelete(string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel<List<RegisterResponseModel>> RegisterRawData(List<T> requestModel) => MakeApiRequestModel<WebApiRequestModel<List<RegisterResponseModel>>>();

        public WebApiRequestModel<List<T>> ODataRawData(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<List<T>>>();

        public WebApiRequestModel<T> Get(string key, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<T>>();

        public WebApiRequestModel<List<T>> GetList(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<List<T>>>();

        public WebApiRequestModel<bool> Exists(string key, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<bool>>();

        public WebApiRequestModel<string> Register(T requestModel, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<string>>();

        public WebApiRequestModel<List<string>> RegisterList(List<T> requestModel, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<List<string>>>();

        public WebApiRequestModel Delete(string key, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel DeleteAll(string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel Update(string key, T reuestModel, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel<CreateAttachFileResponseModel> CreateAttachFile(CreateAttachFileRequestModel data, string queryString = null) => MakeApiRequestModel<WebApiRequestModel<CreateAttachFileResponseModel>>();

        public WebApiRequestModel UploadAttachFile(Stream data, string fileId, string queryString = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel<ImageModel> GetAttachFile(string fileId, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<ImageModel>>();

        public WebApiRequestModel DeleteAttachFile(string fileId, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel<GetAttachFileResponseModel> GetAttachFileMeta(string fileId, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<GetAttachFileResponseModel>>();

        public WebApiRequestModel<List<GetAttachFileResponseModel>> GetAttachFileMetaList(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<List<GetAttachFileResponseModel>>>();

        public WebApiRequestModel AdaptResourceSchema() => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel GetResourceSchema() => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel DriveOutDocument(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel ReturnDocument(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel HistoryThrowAway(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel<List<GetDocumentVersionResponseModel>> GetDocumentVersion(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<List<GetDocumentVersionResponseModel>>>();

        public WebApiRequestModel<T> GetDocumentHistory(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<T>>();

        public WebApiRequestModel<T> GetDocumentHistoryWithVersionNo(string id, int versionNo) => MakeApiRequestModel<WebApiRequestModel<T>>();

        public WebApiRequestModel<T> GetDocumentHistoryWithVersionGuid(string id, Guid versionGuid) => MakeApiRequestModel<WebApiRequestModel<T>>();

        public WebApiRequestModel<List<GetAttachFileVersionResponseModel>> GetAttachFileDocumentVersion(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel<List<GetAttachFileVersionResponseModel>>>();

        public WebApiRequestModel DriveOutAttachFileDocument(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel ReturnAttachFileDocument(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel GetAttachFileDocumentHistory(string id, string querystring = null) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionNo(string id, int versionNo) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionNoAndKey(string id, int versionNo, string key) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionGuid(string id, Guid versionGuid) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel GetAttachFileDocumentHistoryWithVersionGuidAndKey(string id, Guid versionGuid, string key) => MakeApiRequestModel<WebApiRequestModel>();

        public WebApiRequestModel<CurrentVersionResponseModel> GetCurrentVersion(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<CurrentVersionResponseModel>>();

        public WebApiRequestModel<VersionInfoResponseModel> GetVersionInfo(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<VersionInfoResponseModel>>();

        public WebApiRequestModel<RegisterVersionResponseModel> CreateRegisterVersion(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<RegisterVersionResponseModel>>();

        public WebApiRequestModel<VersionInfoResponseModel> CompleteRegisterVersion(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<VersionInfoResponseModel>>();

        public WebApiRequestModel<RegisterVersionResponseModel> GetRegisterVersion(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<RegisterVersionResponseModel>>();

        public WebApiRequestModel<CurrentVersionResponseModel> SetNewVersion(string querystring = null) => MakeApiRequestModel<WebApiRequestModel<CurrentVersionResponseModel>>();
    }
}

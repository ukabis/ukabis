using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    internal interface IDyanamicApiDataStoreRepository
    {
        Action<JToken> CallbackDelete { get; set; }

        RepositoryInfo RepositoryInfo { get; set; }

        IResourceVersionRepository ResourceVersionRepository { get; set; }

        IDocumentVersionRepository DocumentVersionRepository { get; set; }

        INewDynamicApiDataStoreRepository NewDynamicApiDataStoreRepository { get; set; }

        JsonSearchResult GetDataById(IDynamicApiAction action, UrlParameter keyValue, Identification identification);

        JsonSearchResult GetData(IDynamicApiAction action, HasSingleData hasSingleData);

        IEnumerable<string> GetDataEnumerable(IDynamicApiAction action);

        List<JsonDocument> GetDataByOData(IDynamicApiAction action, out XResponseContinuation xResponseContinuation);

        bool IsIdValid(JToken idProperty, IDynamicApiAction action, JToken json, out DocumentDataId id);

        DocumentDataId GetId(IDynamicApiAction action, JToken json);

        string RegistData(IDynamicApiAction action, JsonDocument registData);

        IEnumerable<string> DeleteData(IDynamicApiAction action);

        (bool, List<JsonDocument>) DeleteDataByOData(IDynamicApiAction action, string odataquery);

        string CreateCacheKey(IDynamicApiAction action);

        //以降COSMOSDB独自の　IF　DataLakeStore　では　実装不要

        ResourceVersion GetMaxVersion(RepositoryKey repositoryKey, RepositoryInfo repositoryInfo);

        ResourceVersion GetCurrentVersion(RepositoryKey repositoryKey, RepositoryInfo repositoryInfo);

        ResourceVersion AddNewVersion(RepositoryKey repositoryKey, RepositoryInfo repositoryInfo);

        void RefreshVersion(RepositoryKey repositoryKey, RepositoryInfo repositoryInfo);

        ResourceVersion CreateRegisterVersion(RepositoryKey repositoryKey, RepositoryInfo repositoryInfo);

        ResourceVersion CompleteRegisterVersion(RepositoryKey repositoryKey, RepositoryInfo repositoryInfo);

        Uri CopyFile(string destBlobName, string destContainerName, string srcBlobName, string srcContainerName);
    }
}
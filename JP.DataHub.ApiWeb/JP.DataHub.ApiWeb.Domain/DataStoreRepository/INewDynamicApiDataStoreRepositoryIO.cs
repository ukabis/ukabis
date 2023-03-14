using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    // .NET6
    internal interface INewDynamicApiDataStoreRepositoryIO
    {
        bool CanOptimisticConcurrency { get; }
        bool CanQuery { get; }
        bool CanVersionControl { get; }
        bool CanQueryAttachFileMetaByOData { get; }
        ODataPatchSupport ODataPatchSupport { get; }

        string VersionInfoQuery { get; }
        string RepositoryName { get; }
        RepositoryInfo RepositoryInfo { get; set; }
        RepositoryKeyInfo RepositoryKeyInfo { get; }
        T QueryOnce<T>(QueryParam param);
        JsonDocument QueryOnce(QueryParam param);
        IEnumerable<JsonDocument> QueryEnumerable(QueryParam param);
        IList<JsonDocument> Query(QueryParam param, out XResponseContinuation XResponseContinuation);
        RegisterOnceResult RegisterOnce(RegisterParam param);
        void DeleteOnce(DeleteParam param);
        IEnumerable<string> Delete(DeleteParam param);
        int ODataPatch(QueryParam queryParam, JToken updateData);
    }
}

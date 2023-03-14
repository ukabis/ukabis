using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    internal interface IKeyManagementRepository
    {
        string GetCacheKey(QueryParam param, IPerRequestDataContainer PerRequestDataContainer, IResourceVersionRepository resourceVersionRepository);

        Dictionary<string, object> GetGenerateKey(QueryParam param, StringBuilder conditionString, IResourceVersionRepository resourceVersionRepository, RepositoryKey repositoryKey = null);

        bool IsIdValid(JToken idProperty, RegisterParam registerParam, IResourceVersionRepository resourceVersionRepository, out DocumentDataId id);

        DocumentDataId GetId(RegisterParam registerParam, IResourceVersionRepository resourceVersionRepository, IPerRequestDataContainer perRequestDataContainer);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class QueueStoreKeyManagement : IKeyManagement
    {
        public RepositoryInfo RepositoryInfo { get; set; }

        public string GetCacheKey(QueryParam param, IPerRequestDataContainer PerRequestDataContainer, IResourceVersionRepository resourceVersionRepository)
        {
            return null;
        }

        public DocumentDbId CreateDocumentDbId(QueryParam param)
        {
            return null;
        }

        public Dictionary<string, object> GetGenerateKey(QueryParam param, StringBuilder conditionString, IResourceVersionRepository resourceVersionRepository, RepositoryKey repositoryKey = null)
        {
            var retdic = new Dictionary<string, object>();
            return retdic;
        }

        public bool IsIdValid(JToken idProperty, RegisterParam registerParam, IResourceVersionRepository resourceVersionRepository, out DocumentDataId id)
        {
            id = null;
            return false;
        }

        public DocumentDataId GetId(RegisterParam registerParam, IResourceVersionRepository resourceVersionRepository, IPerRequestDataContainer perRequestDataContainer)
        {
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    internal interface INewDynamicApiDataStoreRepository : INewDynamicApiDataStoreRepositoryIO
    {
        IResourceVersionRepository ResourceVersionRepository { get; set; }
        IDocumentVersionRepository DocumentVersionRepository { get; set; }
        IPerRequestDataContainer PerRequestDataContainer { get; }
        IKeyManagement KeyManagement { get; }
        string DocumentVersionQuery { get; }
        IEnumerable<string> AttachFileMetaManagementFields { get; }
        string GetInternalAddWhereString(QueryParam param, out IDictionary<string, object> parameters);
    }
}

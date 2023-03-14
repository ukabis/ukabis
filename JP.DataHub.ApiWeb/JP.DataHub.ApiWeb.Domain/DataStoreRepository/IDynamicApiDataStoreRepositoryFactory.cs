using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    // .NET6
    internal interface IDynamicApiDataStoreRepositoryFactory
    {
        IDyanamicApiDataStoreRepository Restore(string repositoryType, RepositoryInfo repositoryInfo);
        INewDynamicApiDataStoreRepository NewDataStoreRestore(RepositoryType repositoryType, RepositoryInfo repositoryInfo, IsEnableResourceVersion isEnableResourceVersion);
    }
}

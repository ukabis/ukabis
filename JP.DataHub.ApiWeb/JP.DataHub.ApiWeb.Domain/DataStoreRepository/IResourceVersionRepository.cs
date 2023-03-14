using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    internal interface IResourceVersionRepository
    {
        INewDynamicApiDataStoreRepository PhysicalRepository { get; set; }

        VendorId VendorId { get; set; }
        SystemId SystemId { get; set; }
        OpenId OpenId { get; set; }
        RepositoryKey RepositoryKey { get; set; }
        IsAutomaticId IsAutomaticId { get; set; }

        ResourceVersion AddNewVersion(RepositoryKey repositoryKey);
        ResourceVersion GetMaxVersion(RepositoryKey repositoryKey);
        ResourceVersion GetRegisterVersion(RepositoryKey repositoryKey, XVersion xversion);
        ResourceVersion GetCurrentVersion(RepositoryKey repositoryKey);
        string GetVersionInfo(RepositoryKey repositoryKey);
        void RefreshVersion(RepositoryKey repositoryKey);
        ResourceVersion CreateRegisterVersion(RepositoryKey repositoryKey);
        ResourceVersion CompleteRegisterVersion(RepositoryKey repositoryKey);
    }
}

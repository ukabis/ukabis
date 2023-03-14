using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    /// <summary>
    /// リソースのバージョンを使用しない場合のRepository
    /// </summary>
    internal class DisabledResourceVersionRepository : IResourceVersionRepository
    {
        const int STABLEVERSIONNO = 1;

        public INewDynamicApiDataStoreRepository PhysicalRepository { get; set; }

        public VendorId VendorId { get; set; }
        public SystemId SystemId { get; set; }
        public OpenId OpenId { get; set; }
        public RepositoryKey RepositoryKey { get; set; }
        public IsAutomaticId IsAutomaticId { get; set; }

        public ResourceVersion AddNewVersion(RepositoryKey repositoryKey) => new ResourceVersion(STABLEVERSIONNO);

        public ResourceVersion GetMaxVersion(RepositoryKey repositoryKey) => new ResourceVersion(STABLEVERSIONNO);

        public ResourceVersion GetRegisterVersion(RepositoryKey repositoryKey, XVersion xversion) => new ResourceVersion(STABLEVERSIONNO);

        public ResourceVersion GetCurrentVersion(RepositoryKey repositoryKey) => new ResourceVersion(STABLEVERSIONNO);

        public string GetVersionInfo(RepositoryKey repositoryKey)
        {
            throw new NotImplementedException();
        }

        public void RefreshVersion(RepositoryKey repositoryKey)
        {
            throw new NotImplementedException();
        }

        public ResourceVersion CreateRegisterVersion(RepositoryKey repositoryKey)
        {
            throw new NotImplementedException();
        }

        public ResourceVersion CompleteRegisterVersion(RepositoryKey repositoryKey)
        {
            throw new NotImplementedException();
        }
    }
}

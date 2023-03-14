using AutoMapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    /// <summary>
    /// IDyanamicApiDataStoreRepository用リポジトリー
    /// </summary>
    internal class DynamicApiDataStoreRepositoryFactory : IDynamicApiDataStoreRepositoryFactory
    {
        private static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IMethod, IResourceVersionRepository>()
                    .ForMember(x => x.PhysicalRepository, opt => opt.Ignore())
                    .AfterMap((s, d) =>
                    {
                    });
            });
            return mappingConfig.CreateMapper();
        });

        private IMapper Mapper { get => _Mapper.Value; }

        public IDyanamicApiDataStoreRepository Restore(string repositoryType, RepositoryInfo repositoryInfo)
        {
            var dyanamicApiDataStoreRepository = UnityCore.Resolve<IDyanamicApiDataStoreRepository>(repositoryType);
            if (dyanamicApiDataStoreRepository != null)
            {
                dyanamicApiDataStoreRepository.RepositoryInfo = repositoryInfo;

                // ResourceVersion
                dyanamicApiDataStoreRepository.ResourceVersionRepository = UnityCore.Resolve<IResourceVersionRepository>();
                INewDynamicApiDataStoreRepository physicalRepository = null;
                try
                {
                    physicalRepository = UnityCore.Resolve<INewDynamicApiDataStoreRepository>(repositoryType);
                }
                catch (Exception)
                {
                    // repositoryTypeに合致したものが作られない場合はそのまま（いずれ実装するので）
                    // nothing...
                }
                if (physicalRepository != null && physicalRepository?.CanQuery == true)
                {
                    physicalRepository.RepositoryInfo = repositoryInfo;
                }
                else
                {
                    // configからもらう
                    //physicalRepository = InfrastructureUnityContainer.Resolve<INewDyanamicApiDataStoreRepository>("ddb");
                    //physicalRepository.RepositoryInfo = configから
                }
                dyanamicApiDataStoreRepository.ResourceVersionRepository.PhysicalRepository = physicalRepository;

                // DocumentVersionを管理するためのDataStoreRepository
                dyanamicApiDataStoreRepository.DocumentVersionRepository = UnityCore.Resolve<IDocumentVersionRepository>();
                if (dyanamicApiDataStoreRepository.DocumentVersionRepository != null)
                {
                    // 物理RepositoryはResourceVersionと同じものでよい
                    dyanamicApiDataStoreRepository.DocumentVersionRepository.PhysicalRepository = physicalRepository;
                }

                // DocumentVersionの退避用DataStoreRepository
                //if (historyEvacuationRepositoryInfo != null)
                //{
                //    var xxx = InfrastructureUnityContainer.Resolve<INewDyanamicApiDataStoreRepository>("blb"/*履歴の退避用はBlob固定の為*/);
                //    if (xxx != null)
                //    {
                //        xxx.RepositoryInfo = repositoryInfo;
                //    }
                //}
                dyanamicApiDataStoreRepository.NewDynamicApiDataStoreRepository = UnityCore.Resolve<INewDynamicApiDataStoreRepository>(repositoryType);
                dyanamicApiDataStoreRepository.NewDynamicApiDataStoreRepository.RepositoryInfo = repositoryInfo;
                dyanamicApiDataStoreRepository.NewDynamicApiDataStoreRepository.ResourceVersionRepository = dyanamicApiDataStoreRepository.ResourceVersionRepository;
            }
            return dyanamicApiDataStoreRepository;
        }

        public INewDynamicApiDataStoreRepository NewDataStoreRestore(RepositoryType repositoryType, RepositoryInfo repositoryInfo, IsEnableResourceVersion isEnableResourceVersion)
        {
            if (repositoryInfo == null)
            {
                return null;
            }

            var result = UnityCore.Resolve<INewDynamicApiDataStoreRepository>(repositoryType.ToCode());
            if (result != null)
            {
                result.RepositoryInfo = repositoryInfo;

                var canVersionControl = (isEnableResourceVersion?.Value == true && result.CanVersionControl).ToString();
                result.ResourceVersionRepository = UnityCore.Resolve<IResourceVersionRepository>(canVersionControl);

                INewDynamicApiDataStoreRepository physicalRepository = null;
                try
                {
                    physicalRepository = UnityCore.Resolve<INewDynamicApiDataStoreRepository>(repositoryType.ToCode());
                }
                catch (Exception)
                {
                    // repositoryTypeに合致したものが作られない場合はそのまま（いずれ実装するので）
                    // nothing...
                }
                if (physicalRepository != null && physicalRepository?.CanQuery == true)
                {
                    physicalRepository.RepositoryInfo = repositoryInfo;
                }
                else
                {
                    // configからもらう
                    //physicalRepository = InfrastructureUnityContainer.Resolve<INewDyanamicApiDataStoreRepository>("ddb");
                    //physicalRepository.RepositoryInfo = configから
                }
                result.ResourceVersionRepository.PhysicalRepository = physicalRepository;

                // DocumentVersionを管理するためのDataStoreRepository
                result.DocumentVersionRepository = UnityCore.Resolve<IDocumentVersionRepository>();
                if (result.DocumentVersionRepository != null)
                {
                    // 物理RepositoryはResourceVersionと同じものでよい
                    result.DocumentVersionRepository.PhysicalRepository = physicalRepository;
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataRepository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure
{
    internal static class RepositoryTypeInitializer
    {
        internal class RepositoryTypeMap
        {
            public RepositoryType RepositoryType { get; set; }
            public Type RepositortyClass { get; set; }
            public Type RdbmsRepositoryClass { get; set; }
            public Type KeyManagementClass { get; set; }
            public Type ApiQueryCompiler { get; set; }
            public RepositoryTypeMap(RepositoryType repositoryType, Type repositortyClass = null, Type rdbmsRepositoryClass = null, Type keyManagementClass = null, Type apiQueryCompiler = null)
            {
                RepositoryType = repositoryType;
                RepositortyClass = repositortyClass;
                RdbmsRepositoryClass = rdbmsRepositoryClass;
                KeyManagementClass = keyManagementClass;
                ApiQueryCompiler = apiQueryCompiler;
            }
        }

        private static List<RepositoryTypeMap> _map = new List<RepositoryTypeMap>()
        {
            new RepositoryTypeMap(RepositoryType.Unknown) { ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.SQLServer2) { RepositortyClass = typeof(SqlServerDataStoreRepository), RdbmsRepositoryClass = typeof(SqlServerDataStoreRepository), KeyManagementClass = typeof(SqlServerKeyManagement), ApiQueryCompiler = typeof(SqlServerApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.OracleDb) { RepositortyClass = typeof(OracleDbDataStoreRepository), RdbmsRepositoryClass = typeof(OracleDbDataStoreRepository), KeyManagementClass = typeof(OracleDbKeyManagement), ApiQueryCompiler = typeof(OraclDbeApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.BlobFileSharding) { ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.CosmosDB) { RepositortyClass = typeof(NewCosmosDbDataStoreRepository), KeyManagementClass = typeof(CosmonsDBKeyManagement), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.BlobFile) { RepositortyClass = typeof(NewBlobDataStoreRepository), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.DataLake) { ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.QueueStorage) { RepositortyClass = typeof(NewQueueStoreRepository), KeyManagementClass = typeof(QueueStoreKeyManagement), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.EventHub) { RepositortyClass = typeof(NewEventHubStoreRepository), KeyManagementClass = typeof(EventHubStoreKeyManagement), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.AllEventHub) { ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.Fiware) { ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.AttachFileBlob) { RepositortyClass = typeof(OracleDynamicApiAttachFileRepository), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.AttachFileMetaCosmosDb) {  RepositortyClass = typeof(NewCosmosDbDataStoreRepository), KeyManagementClass = typeof(CosmonsDBKeyManagement), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.AttachFileMetaSqlServer) { RepositortyClass = typeof(SqlServerDataStoreRepository), RdbmsRepositoryClass = typeof(SqlServerDataStoreRepository), KeyManagementClass = typeof(SqlServerKeyManagement), ApiQueryCompiler = typeof(SqlServerApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.BlockchainNode) { ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.DocumentHistoryStorage) { RepositortyClass = typeof(OracleDocumentHistoryDataStoreRepository), KeyManagementClass = typeof(OracleDbKeyManagement), ApiQueryCompiler = typeof(OraclDbeApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.MomgoDB) { RepositortyClass = typeof(NewMongoDbDataStoreRepository), KeyManagementClass = typeof(MongoDBKeyManagement), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
            new RepositoryTypeMap(RepositoryType.TimeSeriesInsights) { RepositortyClass = typeof(TimeSeriesInsightsDataStoreRepository), KeyManagementClass = typeof(TimeSeriesInsightsKeyManagement), ApiQueryCompiler = typeof(DefaultApiQueryCompiler) },
        };

        public static void RegisterRepositoryType(this IUnityContainer container)
        {
            _map.Where(x => x.RepositortyClass != null).ToList().ForEach(x => container.RegisterType(typeof(INewDynamicApiDataStoreRepository), x.RepositortyClass, x.RepositoryType.ToCode(), new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>()));
            _map.Where(x => x.RdbmsRepositoryClass != null).ToList().ForEach(x => container.RegisterType(typeof(INewDynamicApiDataStoreRdbmsRepository), x.RepositortyClass, x.RepositoryType.ToCode(), new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>()));
            _map.Where(x => x.KeyManagementClass != null).ToList().ForEach(x => container.RegisterType(typeof(IKeyManagement), x.KeyManagementClass, x.RepositoryType.ToCode(), new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>()));
            _map.Where(x => x.ApiQueryCompiler != null).ToList().ForEach(x => container.RegisterType(typeof(IApiQueryCompiler), x.ApiQueryCompiler, x.RepositoryType.ToCode(), new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>()));

            // JPDataHubDbConnection生成時に渡すDB接続文字列が静的に指定できないためFactoryメソッドをコンストラクタで渡します。
            // このようにするとUTでIJPDataHubDbConnectionのMockを渡すことができるようになります。
            container.RegisterType<INewDynamicApiDataStoreRepository, SqlServerDataStoreRepositoryQueryOnly>(RepositoryType.SQLServer.ToCode(),
                new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(new Func<string, IJPDataHubDbConnection>((cs) => new JPDataHubDbConnection(cs, "System.Data.SqlClient")),
                new ResolvedParameter<IPerRequestDataContainer>(), new ResolvedParameter<IContainerDynamicSeparationRepository>()));

            container.RegisterType<IApiQueryCompiler, DefaultApiQueryCompiler>(RepositoryType.SQLServer.ToCode(), new PerResolveLifetimeManager(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
        }
    }
}

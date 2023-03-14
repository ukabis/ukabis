using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Serializer;
using JP.DataHub.Com.Transaction;
using JP.DataHub.MVC.Unity;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.Api.Core.Repository.Impl;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Scripting;
using JP.DataHub.ApiWeb.Infrastructure.Cache;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;
using JP.DataHub.ApiWeb.Infrastructure.Data.CosmosDb;
using JP.DataHub.ApiWeb.Infrastructure.Data.GraphApi;
using JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.ApiWeb.Infrastructure.ResourceSchemaAdapter;
using JP.DataHub.ApiWeb.Infrastructure.Sql;
using JP.DataHub.ApiWeb.Infrastructure.Scripting.Roslyn;
using JP.DataHub.ApiWeb.Infrastructure.WebRequest;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using JP.DataHub.Infrastructure.Database.Data.SqlServer;
using JP.DataHub.Infrastructure.Database.Data.OracleDb;
using JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage;
using JP.DataHub.ApiWeb.Infrastructure.Configuration;
using Newtonsoft.Json;

namespace JP.DataHub.ApiWeb.Infrastructure
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("ConnectionStrings");
            var appConfig = configuration.GetSection("AppConfig");

            // repository
            container.RegisterType<IDynamicApiRepository, DynamicApiRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAuthenticationRepository, AuthenticationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAsyncDyanamicApiRepository, AsyncDyanamicApiRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAsyncDyanamicApiRepository, AsyncDyanamicApiRepositoryOci>("AsyncDyanamicApiRepositoryOci", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IVendorSystemAccessTokenClientRepository, VendorSystemAccessTokenClientRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDynamicApiDataStoreRepositoryFactory, DynamicApiDataStoreRepositoryFactory>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDynamicGatewayRepository, DynamicGatewayRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IContainerDynamicSeparationRepository, ContainerDynamicSeparationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IWebRequestManager, WebRequestManager>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDocumentVersionRepository, DocumentVersionRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ILoggingFilterRepository, LoggingFilterRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAttachFileRepository, AttachFileRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAttachFileInformationRepository, AttachFileInformationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAsymmetricKeyRepository, AsymmetricKeyRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICommonKeyRepository, CommonKeyRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IMetadataInfoRepository, MetadataInfoRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceChangeEventHubStoreRepository, ResourceChangeEventHubStoreRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IClientCertificateRepository, ClientCertificateRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IPhysicalRepositoryGroupRepository, PhysicalRepositoryGroupRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IOpenIdUserRepository, OpenIdUserRepositoryForAzure>(
                new InjectionConstructor(new HttpClient(new GraphApiMessageHandler())),
                new Interceptor<InterfaceInterceptor>(), 
                new InterceptionBehavior<PolicyInjectionBehavior>());

            container.RegisterType<IExternalAttachFileRepository, ExternalAttachFileAzureBlobRepository>("az-blob", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            container.RegisterType<IResourceVersionRepository, ResourceVersionRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceVersionRepository, ResourceVersionRepository>(bool.TrueString, new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceVersionRepository, DisabledResourceVersionRepository>(bool.FalseString, new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            container.RegisterType<ITermsRepository, TermsRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserTermsRepository, UserTermsRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICertifiedApplicationRepository, CertifiedApplicationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserGroupRepository, UserGroupRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserResourceShareRepository, UserResourceShareRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IRevokeRepository, RevokeRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            container.RegisterType<IResourceSharingPersonRepository, ResourceSharingPersonRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            // database
            if (UnityCore.IsRegistered<IConnectionStrings>() == true)
            {
                var list = UnityCore.Resolve<IConnectionStrings>();
                list?.ForEach(x =>
                {
                    container.RegisterType<IJPDataHubDbConnection, JPDataHubDbConnection>(x.Name, list.LifeTimeManager.CreateInstanseLifeTimeManager(),
                        new InjectionConstructor(new JPDataHubDbConnectionParam() { ConnectionString = x.ConnectionString, ProviderName = x.Provider, IsMultithread = false, IsTransactionManage = list.IsTransactionManage, IsTransactionScope = list.IsTransactionScope, Options = x.Options }),
                        new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                    container.RegisterType<IJPDataHubDbConnection, JPDataHubDbConnection>(x.Name + "-Multithread", new PerThreadLifetimeManager(),
                        new InjectionConstructor(new JPDataHubDbConnectionParam() { ConnectionString = x.ConnectionString, ProviderName = x.Provider, IsMultithread = true, IsTransactionManage = list.IsTransactionManage, IsTransactionScope = list.IsTransactionScope, Options = x.Options }),
                        new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                });
            }

            // キャッシュビルドアップ
            container.CacheBiludup(configuration);
            container.RegisterType<ISerializer, ComMessagePackSerializer>(new PerResolveLifetimeManager());
            container.RegisterType<ISerializer, ComMessagePackSerializer>("InMemoryCache", new PerResolveLifetimeManager());
            container.RegisterType<IBlobCache, BlobCache>(new PerRequestLifetimeManager(), new InjectionConstructor("BlobCache"));
            container.RegisterType<IBlobCache, BlobCache>("AzureRedisCacheAccessToken", new PerRequestLifetimeManager());
            container.RegisterType<IBlobCache, BlobCache>("AzureRedisCacheCsvDownload", new PerRequestLifetimeManager());
            // イベント通知
            // EventNotifyBiludup(container, configuration);
            // Storage
            // StorageBiludup(container, configuration);

            // DataStore
            container.RegisterType<IResourceVersionRepository, ResourceVersionRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDocumentVersionRepository, DocumentVersionRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterRepositoryType();

            container.RegisterType<IServiceBusEventRepository, ServiceBusEventRepository>("DomainDataSync", new InjectionConstructor(connectionString.GetValue<string>("DomainDataSyncServiceBus"), appConfig.GetValue<bool>("UseServiceBusForDataSync", true)));
            container.RegisterType<IStreamingServiceEventRepository, StreamingServiceEventRepository>("DomainDataSyncOci",
                new InjectionConstructor(
                    configuration.GetValue<string>("OciCredential:ConfigurationFilePath"),
                    configuration.GetValue<string>("OciCredential:Profile"),
                    configuration.GetValue<string>("OciCredential:PemFilePath"),
                    configuration.GetValue<string>("DomainDataSyncStreamingService:Ocid"),
                    configuration.GetValue<string>("DomainDataSyncStreamingService:EndPoint"),
                    appConfig.GetValue<bool>("UseStreamingServiceForDataSync", true),
                    configuration.GetValue<string>("DomainDataSync:DomainDataSync")));

            container.RegisterType<IJPDataHubCosmosDb, JPDataHubCosmosDb>(new PerResolveLifetimeManager());
            container.RegisterType<IJPDataHubMongoDb, JPDataHubMongoDb>(new PerResolveLifetimeManager());
            container.RegisterType<IJPDataHubRdbms, JPDataHubSqlServer>("SqlServer", new PerResolveLifetimeManager());
            container.RegisterType<IJPDataHubRdbms, JPDataHubOracleDb>("Oracle", new PerResolveLifetimeManager());
            container.RegisterType<IJPDataHubTimeSeriesInsights, JPDataHubTimeSeriesInsights>(new PerResolveLifetimeManager());
            container.RegisterType<IJPDataHubHttpClientFactory, JPDataHubHttpClientFactory>(new PerResolveLifetimeManager());
            container.RegisterType<IJPDataHubEventHub, JPDataHubEventHub>(new PerResolveLifetimeManager());
            container.RegisterType<IJPDataHubEventHub, JPDataHubEventHub>("Logging", new PerResolveLifetimeManager(), new InjectionConstructor(connectionString.GetValue<string>("LoggingEventHub")));

            container.RegisterType<IEventHubStreamingService, EventHubStreamingService>(new PerResolveLifetimeManager());
            container.RegisterType<IEventHubStreamingService, EventHubStreamingService>("RevokeStreamingService", new PerResolveLifetimeManager(), new InjectionConstructor(
                configuration.GetValue<string>("OciCredential:ConfigurationFilePath"),
                configuration.GetValue<string>("OciCredential:Profile"),
                configuration.GetValue<string>("OciCredential:PemFilePath"),
                configuration.GetValue<string>("RevokeStreamingService:Ocid"),
                configuration.GetValue<string>("RevokeStreamingService:EndPoint")
            ));
            container.RegisterType<IEventHubStreamingService, EventHubStreamingService>("LoggingStreamingService", new PerResolveLifetimeManager(), new InjectionConstructor(
                configuration.GetValue<string>("OciCredential:ConfigurationFilePath"),
                configuration.GetValue<string>("OciCredential:Profile"),
                configuration.GetValue<string>("OciCredential:PemFilePath"),
                configuration.GetValue<string>("LoggingEventStreamingService:Ocid"),
                configuration.GetValue<string>("LoggingEventStreamingService:EndPoint")
            ));

            // config
            container.RegisterInstance<bool>("IsAuthenticationStaticCache", configuration.GetValue<bool>("StaticCache:IsAuthenticationStaticCache", false));
            container.RegisterInstance<bool>("IsDynamicApiStaticCache", configuration.GetValue<bool>("StaticCache:IsDynamicApiStaticCache", false));
            container.RegisterInstance<TimeSpan>("AuthenticationStaticCacheExpirationTimeSpan", configuration.GetValue<TimeSpan>("StaticCache:AuthenticationStaticCacheExpirationTimeSpan", new TimeSpan(24, 0, 0)));
            container.RegisterInstance<TimeSpan>("DynamicApiStaticCacheExpirationTimeSpan", configuration.GetValue<TimeSpan>("StaticCache:DynamicApiStaticCacheExpirationTimeSpan", new TimeSpan(24, 0, 0)));
            container.RegisterInstance<string>("StaticCacheClusterSyncTimeServer", configuration.GetValue<string>("StaticCache:ClusterSync:TimeServer", "off").ToLower());
            container.RegisterInstance<int>("StaticCacheClusterSyncCheckInterval", configuration.GetValue<int>("StaticCache:ClusterSync:CheckInterval", 60));

            // QuerySyntaxValidator
            container.RegisterType<IQuerySyntaxValidatorFactory, QuerySyntaxValidatorFactory>(new PerResolveLifetimeManager());
            container.RegisterType<IQuerySyntaxValidator, MongoDbQuerySyntaxValidatior>(RepositoryType.MomgoDB.ToCode(), new PerResolveLifetimeManager());

            // ODataSqlManager
            container.RegisterType<IODataSqlManager, CosmosDBODataSqlManager>("CosmosDB", new PerResolveLifetimeManager());
            container.RegisterType<IODataSqlManager, MongoDbODataSqlManager>("MongoDB", new PerResolveLifetimeManager());
            container.RegisterType<IMongoDbODataSqlManager, MongoDbODataSqlManager>(new PerResolveLifetimeManager());
            container.RegisterType<IODataSqlManager, SqlServerODataSqlManager>("SqlServer", new PerResolveLifetimeManager());
            container.RegisterType<IODataSqlManager, SqlServerObsoleteODataSqlManager>("SqlServerQueryOnly", new PerResolveLifetimeManager());
            container.RegisterType<IODataSqlManager, OracleDbODataSqlManager>("Oracle", new PerResolveLifetimeManager());

            // SqlBuilder
            container.RegisterType<IQuerySqlBuilder, SqlServerQuerySqlBuilder>("SqlServer", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<QueryParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IResourceVersionRepository>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null),
                    new InjectionParameter<IDynamicApiRepository>(null),
                    false));
            container.RegisterType<IUpsertSqlBuilder, SqlServerUpsertSqlBuilder>("SqlServer", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<RegisterParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null)));
            container.RegisterType<IDeleteSqlBuilder, SqlServerDeleteSqlBuilder>("SqlServer", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<DeleteParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null)));
            container.RegisterType<IODataPatchSqlBuilder, SqlServerODataPatchSqlBuilder>("SqlServer", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<QueryParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null),
                    new InjectionParameter<JToken>(null)));

            // SqlBuilder
            container.RegisterType<IQuerySqlBuilder, OracleDbQuerySqlBuilder>("Oracle", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<QueryParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IResourceVersionRepository>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null),
                    new InjectionParameter<IDynamicApiRepository>(null),
                    false));
            container.RegisterType<IUpsertSqlBuilder, OracleDbUpsertSqlBuilder>("Oracle", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<RegisterParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null)));
            container.RegisterType<IDeleteSqlBuilder, OracleDbDeleteSqlBuilder>("Oracle", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<DeleteParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null)));
            container.RegisterType<IODataPatchSqlBuilder, OracleDbODataPatchSqlBuilder>("Oracle", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<QueryParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null),
                    new InjectionParameter<JToken>(null)));

            // Script
            container.RegisterType<IScriptingExecuter, CSharpScriptingExecuter>(ScriptType.RoslynScript.ToCode(), new PerResolveLifetimeManager());
            container.RegisterType<IBlobStorage, ScriptRuntimeLogFileBlobStorage>("ScriptRuntimeFileBlobStorage", new PerResolveLifetimeManager(), new InjectionConstructor(configuration.GetValue<string>("ConnectionStrings:ScriptRuntimeLogBlobStorageConnectionStrings")));
            container.RegisterType<IScriptRuntimeLogMetaDataRepository, ScriptRuntimeLogMetaDataRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IScriptRuntimeLogFileRepository, ScriptRuntimeLogFileRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            // AttachFile(Static)
            container.RegisterType<IBlobStorage, AttachFileBlobStorage>("AttachFileBlobStorage", new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(new InjectionParameter<string>(null)));

            // Async
            container.RegisterType<IBlobStorage, ScriptRuntimeLogFileBlobStorage>("AsyncDynamicApiBlobStorage", new PerResolveLifetimeManager(), new InjectionConstructor(configuration.GetValue<string>("ConnectionStrings:AsyncDynamicApiStorageConnectionStrings")));
            container.RegisterType<IOciObjectStorage, OciObjectStorage>("AsyncDynamicApiObjectStorage", new PerResolveLifetimeManager(), new InjectionConstructor(
                JsonConvert.DeserializeObject<OciStorageSetting>(configuration.GetValue<String>("ConnectionStrings:AsyncDynamicApiStorageConnectionStringsOci"))));

            // ResourceSchemaAdapter
            container.RegisterType<IResourceSchemaAdapter, DefaultResourceSchemaAdapter>(new PerResolveLifetimeManager());
            container.RegisterType<IResourceSchemaAdapter, SqlServerResourceSchemaAdapter>(RepositoryType.SQLServer2.ToCode(), new PerResolveLifetimeManager(),
                new InjectionConstructor(
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<ControllerId>(null),
                    new InjectionParameter<DataSchema>(null)));
            container.RegisterType<IResourceSchemaAdapter, OracleDbResourceSchemaAdapter>(RepositoryType.OracleDb.ToCode(), new PerResolveLifetimeManager(),
                new InjectionConstructor(
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<ControllerId>(null),
                    new InjectionParameter<DataSchema>(null)));

            // 同意・認可関係
            container.RegisterType<INotificationService, NotificationService>("Revoke", new PerResolveLifetimeManager(), new InjectionConstructor(configuration.GetValue<string>("ConnectionStrings:Revoke")));
            container.RegisterType<ITermsRepository, TermsRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserTermsRepository, UserTermsRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICertifiedApplicationRepository, CertifiedApplicationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserGroupRepository, UserGroupRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserResourceShareRepository, UserResourceShareRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceGroupRepository, ResourceGroupRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
        }
    }
}
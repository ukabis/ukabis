using System.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Serializer;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Api.Core.Authentication;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.Api.Core.Repository.Impl;
using JP.DataHub.ManageApi.Core.DataContainer;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.ManageApi.Infrastructure.Repository;
using JP.DataHub.ManageApi.Service.CharacterLimit;
using JP.DataHub.Infrastructure.Database.Data;
using SendGrid;

namespace JP.DataHub.ManageApi.Infrastructure
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("ConnectionStrings");
            var appConfig = configuration.GetSection("AppConfig");

            // キャッシュビルドアップ
            container.CacheBiludup(configuration);
            container.RegisterType<ISerializer, ComMessagePackSerializer>(new PerResolveLifetimeManager());
            container.RegisterType<ISerializer, ComMessagePackSerializer>("InMemoryCache", new PerResolveLifetimeManager());
            container.RegisterType<IAccessTokenCache, AccessTokenCache>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            // Repository
            container.RegisterType<IAdminInfoRepository, AdminInfoRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDataSchemaRepository, DataSchemaRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDynamicApiRepository, DynamicApiRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IRepositoryGroupRepository, RepositoryGroupRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IRoleRepository, RoleRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IVendorAuthenticationRepository, VendorAuthenticationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IPhysicalRepositoryGroupRepository, PhysicalRepositoryGroupRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ILoggingFilterRepository, LoggingFilterRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserRoleCheckRepository, UserRoleCheckRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAgreementRepository, AgreementRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ITestRepository, TestRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDocumentRepository, DocumentRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IVendorRepository, VendorRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ISendMailRepository, SendMailRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAttachFileRepository, AttachFileRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IInformationRepository, InformationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserInvitationRepository, UserInvitationRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IMailTemplateRepository, MailTemplateRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IApiMailTemplateRepository, ApiMailTemplateRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ISystemRepository, SystemRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICommonIpFilterRepository, CommonIpFilterRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IApiCoreSystemRepository, ApiCoreSystemRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ILoggingRepository, LoggingRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceSharingRepository, ResourceSharingRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceSharingPersonRepository, ResourceSharingPersonRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IApiWebhookRepository, ApiWebhookRepository>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            container.RegisterType<ICharacterLimit, CharacterLimit.CharacterLimit>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IQuerySyntaxValidatorFactory, QuerySyntaxValidatorFactory>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

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

            container.RegisterType<IServiceBusEventRepository, ServiceBusEventRepository>("Trail", new InjectionConstructor(connectionString.GetValue<string>("TrailServiceBus"), appConfig.GetValue<bool>("UseServiceBusForTrail", true)));
            container.RegisterType<IStreamingServiceEventRepository, StreamingServiceEventRepository>("TrailOci",
                                                                                                        new InjectionConstructor(
                                                                                                            configuration.GetValue<string>("OciCredential:ConfigurationFilePath"),
                                                                                                            configuration.GetValue<string>("OciCredential:Profile"),
                                                                                                            configuration.GetValue<string>("OciCredential:PemFilePath"),
                                                                                                            configuration.GetValue<string>("TrailStreamingService:Trail:Ocid"),
                                                                                                            configuration.GetValue<string>("TrailStreamingService:Trail:EndPoint"),
                                                                                                            appConfig.GetValue<bool>("UseStreamingServiceForTrail", true),
                                                                                                            configuration.GetValue<string>("TrailEventProcess:Trail")));

            container.RegisterType<ISendGridClient, SendGridClient>(new PerResolveLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<string>(),
                    new OptionalParameter<string>(),
                    new OptionalParameter<Dictionary<string, string>>(),
                    new OptionalParameter<string>(),
                    new OptionalParameter<string>()));

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

            // eventHub
            container.RegisterType<IJPDataHubEventHub, JPDataHubEventHub>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IJPDataHubEventHub, JPDataHubEventHub>("Logging", new InjectionConstructor(connectionString.GetValue<string>("LoggingEventHub")), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            container.RegisterType<IEventHubStreamingService, EventHubStreamingService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IEventHubStreamingService, EventHubStreamingService>("LoggingStreamingService", new InjectionConstructor(
                configuration.GetValue<string>("OciCredential:ConfigurationFilePath"),
                configuration.GetValue<string>("OciCredential:Profile"),
                configuration.GetValue<string>("OciCredential:PemFilePath"),
                configuration.GetValue<string>("LoggingEventStreamingService:Ocid"),
                configuration.GetValue<string>("LoggingEventStreamingService:EndPoint")
                ), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
        }
    }
}

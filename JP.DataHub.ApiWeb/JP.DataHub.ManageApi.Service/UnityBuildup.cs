using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Api.Core.Service.Impl;
using JP.DataHub.ManageApi.Service.Impl;
using JP.DataHub.ManageApi.Service.DymamicApi.Scripting;

namespace JP.DataHub.ManageApi.Service
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.RegisterType<IAdminInfoService, AdminInfoService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDynamicApiService, DynamicApiService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IRepositoryGroupService, RepositoryGroupService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IRoleService, RoleService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IVendorAuthenticationService, VendorAuthenticationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IPhysicalRepositoryGroupService, PhysicalRepositoryGroupService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ILoggingFilterService, LoggingFilterService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserRoleCheckService, UserRoleCheckService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAgreementService, AgreementService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IApiDescriptionService, ApiDescriptionService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ITestService, TestService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IDocumentService, DocumentService>();
            container.RegisterType<IVendorService, VendorService>();
            container.RegisterType<IServiceBusEventService, ServiceBusEventService>("Trail");
            container.RegisterType<IStreamingServiceEventService, StreamingServiceEventService>("TrailOci");
            container.RegisterType<IInformationService, InformationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICacheService, CacheService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserInvitationService, UserInvitationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAttachFileService, AttachFileService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IMailTemplateApplicationService, MailTemplateApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ISystemService, SystemService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAuthenticationService, AuthenticationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ILoggingService, LoggingService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceSharingService, ResourceSharingService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceSharingPersonService, ResourceSharingPersonService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IApiWebhookService, ApiWebhookService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            // Script
            container.RegisterType<IScriptingCompiler, CSharpScriptingCompiler>(ScriptType.RoslynScript.ToCode(), new PerResolveLifetimeManager());

            // MultiLanguage
            var supportedCultures = (string.IsNullOrWhiteSpace(configuration.GetSection("AppConfig")?.GetValue<string>("SupportedCultures")) ? "ja" : configuration.GetSection("AppConfig")?.GetValue<string>("SupportedCultures"))
                .Split(new char[] { ';' })?.Where(x => !string.IsNullOrWhiteSpace(x));
            container.RegisterInstance<string[]>("SupportedCultures", supportedCultures.ToArray());
            container.RegisterInstance<string>("DefaultCulture", supportedCultures.FirstOrDefault());
        }
    }
}
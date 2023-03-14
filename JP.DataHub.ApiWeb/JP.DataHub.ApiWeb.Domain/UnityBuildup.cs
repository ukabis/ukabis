using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using JP.DataHub.Aop;
using JP.DataHub.Api.Core.Authentication;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.Api.Core.Service.Impl;
using JP.DataHub.ApiWeb.Domain.ApiFilter;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.ApplicationService.Impl;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.Scripting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.ApiWeb.Domain.Scripting.Aop;
using JP.DataHub.ApiWeb.Domain.Service.Impl;
using JP.DataHub.ApiWeb.Domain.Service;

namespace JP.DataHub.ApiWeb.Domain
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            // AOP CACHE
            container.RegisterType<IAopCacheHelper, AopCacheHelper>(new PerResolveLifetimeManager(), new InjectionConstructor(new InjectionParameter<string>(null)));

            // config
            var appconfig = configuration.GetSection("AppConfig");
            var supportedCultures = appconfig.GetSection("SupportedCultures")?.Get<string[]>() ?? new[] { "ja" };
            container.RegisterInstance("SupportedCultures", supportedCultures);
            container.RegisterInstance("DefaultCulture", supportedCultures.FirstOrDefault());
            container.RegisterInstance("IsInternalServerErrorDetailResponse", appconfig.GetValue<bool>("IsInternalServerErrorDetailResponse", false));
            container.RegisterInstance("VendorSystemAuthenticationDefaultVendorId", appconfig.GetValue<string>("VendorSystemAuthenticationDefaultVendorId", "00000000-0000-0000-0000-000000000001"));
            container.RegisterInstance("VendorSystemAuthenticationDefaultSystemId", appconfig.GetValue<string>("VendorSystemAuthenticationDefaultSsytemId", "00000000-0000-0000-0000-000000000001"));
            container.RegisterInstance("VendorSystemAuthenticationDefaultVendorId", appconfig.GetValue<Guid>("VendorSystemAuthenticationDefaultVendorId", Guid.Parse("00000000-0000-0000-0000-000000000001")));
            container.RegisterInstance("VendorSystemAuthenticationDefaultSystemId", appconfig.GetValue<Guid>("VendorSystemAuthenticationDefaultSsytemId", Guid.Parse("00000000-0000-0000-0000-000000000001")));
            container.RegisterInstance("OperatingVendorVendorId", appconfig.GetValue<string>("OperatingVendorVendorId"));
            container.RegisterInstance("DefaultVendorId", appconfig.GetValue<string>("DefaultVendorId"));
            container.RegisterInstance("AllowAsync", appconfig.GetValue<bool>("AllowAsync", false));
            container.RegisterInstance("LoggingHttpHeaders", appconfig.GetValue<string>("LoggingHttpHeaders", "*"));
            container.RegisterInstance("AdminKeyword", appconfig.GetValue<string>("AdminKeyword"));
            container.RegisterInstance("UseClientCertificateAuth", appconfig.GetValue<bool>("UseClientCertificateAuth", false));
            container.RegisterInstance("EnableIpFilter", appconfig.GetValue<bool>("EnableIpFilter", false));
            container.RegisterInstance("HeaderAuthentication", appconfig.GetValue<bool>("HeaderAuthentication", false));
            container.RegisterInstance("EnableVendorDataUseAndOffer", appconfig.GetValue<bool>("EnableVendorDataUseAndOffer", true));
            container.RegisterInstance("HeaderIgnoreCase", appconfig.GetValue<bool>("HeaderIgnoreCase", false));
            container.RegisterInstance("EnableResourceVersion", appconfig.GetValue<bool>("EnableResourceVersion", true));
            container.RegisterInstance("EnableThreadingOfReference", appconfig.GetValue<bool>("EnableThreadingOfReference", true));
            container.RegisterInstance("EnableOriginalAuthentication", appconfig.GetValue<bool>("EnableOriginalAuthentication", true));
            container.RegisterInstance("EnableWebHookAndMailTemplate", appconfig.GetValue<bool>("EnableWebHookAndMailTemplate", true));
            container.RegisterInstance("UseApiAttachFileDocumentHistory", appconfig.GetValue<bool>("UseApiAttachFileDocumentHistory", true));
            container.RegisterInstance("MaxRegisterContentLength", appconfig.GetValue<int>("MaxRegisterContentLength"));
            container.RegisterInstance("MaxBase64AttachFileContentLength", appconfig.GetValue<int>("MaxBase64AttachFileContentLength"));
            container.RegisterInstance("MaxSaveApiResponseCacheSize", appconfig.GetValue<int>("MaxSaveApiResponseCacheSize"));
            container.RegisterInstance("InvalidODataColums", appconfig.GetSection("InvalidODataColums").Get<string[]>());
            container.RegisterInstance("XRequestContinuationNeedsTopCount", appconfig.GetValue<bool>("XRequestContinuationNeedsTopCount", false));
            container.RegisterInstance("EnableJsonDocumentReference", appconfig.GetValue<bool>("EnableJsonDocumentReference", true));
            container.RegisterInstance("EnableJsonDocumentHistory", appconfig.GetValue<bool>("EnableJsonDocumentHistory", true));
            container.RegisterInstance("ThresholdJsonSchemaValidaitonParallelize", appconfig.GetValue<int>("ThresholdJsonSchemaValidaitonParallelize", 10));
            container.RegisterInstance("EnableJsonDocumentKeepRegDate", appconfig.GetValue<bool>("EnableJsonDocumentKeepRegDate", true));
            container.RegisterInstance("SupportedCultures", appconfig.GetValue<string>("SupportedCultures", "ja").Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());
            container.RegisterInstance("NeedToModifyErrorApiList", appconfig.GetValue<string>("NeedToModifyErrorApiList"));
            container.RegisterInstance("GatewayClientTiemoutSec", appconfig.GetValue<int>("GatewayClientTiemoutSec", 60 * 60 * 24));
            container.RegisterInstance("UseEmitBlockchainEvent", appconfig.GetValue<bool>("UseEmitBlockchainEvent", true));
            container.RegisterInstance("XRequestContinuationNeedsTopCount", appconfig.GetValue<bool>("XRequestContinuationNeedsTopCount", false));
            container.RegisterInstance("FixedOpenIdAllowedApplication", appconfig.GetValue<string>("FixedOpenIdAllowedApplication", ""));
            container.RegisterInstance("AuthenticationCacheExpirationTimeSpan", appconfig.GetValue<TimeSpan>("AuthenticationCacheExpirationTimeSpan", new TimeSpan(0, 30, 0)));
            container.RegisterInstance("DynamicApiCacheExpirationTimeSpan", appconfig.GetValue<TimeSpan>("DynamicApiCacheExpirationTimeSpan", new TimeSpan(0, 30, 0)));
            container.RegisterInstance("Authentication.IsCheck.VendorSystemFunc", appconfig.GetValue<bool>("Authentication.IsCheck.VendorSystemFunc", true));
            container.RegisterInstance("Return.JsonValidator.ErrorDetail", appconfig.GetValue<bool>("Return.JsonValidator.ErrorDetail", true));
            container.RegisterInstance("UseForeignKeyCache", appconfig.GetValue<bool>("UseForeignKeyCache", false));
            container.RegisterInstance("ScriptRuntimeLogDynamicApiUrlRegist", appconfig.GetValue<string>("ScriptRuntimeLogDynamicApiUrlRegist"));
            container.RegisterInstance("ScriptRuntimeLogFileBlobContainerName", appconfig.GetValue<string>("ScriptRuntimeLogFileBlobContainerName"));
            container.RegisterInstance("IsAopCacheEnable", appconfig.GetValue<bool>("IsAopCacheEnable", false));
            container.RegisterInstance("IsRoslynScriptCacheEnable", appconfig.GetValue<bool>("IsRoslynScriptCacheEnable", false));
            container.RegisterInstance<bool>("IsEnableScriptRuntimeLogService",
                !string.IsNullOrWhiteSpace(appconfig.GetValue<string>("ScriptRuntimeLogDynamicApiUrlRegist")) &&
                !string.IsNullOrWhiteSpace(appconfig.GetValue<string>("ScriptRuntimeLogFileBlobContainerName")) &&
                !string.IsNullOrWhiteSpace(configuration.GetValue<string>("ConnectionStrings:ScriptRuntimeLogBlobStorageConnectionStrings")));
            container.RegisterInstance("UseTerms", appconfig.GetValue<bool>("UseTerms", false));
            container.RegisterInstance("UseStrictValidationOnUpdate", appconfig.GetValue<bool>("UseStrictValidationOnUpdate", true));

            var attachFileConfig = configuration.GetSection("AttachFile");
            container.RegisterInstance("AttachFileTmpPath", attachFileConfig.GetValue<string>("AttachFileTmpPath"));
            container.RegisterInstance("IsEnableUploadContentCheck", attachFileConfig.GetValue<bool>("IsEnableUploadContentCheck", false));
            container.RegisterInstance("UploadOK_ContentTypeList", attachFileConfig.GetValue<string>("UploadOK_ContentTypeList", "").Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            container.RegisterInstance("UploadOK_ExtensionList", attachFileConfig.GetValue<string>("UploadOK_ExtensionList", "").Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            container.RegisterInstance("IsUploadOk_NoExtensionFile", attachFileConfig.GetValue<bool>("IsUploadOk_NoExtensionFile", true));
            container.RegisterInstance("AttachFileMetaDataSchemaId", attachFileConfig.GetValue<string>("AttachFileMetaDataSchemaId"));

            // DataContainer(inner domain)
            container.RegisterType<IDynamicApiDataContainer, DynamicApiDataContainer>(new PerRequestLifetimeManager());

            // ApplicationServiceの登録
            container.RegisterType<IDynamicApiApplicationService, DynamicApiApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IVendorSystemAuthenticationApplicationService, VendorSystemAuthenticationApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IVendorSystemClientCertificateAuthenticationApplicationService, VendorSystemClientCertificateAuthenticationApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IScriptRuntimeLogApplicationService, ScriptRuntimeLogApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IAttachFileApplicationService, AttachFileApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IOpenIdUserApplicationService, OpenIdUserApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICryptographyApplicationService, CryptographyApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICryptographyManagementApplicationService, CryptographyManagementApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IMetadataInfoApplicationService, MetadataInfoApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICacheApplicationService, CacheApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            // Serviceの登録
            container.RegisterType<ILoggingFilterService, LoggingFilterService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceSharingPersonService, ResourceSharingPersonService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            // Entityの登録
            container.RegisterType<IMethod, Method>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            // DomainEventPublisher
            container.RegisterType<IDomainEventPublisher, DomainEventPublisher>(new ContainerControlledLifetimeManager());
            var domainEventPublisher = container.Resolve<IDomainEventPublisher>();
            domainEventPublisher.Subscribe<ScriptRuntimeLogWriteEventData>(new ScriptRuntimeLogWriteSubscriber());

            // other
            container.RegisterType<IApiHelper, ApiHelperBypass>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITermsHelper, TermsHelper>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFilterManager, FilterManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICacheHelperFactory, CacheHelperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterActionType();
            container.RegisterType<IAccessTokenCache, AccessTokenCache>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

            container.RegisterType<ITermsService, TermsService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserTermsService, UserTermsService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<ICertifiedApplicationService, CertifiedApplicationService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserGroupService, UserGroupService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IUserResourceShareService, UserResourceShareService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IResourceGroupService, ResourceGroupService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            container.RegisterType<IRevokeService, RevokeService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());

        }
    }
}

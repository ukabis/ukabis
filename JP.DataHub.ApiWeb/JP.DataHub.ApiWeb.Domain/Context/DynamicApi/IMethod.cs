using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Aop;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    // .NET6
    interface IMethod : IEntity
    {

        VendorId VendorId { get; set; }

        SystemId SystemId { get; set; }

        ApiId ApiId { get; set; }

        ControllerId ControllerId { get; set; }

        ControllerUrl ControllerRelativeUrl { get; set; }

        IsUseBlobCache IsUseBlobCache { get; set; }

        HttpMethodType MethodType { get; set; }

        DataSchema RequestSchema { get; set; }

        DataSchema ControllerSchema { get; set; }

        DataSchema UriSchema { get; set; }

        DataSchema ResponseSchema { get; set; }

        ReadOnlyCollection<RepositoryInfo> RepositoryInfo { get; set; }

        IsHeaderAuthentication IsHeaderAuthentication { get; set; }

        IsVendorSystemAuthenticationAllowNull IsVendorSystemAuthenticationAllowNull { get; set; }

        IsClientCertAuthentication IsClientCertAuthentication { get; set; }

        IsOpenIdAuthentication IsOpenIdAuthentication { get; set; }

        IsAdminAuthentication IsAdminAuthentication { get; set; }

        RepositoryKey RepositoryKey { get; set; }

        RepositoryKey ControllerRepositoryKey { get; set; }

        UrlParameter KeyValue { get; set; }

        QueryStringVO Query { get; set; }

        QueryType QueryType { get; set; }

        PostDataType PostDataType { get; set; }

        RelativeUri RelativeUri { get; set; }

        ApiUri ApiUri { get; set; }

        ApiQuery ApiQuery { get; set; }

        IsVendor IsVendor { get; set; }

        IsPerson IsPerson { get; set; }

        GatewayInfo GatewayInfo { get; set; }

        IsOverPartition IsOverPartition { get; set; }

        Script Script { get; set; }

        ScriptTypeVO ScriptType { get; set; }

        ActionTypeVO ActionType { get; set; }

        ActionTypeVO AsyncOriginalActionType { get; set; }

        CacheInfo CacheInfo { get; set; }

        IsAccesskey IsAccesskey { get; set; }

        IsAutomaticId IsAutomaticId { get; set; }

        ActionTypeVersion ActionTypeVersion { get; set; }

        ActionInjectorHandler ActionInjectorHandler { get; set; }

        PartitionKey PartitionKey { get; set; }

        ApiResourceSharing ApiResourceSharing { get; set; }

        List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; set; }

        HasMailTemplate HasMailTemplate { get; set; }

        HasWebhook HasWebhook { get; set; }

        IsEnableAttachFile IsEnableAttachFile { get; set; }
        InternalOnly InternalOnly { get; set; }

        /// <summary>
        /// AttachFileBlobRepositoryの情報
        /// </summary>
        RepositoryInfo AttachFileBlobRepositoryInfo { get; set; }
        RepositoryGroupId AttachFileBlobRepositoryGroupId { get; set; }

        IsSkipJsonSchemaValidation IsSkipJsonSchemaValidation { get; set; }

        PublicDate PublicDate { get; set; }

        IsOpenidAuthenticationAllowNull IsOpenidAuthenticationAllowNull { get; set; }

        IsOptimisticConcurrency IsOptimisticConcurrency { get; set; }
        IsEnableBlockchain IsEnableBlockchain { get; set; }
        IsDocumentHistory IsDocumentHistory { get; set; }
        RepositoryInfo DocumentHistoryRepositoryInfo { get; set; }
        IsVisibleAgreement IsVisibleAgreement { get; set; }
        IsContainerDynamicSeparation IsContainerDynamicSeparation { get; set; }
        IsOtherResourceSqlAccess IsOtherResourceSqlAccess { get; set; }

        IsTransparentApi IsTransparentApi { get; set; }
        IsEnableResourceVersion IsEnableResourceVersion { get; set; }
        IsRequireConsent IsRequireConsent { get; set; }
        TermsGroupCode TermsGroupCode { get; set; }
        ResourceGroupId ResourceGroupId { get; set; }

        [DynamicApiLogging]
        HttpResponseMessage Request(ActionId actionId, MediaType mediaType, NotAuthentication notAuthentication, Contents contents, Accept accept = null, ContentRange contentRange = null);

        HttpResponseMessage Authenticate();
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Aop;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    [Log]
    internal interface IDynamicApiAction
    {
        ActionId ActionId { get; set; }

        ApiId ApiId { get; set; }

        ControllerId ControllerId { get; set; }

        ControllerUrl ControllerRelativeUrl { get; set; }

        HttpMethodType MethodType { get; set; }

        DataSchema RequestSchema { get; set; }

        DataSchema UriSchema { get; set; }

        DataSchema ControllerSchema { get; set; }

        DataSchema ResponseSchema { get; set; }

        ReadOnlyCollection<RepositoryInfo> RepositoryInfo { get; set; }

        IsOpenIdAuthentication IsOpenIdAuthentication { get; set; }

        RepositoryKey RepositoryKey { get; set; }

        RepositoryKey ControllerRepositoryKey { get; set; }

        UrlParameter KeyValue { get; set; }

        QueryStringVO Query { get; set; }

        QueryType QueryType { get; set; }

        PostDataType PostDataType { get; set; }

        Contents Contents { get; set; }

        RelativeUri RelativeUri { get; set; }

        ApiUri ApiUri { get; set; }

        ApiQuery ApiQuery { get; set; }

        IsVendor IsVendor { get; set; }

        IsPerson IsPerson { get; set; }

        VendorId VendorId { get; set; }

        SystemId SystemId { get; set; }

        VendorId ProviderVendorId { get; set; }

        SystemId ProviderSystemId { get; set; }

        OpenId OpenId { get; set; }

        XAdmin Xadmin { get; set; }

        XVersion Xversion { get; set; }

        XGetInnerField XGetInnerAllField { get; set; }

        XRequestContinuation XRequestContinuation { get; set; }

        IsOverPartition IsOverPartition { get; set; }

        ActionTypeVO ActionType { get; set; }
        ActionTypeVO AsyncOriginalActionType { get; set; }

        CacheInfo CacheInfo { get; set; }

        IsAutomaticId IsAutomaticId { get; set; }

        ActionTypeVersion ActionTypeVersion { get; set; }

        ActionInjectorHandler ActionInjectorHandler { get; set; }

        PartitionKey PartitionKey { get; set; }

        ApiResourceSharing ApiResourceSharing { get; set; }

        XResourceSharingPerson XResourceSharingPerson { get; set; }

        List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; set; }

        ReadOnlyCollection<INewDynamicApiDataStoreRepository> DynamicApiDataStoreRepository { get; set; }

        MediaType MediaType { get; set; }

        IsOverrideId IsOverrideId { get; set; }

        Accept Accept { get; set; }
        ContentRange ContentRange { get; set; }

        HasMailTemplate HasMailTemplate { get; set; }

        HasWebhook HasWebhook { get; set; }

        IsEnableAttachFile IsEnableAttachFile { get; set; }

        RepositoryInfo AttachFileBlobRepositoryInfo { get; set; }

        IDynamicApiAttachFileRepository AttachFileDynamicApiDataStoreRepository { get; set; }

        IsSkipJsonSchemaValidation IsSkipJsonSchemaValidation { get; set; }

        IsUseBlobCache IsUseBlobCache { get; set; }

        IsContainerDynamicSeparation IsContainerDynamicSeparation { get; set; }

        IsOtherResourceSqlAccess IsOtherResourceSqlAccess { get; set; }


        void Initialize();
        IsOptimisticConcurrency IsOptimisticConcurrency { get; set; }
        XNoOptimistic XNoOptimistic { get; set; }
        IsEnableBlockchain IsEnableBlockchain { get; set; }
        IsDocumentHistory IsDocumentHistory { get; set; }
        RepositoryGroupId AttachFileBlobRepositoryGroupId { get; set; }
        INewDynamicApiDataStoreRepository HistoryEvacuationDataStoreRepository { get; set; }
        XUserResourceSharing XUserResourceSharing { get; set; }

        [DynamicApiActionFilter]
        HttpResponseMessage ExecuteAction();
    }
}

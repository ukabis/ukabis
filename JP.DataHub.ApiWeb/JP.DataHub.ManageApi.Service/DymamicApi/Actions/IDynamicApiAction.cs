using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Actions
{
    internal interface IDynamicApiAction
    {
        //string ActionId { get; set; }

        //string ApiId { get; set; }

        //string ControllerId { get; set; }

        //string ControllerRelativeUrl { get; set; }

        //HttpMethodType MethodType { get; set; }

        //DataSchema RequestSchema { get; set; }

        //DataSchema UriSchema { get; set; }

        //DataSchema ControllerSchema { get; set; }

        //DataSchema ResponseSchema { get; set; }

        //ReadOnlyCollection<RepositoryInfo> RepositoryInfo { get; set; }

        //IsOpenIdAuthentication IsOpenIdAuthentication { get; set; }

        //RepositoryKey RepositoryKey { get; set; }

        //RepositoryKey ControllerRepositoryKey { get; set; }

        //UrlParameter KeyValue { get; set; }

        //QueryString Query { get; set; }

        //QueryType QueryType { get; set; }

        //PostDataType PostDataType { get; set; }

        //Contents Contents { get; set; }

        //RelativeUrl RelativeUrl { get; set; }

        //ApiUrl ApiUrl { get; set; }

        //ApiQuery ApiQuery { get; set; }

        //IsVendor IsVendor { get; set; }

        //IsPerson IsPerson { get; set; }

        //VendorId VendorId { get; set; }

        //SystemId SystemId { get; set; }

        //VendorId ProviderVendorId { get; set; }

        //SystemId ProviderSystemId { get; set; }

        //OpenId OpenId { get; set; }

        //Xadmin Xadmin { get; set; }

        //XVersion Xversion { get; set; }

        //XGetInnerField XGetInnerAllField { get; set; }

        //XRequestContinuation XRequestContinuation { get; set; }

        //IsOverPartition IsOverPartition { get; set; }

        //ActionType ActionType { get; set; }
        //ActionType AsyncOriginalActionType { get; set; }

        //CacheInfo CacheInfo { get; set; }

        //IsAutomaticId IsAutomaticId { get; set; }

        //ActionTypeVersion ActionTypeVersion { get; set; }

        //ActionInjectorHandler ActionInjectorHandler { get; set; }

        //PartitionKey PartitionKey { get; set; }

        //ApiResourceSharing ApiResourceSharing { get; set; }

        //XResourceSharingPerson XResourceSharingPerson { get; set; }

        //List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; set; }

        //ReadOnlyCollection<INewDynamicApiDataStoreRepository> DynamicApiDataStoreRepository { get; set; }

        //MediaType MediaType { get; set; }

        //IsOverrideId IsOverrideId { get; set; }

        //Accept Accept { get; set; }
        //ContentRange ContentRange { get; set; }

        //HasMailTemplate HasMailTemplate { get; set; }

        //HasWebhook HasWebhook { get; set; }

        //IsEnableAttachFile IsEnableAttachFile { get; set; }

        //RepositoryInfo AttachFileBlobRepositoryInfo { get; set; }

        //IDynamicApiAttachFileRepository AttachFileDynamicApiDataStoreRepository { get; set; }

        //IsSkipJsonSchemaValidation IsSkipJsonSchemaValidation { get; set; }

        //IsUseBlobCache IsUseBlobCache { get; set; }

        //IsContainerDynamicSeparation IsContainerDynamicSeparation { get; set; }

        //IsOtherResourceSqlAccess IsOtherResourceSqlAccess { get; set; }


        //void Initialize();
        //IsOptimisticConcurrency IsOptimisticConcurrency { get; set; }
        //XNoOptimistic XNoOptimistic { get; set; }
        //IsEnableBlockchain IsEnableBlockchain { get; set; }
        //IsDocumentHistory IsDocumentHistory { get; set; }
        //RepositoryGroupId AttachFileBlobRepositoryGroupId { get; set; }
        //INewDynamicApiDataStoreRepository HistoryEvacuationDataStoreRepository { get; set; }

        //[DynamicApiActionFilter]
        //HttpResponseMessage ExecuteAction();
    }
}

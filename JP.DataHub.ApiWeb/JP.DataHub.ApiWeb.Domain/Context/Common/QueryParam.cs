using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal record QueryParam : IValueObject
    {
        public bool IsNative { get => !string.IsNullOrEmpty(NativeQuery?.Sql); }
        public bool HasFilePath { get => FilePath != null; }

        public VendorId VendorId { get; }
        public SystemId SystemId { get; }
        public OpenId OpenId { get; }
        public RepositoryKey RepositoryKey { get; }
        public DataSchema ControllerSchema { get; private set; }
        public DataSchema UriSchema { get; }
        public DataSchema RequestSchema { get; }
        public DataSchema ResponseSchema { get; }
        public ApiQuery ApiQuery { get; }
        public PostDataType PostDataType { get; }
        public ActionTypeVO ActionType { get; }
        public PartitionKey PartitionKey { get; }
        public UrlParameter KeyValue { get; }
        public IsPerson IsPerson { get; }
        public IsVendor IsVendor { get; }
        public IsOverPartition IsOverPartition { get; }
        public IsAutomaticId IsAutomaticId { get; }
        public QueryStringVO QueryString { get; }
        public QueryType QueryType { get; }
        public ODataQuery ODataQuery { get; }
        public NativeQuery NativeQuery { get; private set; }
        public ApiResourceSharing ApiResourceSharing { get; }
        public List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; }
        public XResourceSharingWith XResourceSharingWith { get; }
        public XResourceSharingPerson XResourceSharingPerson { get; }
        public XVersion XVersion { get; }
        public HasSingleData HasSingleData { get; private set; }
        public FilePath FilePath { get; }
        public CacheInfo CacheInfo { get; }
        public IsOptimisticConcurrency IsOptimisticConcurrency { get; }
        public ResourceVersion ResourceVersion { get; }
        public Identification Identification { get; }
        public XRequestContinuation XRequestContinuation { get; }
        public SelectCount SelectCount { get; }
        public SkipCount SkipCount { get; }

        public IsContainerDynamicSeparation IsContainerDynamicSeparation { get; }
        public ControllerId ControllerId { get; }
        public IsDocumentHistory IsDocumentHistory { get; }
        public OperationInfo OperationInfo { get; private set; }
        public IsOtherResourceSqlAccess IsOtherResourceSqlAccess { get; }
        public XUserResourceSharing XUserResourceSharing { get; }

        public QueryParam(NativeQuery nativeQuery, HasSingleData hasSingleData,
            IsContainerDynamicSeparation isContainerDynamicSeparation = null, ControllerId controllerId = null, IsPerson isPerson = null)
        {
            NativeQuery = nativeQuery;
            HasSingleData = hasSingleData;
            IsContainerDynamicSeparation = isContainerDynamicSeparation;
            ControllerId = controllerId;
            IsPerson = isPerson;
        }

        [DefaultValueObject]
        public QueryParam(VendorId vendorId, SystemId systemId, OpenId openId, DataSchema controllerSchema,
            DataSchema uriSchema, DataSchema requestSchema, DataSchema responseSchema, RepositoryKey repositoryKey,
            ApiQuery apiQuery, PostDataType postDataType, ActionTypeVO actionType, PartitionKey partitionKey,
            UrlParameter keyValue, IsVendor isVendor, IsPerson isPerson, IsOverPartition isOverPartition,
            IsAutomaticId isAutomaticId, QueryStringVO queryString, QueryType queryType, ODataQuery oDataQuery,
            NativeQuery nativeQuery, ApiResourceSharing apiResourceSharing, List<ResourceSharingPersonRule> resourceSharingPersonRules,
            XResourceSharingWith xResourceSharingWith, XResourceSharingPerson xResourceSharingPerson, XVersion xVersion,
            HasSingleData hasSingleData, CacheInfo cacheInfo, XRequestContinuation xRequestContinuation, IsDocumentHistory isDocumentHistory = null,
            Identification identification = null, SelectCount selectCount = null, SkipCount skipCount = null,
            IsContainerDynamicSeparation isContainerDynamicSeparation = null, ControllerId controllerId = null,
            OperationInfo operationInfo = null, IsOtherResourceSqlAccess isOtherResourceSqlAccess = null, IsOptimisticConcurrency isOptimisticConcurrency = null,
            XUserResourceSharing xUserResourceSharing = null)
        {
            VendorId = vendorId;
            SystemId = systemId;
            OpenId = openId;
            ControllerSchema = controllerSchema;
            UriSchema = uriSchema;
            RequestSchema = requestSchema;
            ResponseSchema = responseSchema;
            RepositoryKey = repositoryKey;
            ApiQuery = apiQuery;
            PostDataType = postDataType;
            ActionType = actionType;
            KeyValue = keyValue;
            IsVendor = isVendor;
            IsPerson = isPerson;
            IsOverPartition = isOverPartition;
            IsAutomaticId = isAutomaticId;
            PartitionKey = partitionKey;
            QueryString = queryString;
            QueryType = queryType;
            ODataQuery = oDataQuery;
            NativeQuery = nativeQuery;
            ApiResourceSharing = apiResourceSharing;
            ResourceSharingPersonRules = resourceSharingPersonRules;
            XResourceSharingWith = xResourceSharingWith;
            XResourceSharingPerson = xResourceSharingPerson;
            XVersion = xVersion;
            HasSingleData = hasSingleData;
            CacheInfo = cacheInfo;
            Identification = identification;
            XRequestContinuation = xRequestContinuation;
            SelectCount = selectCount;
            SkipCount = skipCount;
            IsContainerDynamicSeparation = isContainerDynamicSeparation;
            ControllerId = controllerId;
            IsDocumentHistory = isDocumentHistory;
            OperationInfo = operationInfo;
            IsOtherResourceSqlAccess = isOtherResourceSqlAccess;
            IsOptimisticConcurrency = isOptimisticConcurrency;
            XUserResourceSharing = xUserResourceSharing;
        }

        public QueryParam(FilePath filePath)
        {
            FilePath = filePath;
        }

        public QueryParam ToSingle()
        {
            if ((HasSingleData?.Value) != true)
            {
                HasSingleData = new HasSingleData(true);
            }
            return this;
        }
    }
}

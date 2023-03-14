using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal record RegisterParam : IValueObject
    {
        public bool HasJson { get => Json != null; }
        public bool HasStream { get => Stream != null; }
        public Uri SourceUri { get; }
        public JToken Json { get; }
        public Stream Stream { get; }

        public VendorId VendorId { get; }
        public SystemId SystemId { get; }
        public OpenId OpenId { get; }
        public RepositoryKey RepositoryKey { get; }
        public PartitionKey PartitionKey { get; }
        public IsPerson IsPerson { get; }
        public IsVendor IsVendor { get; }
        public IsAutomaticId IsAutomaticId { get; }
        public ApiResourceSharing ApiResourceSharing { get; }
        public List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; }
        public XResourceSharingWith XResourceSharingWith { get; }
        public XResourceSharingPerson XResourceSharingPerson { get; }

        public IsOverrideId IsOverrideId { get; }
        public IsOptimisticConcurrency IsOptimisticConcurrency { get; }
        public XVersion XVersion { get; }
        public FilePath FilePath { get; }
        public string ContentType { get; }
        public bool NameToLower { get; } = true;

        public ApiId ApiId { get; }
        public ControllerId ControllerId { get; }
        public ControllerUrl ControllerRelativeUrl { get; }
        public RepositoryKey ControllerRepositoryKey { get; }
        public PostDataType PostDataType { get; }
        public ApiUri ApiUri { get; }
        public MediaType MediaType { get; }

        public IsContainerDynamicSeparation IsContainerDynamicSeparation { get; }

        public OperationInfo OperationInfo { get; }
        public DataSchema ControllerSchema { get; }
        public DataSchema RequestSchema { get; }
        public ActionTypeVO ActionType { get; }


        [DefaultValueObject]
        public RegisterParam(JToken json, VendorId vendorId, SystemId systemId, OpenId openId, IsAutomaticId isAutomaticId,
            RepositoryKey repositoryKey, PartitionKey partitionKey, IsOverrideId isOverrideId, IsOptimisticConcurrency isOptimisticConcurrency,
            XVersion xVersion, IsVendor isVendor, IsPerson isPerson, List<ResourceSharingPersonRule> resourceSharingPersonRules, ApiResourceSharing apiResourceSharing,
            XResourceSharingWith xResourceSharingWith, XResourceSharingPerson xResourceSharingPerson,
            ApiId apiId = null, ControllerId controllerId = null, RepositoryKey controllerRepositoryKey = null, PostDataType postDataType = null, ApiUri apiUri = null, MediaType mediaType = null,
            IsContainerDynamicSeparation isContainerDynamicSeparation = null, OperationInfo operationInfo = null, DataSchema controllerSchema = null, DataSchema requestSchema = null,
            ActionTypeVO actionType = null
            )
        {
            Json = json;
            VendorId = vendorId;
            SystemId = systemId;
            OpenId = openId;
            RepositoryKey = repositoryKey;
            PartitionKey = partitionKey;
            IsOverrideId = isOverrideId;
            IsAutomaticId = isAutomaticId;
            IsOptimisticConcurrency = isOptimisticConcurrency;
            XVersion = xVersion;
            IsVendor = isVendor;
            IsPerson = isPerson;
            ResourceSharingPersonRules = resourceSharingPersonRules;
            ApiResourceSharing = apiResourceSharing;
            XResourceSharingWith = xResourceSharingWith;
            XResourceSharingPerson = xResourceSharingPerson;
            IsContainerDynamicSeparation = isContainerDynamicSeparation;

            ApiId = apiId;
            ControllerId = controllerId;
            ControllerRepositoryKey = controllerRepositoryKey;
            PostDataType = postDataType;
            ApiUri = apiUri;
            MediaType = mediaType;
            OperationInfo = operationInfo;
            ControllerSchema = controllerSchema;
            RequestSchema = requestSchema;
            ActionType = actionType;
        }

        public RegisterParam(object obj)
        {
            Json = obj.ToJson();
        }

        public RegisterParam(Stream stream)
        {
            Stream = stream;
        }

        public RegisterParam(Stream stream, VendorId vendorId, FilePath filePath, string contentType, bool nameToLower = true)
        {
            Stream = stream;
            ContentType = contentType;
            FilePath = filePath;
            NameToLower = nameToLower;
            VendorId = vendorId;
        }

        public RegisterParam(Uri uri, JToken json, VendorId vendorId, SystemId systemId, OpenId openId,
        RepositoryKey repositoryKey, PartitionKey partitionKey,
        XVersion xVersion, IsVendor isVendor, IsPerson isPerson,
        ApiId apiId = null, ControllerId controllerRelativeUrl = null, RepositoryKey controllerRepositoryKey = null, ApiUri apiUri = null
        )
        {
            SourceUri = uri;
            Json = json;
            VendorId = vendorId;
            SystemId = systemId;
            OpenId = openId;
            RepositoryKey = repositoryKey;
            PartitionKey = partitionKey;
            XVersion = xVersion;
            IsVendor = isVendor;
            IsPerson = isPerson;

            ApiId = apiId;
            ControllerId = controllerRelativeUrl;
            RepositoryKey = controllerRepositoryKey;
            ApiUri = apiUri;
        }
    }
}

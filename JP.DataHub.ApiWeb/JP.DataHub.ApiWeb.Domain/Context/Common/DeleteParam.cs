using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DeleteParam : IValueObject
    {
        public bool HasJson { get => Json != null; }
        public bool HasFilePath { get => FilePath != null; }
        public JToken Json { get; }

        public VendorId VendorId { get; }
        public FilePath FilePath { get; }
        public Action<JToken, RepositoryType> CallbackDelete { get; }
        public bool ThrowNotFoundExcption { get; } = true;

        public IsContainerDynamicSeparation IsContainerDynamicSeparation { get; }
        public ControllerId ControllerId { get; }

        public IsVendor IsVendor { get; }
        public IsPerson IsPerson { get; }
        public OperationInfo OperationInfo { get; }

        public XResourceSharingPerson XResourceSharingPerson { get; }
        public List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; }


        [DefaultValueObject]
        public DeleteParam(
            VendorId vendorId, JToken json, Action<JToken, RepositoryType> callbackDelete = null,
            IsContainerDynamicSeparation isContainerDynamicSeparation = null, ControllerId controllerId = null, IsVendor isVendor = null, IsPerson isPerson = null,
            XResourceSharingPerson xResourceSharingPerson = null, List<ResourceSharingPersonRule> resourceSharingPersonRules = null,
            OperationInfo operationInfo = null)
        {
            VendorId = vendorId;
            CallbackDelete = callbackDelete;
            Json = json;
            IsContainerDynamicSeparation = isContainerDynamicSeparation;
            ControllerId = controllerId;
            IsVendor = isVendor;
            IsPerson = isPerson;
            XResourceSharingPerson = xResourceSharingPerson;
            ResourceSharingPersonRules = resourceSharingPersonRules;
            OperationInfo = operationInfo;
        }

        public DeleteParam(FilePath filePath, bool throwNotFoundExcption)
        {
            FilePath = filePath;
            ThrowNotFoundExcption = throwNotFoundExcption;
        }

        public DeleteParam(VendorId vendorId, FilePath filePath, bool throwNotFoundExcption)
        {
            VendorId = vendorId;
            FilePath = filePath;
            ThrowNotFoundExcption = throwNotFoundExcption;
        }
    }
}

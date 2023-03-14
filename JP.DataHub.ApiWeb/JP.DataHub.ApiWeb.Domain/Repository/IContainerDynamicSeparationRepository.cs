using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IContainerDynamicSeparationRepository
    {
        /// <summary>
        /// コンテナ名を取得する。存在しなければ作成する。
        /// </summary>
        string GetOrRegisterContainerName(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId, VendorId vendorId, SystemId systemId, OpenId openId, out bool isRegistered);

        /// <summary>
        /// コンテナ名を取得する。存在しなければ作成する。
        /// </summary>
        string GetOrRegisterContainerName(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId, VendorId vendorId, SystemId systemId, OpenId openId = null);

        /// <summary>
        /// リソースの全コンテナ名を取得する。
        /// </summary>
        IList<string> GetAllContainerNames(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId);
    }
}

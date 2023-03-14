using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IAuthenticationRepository
    {
        User Login(VendorId vendorId, SystemId systemId, UserId userId = null);

        AdminAuthResult IsAdmin(AdminKeyword adminKeyword, SystemId systemId);

        /// <summary>
        /// ファンクションを更新する
        /// </summary>
        /// <param name="function">ファンクション情報</param>
        /// <returns>ファンクションID</returns>
        Task<FunctionId> MergeFunction(Function function);

        Task RefreshStaticCache(string time);

        void CheckStaticCacheTime();
    }
}
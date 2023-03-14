using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.OpenIdUser;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    /// <summary>
    /// OpenIdユーザー管理のためのアプリケーションサービスのインターフェースです。
    /// </summary>
    interface IOpenIdUserApplicationService
    {
        /// <summary>
        /// 指定されたユーザーIDのユーザーを削除します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>削除結果</returns>
        Task<OpenIdUserOperationResult> Delete(SystemId systemId, UserId userId);

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        Task<OpenIdUser> Get(UserId userId);

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        Task<OpenIdUserOperationResult> Get(SystemId systemId, UserId userId);

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        Task<OpenIdUserOperationResult> GetFullAccess(UserId userId);

        /// <summary>
        /// 指定されたシステムで登録されたユーザーの一覧を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>ユーザーの一覧</returns>
        Task<IEnumerable<OpenIdUser>> GetList(SystemId systemId);

        /// <summary>
        /// 指定されたユーザー情報でユーザーを登録します。
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>登録結果</returns>
        Task<OpenIdUserOperationResult> Register(OpenIdUser user);
    }
}

using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.OpenIdUser;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    /// <summary>
    /// OpenIdユーザー管理のためのリポジトリのインターフェースです。
    /// </summary>
    interface IOpenIdUserRepository
    {
        /// <summary>
        /// 指定されたオブジェクトIDのユーザーを削除します。
        /// </summary>
        /// <param name="objectId">OpenIdプロバイダー中のオブジェクトID</param>
        /// <returns>Task</returns>
        Task Delete(ObjectId objectId);

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        Task<OpenIdUser> Get(UserId userId);

        /// <summary>
        /// 指定された条件で登録されたユーザーの一覧を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>ユーザーの一覧</returns>
        Task<IEnumerable<OpenIdUser>> GetList(SystemId systemId);

        /// <summary>
        /// 指定されたユーザー情報でユーザーを登録します。
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>登録結果</returns>
        Task<OpenIdUser> Register(OpenIdUser user);

        /// <summary>
        /// 指定されたユーザー情報でユーザーを更新します。
        /// </summary>
        /// <param name="objectId">OpenIdプロバイダー中のオブジェクトID</param>
        /// <param name="user">ユーザー情報</param>
        /// <returns>Task</returns>
        Task Update(ObjectId objectId, OpenIdUser user);
    }
}

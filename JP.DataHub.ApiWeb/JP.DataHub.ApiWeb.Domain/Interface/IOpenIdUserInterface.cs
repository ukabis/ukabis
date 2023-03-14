using System.Net;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    /// <summary>
    /// OpenIdユーザー管理のためのインターフェースです。
    /// </summary>
    public interface IOpenIdUserInterface
    {
        /// <summary>
        /// 指定されたユーザーIDのユーザーを削除します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>削除結果</returns>
        Task<HttpStatusCode> Delete(string systemId, string userId);

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        Task<(HttpStatusCode statusCode, OpenIdUserModel userInfo)> Get(string systemId, string userId);

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        Task<(HttpStatusCode statusCode, OpenIdUserModel userInfo)> GetFullAccess(string userId);

        /// <summary>
        /// 指定されたシステムで登録されたユーザーの一覧を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>ユーザーの一覧</returns>
        Task<IEnumerable<OpenIdUserModel>> GetList(string systemId);

        /// <summary>
        /// 指定されたユーザー情報でユーザーを登録します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="user">ユーザー情報</param>
        /// <returns>登録結果</returns>
        Task<(HttpStatusCode statusCode, OpenIdUserModel userInfo)> Register(string systemId, OpenIdUserModel user);
    }
}

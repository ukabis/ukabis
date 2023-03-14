using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.OpenIdUser
{
    /// <summary>
    /// OpenIdユーザーのオペレーション結果のステータス
    /// </summary>
    public enum OpenIdUserOperationStatus
    {
        None,
        Selected,
        Created,
        Updated,
        Deleted,
        NotFound,
        Forbidden
    }


    /// <summary>
    /// OpenIdユーザーのオペレーション結果
    /// </summary>
    internal class OpenIdUserOperationResult : IValueObject
    {
        /// <summary>結果ステータス</summary>
        public OpenIdUserOperationStatus Status { get; }

        /// <summary>ユーザー情報</summary>
        public OpenIdUser UserInfo { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="status">結果ステータス</param>
        /// <param name="userInfo">ユーザー情報</param>
        public OpenIdUserOperationResult(OpenIdUserOperationStatus status, OpenIdUser userInfo = null)
        {
            Status = status;
            UserInfo = userInfo;
        }
    }
}

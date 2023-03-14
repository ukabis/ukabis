using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.OpenIdUser
{
    /// <summary>
    /// OpenIdユーザーのEntity
    /// </summary>
    internal class OpenIdUser : IEntity
    {
        /// <summary>OpenIdプロバイダー中のオブジェクトID</summary>
        public ObjectId ObjectId { get; }

        /// <summary>ユーザーID(電子メールアドレス)</summary>
        public UserId UserId { get; }

        /// <summary>システムID</summary>
        public SystemId? SystemId { get; }

        /// <summary>パスワード</summary>
        public string Password { get; }

        /// <summary>表示名</summary>
        public string UserName { get; }

        /// <summary>作成日時</summary>
        public DateTime? CreatedDateTime { get; }


        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="objectId">OpenIdプロバイダー中のオブジェクトID</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="systemId">システムID</param>
        /// <param name="password">パスワード</param>
        /// <param name="userName">表示名</param>
        /// <param name="createdDateTime">作成日時</param>
        public OpenIdUser(string objectId, string userId, string systemId, string password, string userName, DateTime? createdDateTime)
        {
            ObjectId = new ObjectId(objectId);
            UserId = new UserId(userId);
            SystemId = string.IsNullOrEmpty(systemId) ? null : new SystemId(systemId);
            Password = password;
            UserName = userName;
            CreatedDateTime = createdDateTime;
        }
    }
}

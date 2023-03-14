using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.OpenIdUser;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    /// <summary>
    /// OpenIdユーザー管理のためのアプリケーションサービスです。
    /// </summary>
    [Log]
    class OpenIdUserApplicationService : IOpenIdUserApplicationService
    {
        private IOpenIdUserRepository Repository = UnityCore.Resolve<IOpenIdUserRepository>();


        /// <summary>
        /// 指定されたユーザーIDのユーザーを削除します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>削除結果</returns>
        public async Task<OpenIdUserOperationResult> Delete(SystemId systemId, UserId userId)
        {
            // 既存ユーザーを取得
            var currentUser = await Repository.Get(userId);

            // ユーザーが存在しない場合
            if (currentUser == null)
                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.NotFound);

            // 他システムのユーザーの場合
            if (!currentUser.SystemId.Equals(systemId))
                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Forbidden);

            // ユーザーを削除
            await Repository.Delete(currentUser.ObjectId);

            return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Deleted);
        }

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        public async Task<OpenIdUser> Get(UserId userId)
            => await Repository.Get(userId);

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        public async Task<OpenIdUserOperationResult> Get(SystemId systemId, UserId userId)
        {
            var currentUser = await Repository.Get(userId);
            if (currentUser == null || systemId == null || currentUser.SystemId == null)
            {
                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.NotFound);
            }

            if (!systemId.Equals(currentUser.SystemId))
            {
                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Forbidden);
            }

            return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Selected, currentUser);
        }

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        public async Task<OpenIdUserOperationResult> GetFullAccess(UserId userId)
        {
            var currentUser = await Repository.Get(userId);
            if (currentUser != null)
            {
                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Selected, currentUser);
            }
            else
            {
                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.NotFound);
            }
        }

        /// <summary>
        /// 指定されたシステムで登録されたユーザーの一覧を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>ユーザーの一覧</returns>
        public async Task<IEnumerable<OpenIdUser>> GetList(SystemId systemId)
            => await Repository.GetList(systemId);

        /// <summary>
        /// 指定されたユーザー情報でユーザーを登録します。
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>登録結果</returns>
        public async Task<OpenIdUserOperationResult> Register(OpenIdUser user)
        {
            // 既存ユーザーを取得
            var currentUser = await Repository.Get(user.UserId);

            // 既存ユーザーが存在する場合
            if (currentUser != null)
            {
                // 他システムのユーザーの場合
                if (!currentUser.SystemId.Equals(user.SystemId))
                    return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Forbidden);

                // ユーザーを更新
                await Repository.Update(currentUser.ObjectId, user);

                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Updated);
            }
            // ユーザーが存在しない場合は登録
            else
            {
                var registerdUser = await Repository.Register(user);

                return new OpenIdUserOperationResult(OpenIdUserOperationStatus.Created, registerdUser);
            }
        }
    }
}

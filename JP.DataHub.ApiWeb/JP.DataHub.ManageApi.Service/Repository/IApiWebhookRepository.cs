using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    /// <summary>
    /// Webhook管理のためのリポジトリのインターフェースです。
    /// </summary>
    interface IApiWebhookRepository
    {
        /// <summary>
        /// 指定されたApiWebhookIdのApiWebhookを取得します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        /// <returns>Webhook</returns>
        ApiWebhookModel Get(string apiWebhookId);

        /// <summary>
        /// 指定されたAPI ID、ベンダーIDのApiWebhookの存在確認をします。
        /// </summary>
        /// <param name="apiId">API ID</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>bool</returns>
        bool IsExists(string apiId, string vendorId);

        /// <summary>
        /// 指定されたベンダーIDのApiWebhook一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ApiWebhookId</param>
        /// <returns>Webhook一覧</returns>
        IList<ApiWebhookModel> GetList(string vendorId);

        /// <summary>
        /// Webhookを登録します。
        /// </summary>
        /// <param name="model">Webhook</param>
        void Register(ApiWebhookModel model);

        /// <summary>
        /// Webhookを更新します。
        /// </summary>
        /// <param name="model">Webhook</param>
        void Update(ApiWebhookModel model);

        /// <summary>
        /// Webhookを削除します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        void Delete(string apiWebhookId);
    }
}
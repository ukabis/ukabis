using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.Com.Transaction.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    interface IApiWebhookService
    {
        /// <summary>
        /// 指定されたApiWebhookIdのApiWebhookを取得します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        /// <returns>Webhook</returns>
        ApiWebhookModel Get(string apiWebhookId);

        /// <summary>
        /// Webhookを登録します。
        /// </summary>
        /// <param name="model">WebhookModel</param>
        /// <returns>登録結果</returns>
        [TransactionScope]
        ApiWebhookModel Register(ApiWebhookModel model);

        /// <summary>
        /// Webhookを更新します。
        /// </summary>
        /// <param name="model">WebhookModel</param>
        /// <returns>更新結果</returns>
        [TransactionScope]
        ApiWebhookModel Update(ApiWebhookModel model);

        /// <summary>
        /// 指定されたベンダーIDのApiWebhook一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>Webhook一覧</returns>
        IList<ApiWebhookModel> GetList(string vendorId);

        /// <summary>
        /// Webhookを削除します。
        /// </summary>
        /// <param name="apiWebhookId">ApiWebhookId</param>
        [TransactionScope]
        void Delete(string apiWebhookId);
    }
}

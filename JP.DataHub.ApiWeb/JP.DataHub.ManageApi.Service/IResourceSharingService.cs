using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Attributes;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    public interface IResourceSharingService
    {
        /// <summary>
        /// 共有設定ルールの取得
        /// </summary>
        /// <returns></returns>
        ResourceSharingRuleModel GetResourceSharingRule(string resourceSharingRuleId);

        /// <summary>
        /// 共有設定ルールリストの取得
        /// </summary>
        /// <returns></returns>
        IList<ResourceSharingRuleModel> GetResourceSharingRuleList(string controllerId, string sharingFromVendorId, string sharingFromSystemId);

        /// <summary>
        /// 共有設定新規作成
        /// </summary>
        /// <returns></returns>
        ResourceSharingRuleModel RegisterResourceSharingRule(ResourceSharingRuleModel resourceSharingRule);

        /// <summary>
        /// 共有設定更新
        /// </summary>
        /// <returns></returns>
        void UpdateResourceSharingRule(ResourceSharingRuleModel resourceSharingRule);

        /// <summary>
        /// 共有設定削除
        /// </summary>
        /// <returns></returns>
        void DeleteResourceSharingRule(string resourceSharingRuleId);
    }
}

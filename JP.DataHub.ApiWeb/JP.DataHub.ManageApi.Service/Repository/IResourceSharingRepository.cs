using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IResourceSharingRepository
    {
        ResourceSharingRuleModel GetResourceSharingRule(string resourceSharingRuleId);
        IList<ResourceSharingRuleModel> GetResourceSharingRuleList(string apiId, string resourceSharingFromVendorId, string resourceSharingFromSystemId);
        ResourceSharingRuleModel MargeResourceSharingRule(ResourceSharingRuleModel resourceSharing);
        void DeleteResourceSharingRule(string resourceSharingRule, string apiId);
    }
}

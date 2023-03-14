using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    interface ICommonIpFilterRepository
    {
        IList<CommonIpFilterGroupInfoModel> GetCommonIPFilter(List<string> FromUris);
        CommonIpFilterGroupModel GetCommonIpFilterGroup(string commonIpFilterGroupId);
        IList<CommonIpFilterGroupModel> GetCommonIpFilterGroups(IList<CommonIpFilterGroupModel> list);
        IList<CommonIpFilterGroupNameModel> GetCommonIpFilterGroupList();
        IList<CommonIpFilterModel> GetCommonIpFilterList(string commonIpFilterGroupId);
        CommonIpFilterGroupModel RegistrationCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup);
        CommonIpFilterGroupModel UpdateCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup);
        void DeleteCommonIpFilterGroup(string commonIpFilterGroupId);
    }
}

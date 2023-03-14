using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    public interface IAuthenticationService
    {
        IList<RoleDetailModel> GetRoleDetail();
        IList<RoleDetailModel> GetRoleDetailEx(string openId);

        IList<CommonIpFilterGroupInfoModel> GetCommonIPFilterList(List<string> commonIpFilterGroupNames);
        CommonIpFilterGroupModel RegistrationCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup);
        CommonIpFilterGroupModel UpdateCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup);
        void DeleteCommonIpFilterGroup(string commonIpFilterGroupId);
        CommonIpFilterGroupModel GetCommonIpFilterGroup(string commonIpFilterGroupId);
        IList<CommonIpFilterGroupNameModel> GetCommonIpFilterGroupList();
        IList<CommonIpFilterGroupModel> GetCommonIpFilterGroupListWithIpAddress();
    }
}
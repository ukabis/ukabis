using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Attributes;
using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    public interface IRepositoryGroupService
    {
        RepositoryGroupModel GetRepositoryGroup(string repositoryGroupId);

        IList<RepositoryGroupModel> GetRepositoryGroupList();

        IEnumerable<RepositoryGroupModel> GetRepositoryGroupListIncludePhysicalRepository(string vendorId = null);

        IEnumerable<RepositoryTypeModel> GetRepositoryGroupTypeList();

        RepositoryGroupModel MergeRepositoryGroup(RepositoryGroupModel repositoryGroup);

        void DeleteReposigoryGroup(string repositoryGroupId);

        void ActivateVendorRepositoryGroup(VendorRepositoryGroupModel vendorRepositoryGroupItem);
        
        void ActivateVendorRepositoryGroupList(IList<VendorRepositoryGroupModel> model);

        VendorRepositoryGroupModel GetVendorRepositoryGroup(string vendorRepositoryGroupId, string vendorId);

        IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupList();

        IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupList(string vendorId);

        bool CheckUsedVendorRepositoryGroup(string repositoryGroupId, string vendorId);
    }
}

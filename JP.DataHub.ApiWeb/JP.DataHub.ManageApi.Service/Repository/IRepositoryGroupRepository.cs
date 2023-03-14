using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IRepositoryGroupRepository
    {
        RepositoryGroupModel GetRepositoryGroup(string repositoryGroupId);

        IList<RepositoryGroupModel> GetRepositoryGroupList(string vendorId = null);

        List<PhysicalRepositoryModel> GetPhysicalRepository(string repositoryGroupId);

        IList<RepositoryTypeModel> GetRepositoryTypeList();

        RepositoryGroupModel MergeRepositoryGroup(RepositoryGroupModel repositoryGroup);

        void DeleteReposigoryGroup(string repositoryGroupId);

        /// <summary>
        /// Rdbリポジトリか
        /// </summary>
        /// <param name="repositoryGroupId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        bool MatchRepositoryType(string repositoryGroupId, string[] rdbTypeCdList);

        void ActivateVendorRepositoryGroup(string vendorId, string repositoryGroupId, bool isActive);

        void ActivateVendorRepositoryGroupList(string vendorId, IList<string> repositoryGroupIds);

        VendorRepositoryGroupModel GetVendorRepositoryGroupInfo(string vendorId, string repositoryGroupId);

        IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupInfoExList();

        IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupInfoList(string vendorId);

        bool CheckUsedVendorRepositoryGroup(string repositoryGroupId, string vendorId);

        void RegistDefaultVendorRepositoryGroup(string vendorId);

        bool ExistsRepositoryGroup(string repositoryGroupId);
    }
}

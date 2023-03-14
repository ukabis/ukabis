using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.VendorRepositoryGroup;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/VendorRepositoryGroup", typeof(VendorRepositoryGroupModel))]
    public interface IVendorRepositoryGroupApi
    {
        [WebApi("GetRepositoryGroupListByVendor?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<VendorRepositoryGroupModel>> GetRepositoryGroupListByVendor(string vendorId);

        [WebApi("GetVendorRepositoryGroup?vendorId={vendorId}&repositoryGroupId={repositoryGroupId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorRepositoryGroupModel> GetVendorRepositoryGroup(string vendorId, string repositoryGroupId);

        [WebApi("GetVendorRepositoryGroupList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<VendorRepositoryGroupListModel>> GetVendorRepositoryGroupList(string vendorId = null);

        [WebApiPost("ActivateVendorRepositoryGroup")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ActivateVendorRepositoryGroupModel> ActivateVendorRepositoryGroup(ActivateVendorRepositoryGroupModel model);

        [WebApiPost("ActivateVendorRepositoryGroupList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ActivateVendorRepositoryGroupModel> ActivateVendorRepositoryGroupList(ActivateVendorRepositoryGroupModel model);

    }
}

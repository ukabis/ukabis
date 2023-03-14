using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;

namespace JP.DataHub.AdminWeb.WebAPI.Resources
{
    [WebApiResource("/Manage/VendorRepositoryGroup", typeof(VendorRepositoryGroupListModel))]
    public interface IVendorRepositoryGroupResource : ICommonResource<VendorRepositoryGroupListModel>
    {
        [WebApiGet("GetVendorRepositoryGroupList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<VendorRepositoryGroupListModel>> GetVendorRepositoryGroupList(string querystring = null);
    }

    [WebApiResource("/Manage/VendorRepositoryGroup", typeof(ActivateVendorRepositoryGroupModel))]
    public interface IActivateVendorRepositoryGroupResource : ICommonResource<ActivateVendorRepositoryGroupModel>
    {
        [WebApiPost("ActivateVendorRepositoryGroupList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ActivateVendorRepositoryGroupModel>> ActivateVendorRepositoryGroupList(List<ActivateVendorRepositoryGroupModel> requestModel, string querystring = null);
    }
}

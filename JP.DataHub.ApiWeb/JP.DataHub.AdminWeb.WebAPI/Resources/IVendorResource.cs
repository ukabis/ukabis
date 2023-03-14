using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Resources
{
    [WebApiResource("/Manage/Vendor", typeof(VendorModel))]
    public interface IVendorResource : ICommonResource<VendorModel>
    {
        [WebApiGet("GetVendorSimpleList?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<VendorSimpleModel>> GetVendorSimpleList(string querystring = null);

        [WebApiGet("GetVendor?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorModel> GetVendor(string vendorId);

        [WebApiGet("GetVendorByOpenId?openId={openId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorOnlyModel> GetVendorByOpenId(string openId);

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorOnlyModel> Register(VendorModel vendor);

        [WebApiPost("RegisterVendorAttachFile")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorOnlyModel> RegisterAttachFile(VendorAttachFileModel vendor);

        [WebApiDelete("DeleteAttachFile?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorOnlyModel> RemoveAttachFile(string vendorId);

        [WebApiPost("Update")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorOnlyModel> Update(VendorModel vendor);

        [WebApiGet("GetStaffList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<StaffModel>> GetStaffList(string vendorId);

        [WebApiPost("RegisterVendorLink")]
        [AutoGenerateReturnModel]
        WebApiRequestModel RegisterVendorLink(List<VendorLinkModel> vendorLinks);

        [WebApiDelete("DeleteVendorLink?vendorLinkId={vendorLinkId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel RemoveVendorLink(string vendorLinkId);

        [WebApiPost("AddStaff")]
        [AutoGenerateReturnModel]
        WebApiRequestModel AddStaff(StaffModel staff);

        [WebApiPost("RegisterVendorOpenIdCa")]
        [AutoGenerateReturnModel]
        WebApiRequestModel RegisterVendorOpenIdCa(List<VendorOpenIdCaModel> vendorOpenIdCaList);

        [WebApiGet("GetAttachFileList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AttachFileStorageModel> GetAttachFileList();

        [WebApiDelete("DeleteStaff?staffId={staffId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorModel> RemoveStaff(string staffId);

        [WebApiDelete("Delete?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorModel> RemoveVendor(string vendorId);

        [WebApiPost("AddInvitedUser")]
        [AutoGenerateReturnModel]
        WebApiRequestModel AddInvitedUser(AddInvitedUserModel model);

        [WebApiPost("SendInvitation")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SendInvitationModel> SendInvitation(SendInvitationModel model);
    }
}

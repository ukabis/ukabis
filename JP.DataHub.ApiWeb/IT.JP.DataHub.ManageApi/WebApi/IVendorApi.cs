using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.Vendor;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/Vendor", typeof(VendorModel))]
    public interface IVendorApi
    {
        [WebApi("GetVendor?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorModel> GetVendor(string vendorId);

        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<VendorModel>> GetList();

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterVendorResultModel> Register(RegisterVendorModel model);

        [WebApiPost("Update")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<UpdateVendorModel> Update(UpdateVendorModel model);

        [WebApiDelete("Delete?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string vendorId);

        [WebApiPost("GetVendorByOpenId?openId={openId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorModel> GetVendorByOpenId(string openId);


        [WebApiPost("AddStaff")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterStafforModel> AddStaff(RegisterStafforModel model);


        [WebApiPost("UpdateStaff")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<UpdateStaffModel> UpdateStaff(UpdateStaffModel model);


        [WebApiDelete("DeleteStaff?staffId={staffId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteStaff(string staffId);

        [WebApi("ExistsStaffAccount?account={account}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ExistsStaffResultModel> ExistsStaffAccount(string account);

        [WebApiPost("ExistsStaffAccount")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ExistsStaffResultModel> ExistsStaffAccount(ExistsStaffModel model);

        [WebApi("GetStaffList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<StaffModel>> GetStaffList(string vendorId);

        [WebApi("GetStaff?staffId={staffId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<StaffModel> GetStaff(string staffId);

        [WebApiPost("RegisterVendorLink")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<VendorLinkModel>> RegisterVendorLink(List<RegisterVendorLinkModel> model);

        [WebApiDelete("DeleteVendorLink?vendorLinkId={vendorLinkId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteVendorLink(string vendorLinkId);

        [WebApi("GetVendorLinkList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<VendorLinkModel>> GetVendorLinkList(string vendorId);

        [WebApi("GetVendorLink?vendorLinkId={vendorLinkId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<VendorLinkModel> GetVendorLink(string vendorLinkId);

        [WebApi("GetVendorOpenIdCa?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<OpenIdCaModel>> GetVendorOpenIdCa(string vendorId);

        [WebApiPost("RegisterVendorOpenIdCa")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<OpenIdCaModel>> RegisterVendorOpenIdCa(List<RegisterOpenIdCaModel> model);

        [WebApiDelete("DeleteVendorOpenIdCa?vendorId={vendorId}&vendorOpenidCaId={vendorOpenidCaId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteVendorOpenIdCa(string vendorId, string vendorOpenidCaId);

        [WebApiPost("SendInvitation")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SendInvitationModel> SendInvitation(SendInvitationModel model);

        [WebApiPost("AddInvitedUser")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<SendInvitationModel> AddInvitedUser(AddInvitedUserModel model);

        [WebApi("GetAttachFileList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AttachFileSettingsModel>> GetAttachFileList();

        [WebApi("GetAttachFile?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GetAttachFileSettingsResponseModel> GetAttachFile(string vendorId);

        [WebApiPost("RegisterAttachFile?vendorId={vendorId}&attachFileId={attachFileId}&isCurrent={isCurrent}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterAttachFileSettingsResponseModel> RegisterAttachFile(string vendorId, string attachFileId, bool isCurrent = true);

        [WebApiDelete("DeleteAttachFile?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteAttachFile(string vendorId);
    }
}

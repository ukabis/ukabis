using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal enum OpenIdType
    {
        Vendor = 1,
        Controller,
        Api
    }

    internal interface IVendorRepository
    {
        IList<VendorModel> GetVendorSystemNameList(bool includeDelete = false, bool enableOnly = false, bool isGetStaffList = false, string vendorId = null);
        VendorModel Register(VendorModel model);
        VendorModel Update(VendorModel model);
        void Delete(string vendorId);
        VendorModel Get(string vendorId);
        bool IsExists(string vendorId);
        VendorModel GetByOpenId(string openId);
        IList<StaffRoleModel> GetStaffList(string vendorId);
        IList<OpenIdCaModel> GetVendorOpenIdCaListByVendorId(string vendorId);
        IList<OpenIdCaModel> GetVendorOpenIdCaListByVendorIdList(IList<string> vendorId);
        IList<OpenIdCaModel> GetControllerOpenIdCaListByVendorId(string controllerId);
        IList<OpenIdCaModel> GetApiOpenIdCaListByVendorId(string apiId);
        IList<OpenIdCaModel> GetOpenIdCaListByVendorId(IList<string> vendorId, OpenIdType type);

        StaffModel AddStaff(string vendor_id, string account, string emailaddress, string? staffId = null);
        StaffModel UpdateStaff(StaffModel model);
        string GetVendorIdByStaffAccount(string account);
        StaffModel GetStaff(string staff_id);
        bool IsExistsStaff(string staff_id);
        bool IsExistsStaffByAccount(string account);
        void DeleteStaff(string staff_id);
        StaffModel GetStaffByAccount(string account, bool isActive = true);
        IList<StaffModel> GetStaffListByVendorId(string vendor_id);
        string AddStaffRole(string staff_id, string role_id, string? staffRoleId = null);
        void DeleteStaffRole(string staffRoleId);

        IList<VendorLinkModel> RegisterVendorLink(IList<RegisterVendorLinkModel> model);
        void DeleteVendorLink(string vendorLinkId);
        IList<VendorLinkModel> GetVendorLinkList(string vendorId);
        VendorLinkModel GetVendorLink(string vendorLinkId);

        IList<RegisterOpenIdCaModel> RegisterVendorOpenIdCaList(IList<RegisterOpenIdCaModel> model);
        void DeleteVendorOpenIdCa(string vendorId, string vendorOpenidCaId);
    }
}

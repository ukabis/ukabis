using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    internal interface IVendorService
    {
        IList<VendorModel> GetEnableVendorSystemNameList(string vendorId = null);
        IList<VendorModel> GetVendorList();
        VendorModel Register(VendorModel model);
        VendorModel Update(VendorModel model);
        void Delete(string vendorId);
        VendorModel Get(string vendorId);
        VendorModel GetByOpenId(string vendorId);

        StaffModel AddStaff(StaffModel model);
        StaffModel UpdateStaff(StaffModel model);
        string GetVendorIdByStaffAccount(string account);
        void DeleteStaff(string staff_id);
        StaffModel GetStaff(string staff_id);
        IList<StaffModel> GetStaffListByVendorId(string vendor_id);

        IList<VendorLinkModel> RegisterVendorLink(IList<RegisterVendorLinkModel> model);
        void DeleteVendorLink(string vendorLinkId);
        IList<VendorLinkModel> GetVendorLinkList(string vendorId);
        VendorLinkModel GetVendorLink(string vendorLinkId);

        IList<VendorOpenIdCaModel> GetVendorOpenIdCaList(string vendorId);
        IList<RegisterOpenIdCaModel> RegisterVendorOpenIdCaList(IList<RegisterOpenIdCaModel> model);
        void DeleteVendorOpenIdCa(string vendorId, string vendorOpenidCaId);
    }
}

using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Service.Interface
{
    public interface IAdminVendorService : ICommonCrudService
    {
        #region Vendor
        WebApiResponseResult<VendorModel> GetVendor<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<VendorModel>> GetVendorAsync<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector;

        WebApiResponseResult<VendorOnlyModel> GetVendorByOpenId<TSelector>(string openId) where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<VendorOnlyModel>> GetVendorByOpenIdAsync<TSelector>(string openId) where TSelector : IDynamicApiClientSelector;

        WebApiResponseResult<List<VendorSimpleModel>> GetVendorSimpleList(string querystring = null);
        Task<WebApiResponseResult<List<VendorSimpleModel>>> GetVendorSimpleListAsync(string querystring = null);

        WebApiResponseResult<List<StaffModel>> GetStaffList<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<StaffModel>>> GetStaffListAsync<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector;

        WebApiResponseResult RegisterVendor(VendorModel vendor);
        Task<WebApiResponseResult> RegisterVendorAsync(VendorModel vendor);

        WebApiResponseResult RegisterVendorAttachFile(VendorAttachFileModel vendor);
        Task<WebApiResponseResult> RegisterVendorAttachFileAsync(VendorAttachFileModel vendor);

        WebApiResponseResult RemoveVendorAttachFile(string vendorId);
        Task<WebApiResponseResult> RemoveVendorAttachFileAsync(string vendorId);

        WebApiResponseResult UpdateVendor(VendorModel vendor);
        Task<WebApiResponseResult> UpdateVendorAsync(VendorModel vendor);

        WebApiResponseResult RegisterVendorLink(List<object> vendorLinks);
        Task<WebApiResponseResult> RegisterVendorLinkAsync(List<object> vendorLinks);

        WebApiResponseResult RemoveVendorLink(string vendorLinkId);
        Task<WebApiResponseResult> RemoveVendorLinkAsync(string vendorLinkId);

        WebApiResponseResult AddStaff(StaffModel staff);
        Task<WebApiResponseResult> AddStaffAsync(StaffModel staff);

        WebApiResponseResult RegisterVendorOpenIdCa(List<object> vendorOpenIdCaList);
        Task<WebApiResponseResult> RegisterVendorOpenIdCaAsync(List<object> vendorOpenIdCaList);

        WebApiResponseResult RemoveStaff(string staffId);
        Task<WebApiResponseResult> RemoveStaffAsync(string staffId);

        WebApiResponseResult RemoveVendor(string vendorId);
        Task<WebApiResponseResult> RemoveVendorAsync(string vendorId);

        #endregion

        #region System
        WebApiResponseResult<SystemModel> GetSystem(Guid systemId);
        Task<WebApiResponseResult<SystemModel>> GetSystemAsync(Guid systemId);

        WebApiResponseResult<List<ClientModel>> GetClientList(Guid systemId);
        Task<WebApiResponseResult<List<ClientModel>>> GetClientListAsync(Guid systemId);

        WebApiResponseResult<List<SystemLinkModel>> GetSystemLinkList(Guid systemId);
        Task<WebApiResponseResult<List<SystemLinkModel>>> GetSystemLinkListAsync(Guid systemId);

        WebApiResponseResult<SystemAdminModel> GetSystemAdmin(Guid systemId);
        Task<WebApiResponseResult<SystemAdminModel>> GetSystemAdminAsync(Guid systemId);

        WebApiResponseResult UpdateSystem(SystemModel model);
        Task<WebApiResponseResult> UpdateSystemAsync(SystemModel model);

        WebApiResponseResult UpdateClient(ClientModel model);
        Task<WebApiResponseResult> UpdateClientAsync(ClientModel model);

        WebApiResponseResult RegisterSystemLink(List<object> model);
        Task<WebApiResponseResult> RegisterSystemLinkAsync(List<object> model);

        WebApiResponseResult RegisterSystemAdmin(SystemAdminModel model);
        Task<WebApiResponseResult> RegisterSystemAdminAsync(SystemAdminModel model);

        WebApiResponseResult RegisterSystem(SystemModel model);
        Task<WebApiResponseResult> RegisterSystemAsync(SystemModel model);

        WebApiResponseResult RegisterClient(ClientModel model);
        Task<WebApiResponseResult> RegisterClientAsync(ClientModel model);


        WebApiResponseResult DeleteSystem(string systemId);
        Task<WebApiResponseResult> DeleteSystemAsync(string systemId);

        WebApiResponseResult DeleteClient(string clientId);
        Task<WebApiResponseResult> DeleteClientAsync(string clientId);

        WebApiResponseResult DeleteSystemLink(string systemLinkId);
        Task<WebApiResponseResult> DeleteSystemLinkAsync(string systemLinkId);

        WebApiResponseResult DeleteSystemAdmin(string systemId);
        Task<WebApiResponseResult> DeleteSystemAdminAsync(string systemId);
        #endregion

        #region Role
        WebApiResponseResult<List<RoleModel>> GetRoleList();
        Task<WebApiResponseResult<List<RoleModel>>> GetRoleListAsync();
        #endregion

        #region AttachFileStorage
        WebApiResponseResult<List<AttachFileStorageModel>> GetAttachFileStorageList<TSelector>() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<AttachFileStorageModel>>> GetAttachFileStorageListAsync<TSelector>() where TSelector : IDynamicApiClientSelector;
        #endregion
    }
}

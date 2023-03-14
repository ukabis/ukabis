using JP.DataHub.AdminWeb.Service.Interface;
using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Resources;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Service.Core.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Service.Impl
{
    public class AdminVendorService : CommonCrudService, IAdminVendorService
    {
        #region Vendor
        public WebApiResponseResult<VendorModel> GetVendor<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector
            => BaseRepository.Access<VendorModel>(GetDaoS<TSelector, IVendorResource, VendorModel>("GetVendor"), vendorId);
        public Task<WebApiResponseResult<VendorModel>> GetVendorAsync<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector
            => Task.Run(() => GetVendor<TSelector>(vendorId));

        public WebApiResponseResult<VendorOnlyModel> GetVendorByOpenId<TSelector>(string openId) where TSelector : IDynamicApiClientSelector
            => BaseRepository.Access<VendorOnlyModel>(GetDaoS<TSelector, IVendorResource, VendorOnlyModel>("GetVendorByOpenId"), openId);
        public Task<WebApiResponseResult<VendorOnlyModel>> GetVendorByOpenIdAsync<TSelector>(string openId) where TSelector : IDynamicApiClientSelector
            => Task.Run(() => GetVendorByOpenId<TSelector>(openId));

        public WebApiResponseResult<List<VendorSimpleModel>> GetVendorSimpleList(string querystring = null)
            => BaseRepository.GetList<VendorSimpleModel>(GetDaoS<ILoginUser, IVendorResource>(), querystring);
        public Task<WebApiResponseResult<List<VendorSimpleModel>>> GetVendorSimpleListAsync(string querystring = null)
            => Task.Run(() => GetVendorSimpleList(querystring));

        public WebApiResponseResult<List<StaffModel>> GetStaffList<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector
            => BaseRepository.Access<List<StaffModel>>(GetDaoS<TSelector, IVendorResource, List<StaffModel>>("GetStaffList"), vendorId);
        public Task<WebApiResponseResult<List<StaffModel>>> GetStaffListAsync<TSelector>(string vendorId) where TSelector : IDynamicApiClientSelector
            => Task.Run(() => GetStaffList<TSelector>(vendorId));

        public WebApiResponseResult RegisterVendor(VendorModel vendor)
            => BaseRepository.Register(GetDaoS<ILoginUser, IVendorResource>("Register"), vendor);
        public Task<WebApiResponseResult> RegisterVendorAsync(VendorModel vendor)
            => Task.Run(() => RegisterVendor(vendor));

        public WebApiResponseResult RegisterVendorAttachFile(VendorAttachFileModel vendor)
            => BaseRepository.Register(GetDaoS<ILoginUser, IVendorResource, VendorAttachFileModel>("RegisterAttachFile"), vendor);
        public Task<WebApiResponseResult> RegisterVendorAttachFileAsync(VendorAttachFileModel vendor)
            => Task.Run(() => RegisterVendorAttachFile(vendor));

        public WebApiResponseResult RemoveVendorAttachFile(string vendorId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, IVendorResource>("RemoveAttachFile"), vendorId);
        public Task<WebApiResponseResult> RemoveVendorAttachFileAsync(string vendorId)
            => Task.Run(() => RemoveVendorAttachFile(vendorId));

        public WebApiResponseResult UpdateVendor(VendorModel vendor)
            => BaseRepository.Update(GetDaoS<ILoginUser, IVendorResource>("Update"), vendor);
        public Task<WebApiResponseResult> UpdateVendorAsync(VendorModel vendor)
            => Task.Run(() => UpdateVendor(vendor));

        public WebApiResponseResult RegisterVendorLink(List<object> vendorLinks)
            => BaseRepository.RegisterList(GetDaoS<ILoginUser, IVendorResource, VendorLinkModel>(), vendorLinks);
        public Task<WebApiResponseResult> RegisterVendorLinkAsync(List<object> vendorLinks)
            => Task.Run(() => RegisterVendorLink(vendorLinks));

        public WebApiResponseResult RemoveVendorLink(string vendorLinkId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, IVendorResource>("RemoveVendorLink"), vendorLinkId);
        public Task<WebApiResponseResult> RemoveVendorLinkAsync(string vendorLinkId)
            => Task.Run(() => RemoveVendorLink(vendorLinkId));

        public WebApiResponseResult AddStaff(StaffModel staff)
            => BaseRepository.Register(GetDaoS<ILoginUser, IVendorResource, StaffModel>(), staff);
        public Task<WebApiResponseResult> AddStaffAsync(StaffModel staff)
            => Task.Run(() => AddStaff(staff));

        public WebApiResponseResult RegisterVendorOpenIdCa(List<object> vendorOpenIdCaList)
            => BaseRepository.RegisterList(GetDaoS<ILoginUser, IVendorResource, VendorOpenIdCaModel>(), vendorOpenIdCaList);
        public Task<WebApiResponseResult> RegisterVendorOpenIdCaAsync(List<object> vendorOpenIdCaList)
            => Task.Run(() => RegisterVendorOpenIdCa(vendorOpenIdCaList));

        public WebApiResponseResult RemoveStaff(string staffId)
           => BaseRepository.Delete(GetDaoS<ILoginUser, IVendorResource>("RemoveStaff"), staffId);
        public Task<WebApiResponseResult> RemoveStaffAsync(string staffId)
            => Task.Run(() => RemoveStaff(staffId));

        public WebApiResponseResult RemoveVendor(string vendorId)
           => BaseRepository.Delete(GetDaoS<ILoginUser, IVendorResource>("RemoveVendor"), vendorId);
        public Task<WebApiResponseResult> RemoveVendorAsync(string vendorId)
            => Task.Run(() => RemoveVendor(vendorId));

        #endregion

        #region System
        // 取得
        public WebApiResponseResult<SystemModel> GetSystem(Guid systemId)
            => BaseRepository.Access<SystemModel> (GetDaoS<ILoginUser, ISystemResource, SystemModel>("GetSystem"), systemId);
        public Task<WebApiResponseResult<SystemModel>> GetSystemAsync(Guid systemId)
            => Task.Run(() => GetSystem(systemId));

        public WebApiResponseResult<List<ClientModel>> GetClientList(Guid systemId)
            => BaseRepository.Access<List<ClientModel>>(GetDaoS<ILoginUser, ISystemResource, List<ClientModel>>("GetClientList"), systemId);
        public Task<WebApiResponseResult<List<ClientModel>>> GetClientListAsync(Guid systemId)
            => Task.Run(() => GetClientList(systemId));

        public WebApiResponseResult<List<SystemLinkModel>> GetSystemLinkList(Guid systemId)
            => BaseRepository.Access<List<SystemLinkModel>>(GetDaoS<ILoginUser, ISystemResource, List<SystemLinkModel>>("GetSystemLinkList"), systemId);
        public Task<WebApiResponseResult<List<SystemLinkModel>>> GetSystemLinkListAsync(Guid systemId)
            => Task.Run(() => GetSystemLinkList(systemId));

        public WebApiResponseResult<SystemAdminModel> GetSystemAdmin(Guid systemId)
            => BaseRepository.Access<SystemAdminModel>(GetDaoS<ILoginUser, ISystemResource, SystemAdminModel>("GetSystemAdmin"), systemId);
        public Task<WebApiResponseResult<SystemAdminModel>> GetSystemAdminAsync(Guid systemId)
            => Task.Run(() => GetSystemAdmin(systemId));

        // 登録・更新
        public WebApiResponseResult UpdateSystem(SystemModel model)
            => BaseRepository.Update(GetDaoS<ILoginUser, ISystemResource, SystemModel>("UpdateSystem"), model);
        public Task<WebApiResponseResult> UpdateSystemAsync(SystemModel model)
            => Task.Run(() => UpdateSystem(model));

        public WebApiResponseResult UpdateClient(ClientModel model)
            => BaseRepository.Update(GetDaoS<ILoginUser, ISystemResource, ClientModel>("UpdateClient"), model);
        public Task<WebApiResponseResult> UpdateClientAsync(ClientModel model)
            => Task.Run(() => UpdateClient(model));

        public WebApiResponseResult RegisterSystemLink(List<object> model)
            => BaseRepository.RegisterList(GetDaoS<ILoginUser, ISystemResource, SystemLinkModel>(), model);
        public Task<WebApiResponseResult> RegisterSystemLinkAsync(List<object> model)
            => Task.Run(() => RegisterSystemLink(model));

        public WebApiResponseResult RegisterSystemAdmin(SystemAdminModel model)
            => BaseRepository.Register(GetDaoS<ILoginUser, ISystemResource, SystemAdminModel>("RegisterSystemAdmin"), model);
        public Task<WebApiResponseResult> RegisterSystemAdminAsync(SystemAdminModel model)
            => Task.Run(() => RegisterSystemAdmin(model));

        public WebApiResponseResult RegisterSystem(SystemModel model)
            => BaseRepository.Register(GetDaoS<ILoginUser, ISystemResource, SystemModel>("RegisterSystem"), model);
        public Task<WebApiResponseResult> RegisterSystemAsync(SystemModel model)
            => Task.Run(() => RegisterSystem(model));

        public WebApiResponseResult RegisterClient(ClientModel model)
            => BaseRepository.Register(GetDaoS<ILoginUser, ISystemResource, ClientModel>("RegisterClient"), model);
        public Task<WebApiResponseResult> RegisterClientAsync(ClientModel model)
            => Task.Run(() => RegisterClient(model));

        // 削除
        public WebApiResponseResult DeleteSystem(string systemId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, ISystemResource>("DeleteSystem"), systemId);
        public Task<WebApiResponseResult> DeleteSystemAsync(string systemId)
            => Task.Run(() => DeleteSystem(systemId));

        public WebApiResponseResult DeleteClient(string clientId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, ISystemResource>("DeleteClient"), clientId);
        public Task<WebApiResponseResult> DeleteClientAsync(string clientId)
            => Task.Run(() => DeleteClient(clientId));

        public WebApiResponseResult DeleteSystemLink(string systemLinkId)
            => BaseRepository.Delete(GetDaoS<ILoginUser, ISystemResource>("DeleteSystemLink"), systemLinkId);
        public Task<WebApiResponseResult> DeleteSystemLinkAsync(string systemLinkId)
            => Task.Run(() => DeleteSystemLink(systemLinkId));

        public WebApiResponseResult DeleteSystemAdmin(string systemId)
        => BaseRepository.Delete(GetDaoS<ILoginUser, ISystemResource>("DeleteSystemAdmin"), systemId);
        public Task<WebApiResponseResult> DeleteSystemAdminAsync(string systemId)
            => Task.Run(() => DeleteSystemAdmin(systemId));

        #endregion

        #region Role
        public WebApiResponseResult<List<RoleModel>> GetRoleList()
            => BaseRepository.Access<List<RoleModel>>(GetDaoS<ILoginUser, IRoleResource, List<RoleModel>>("GetRoleList"));
        public Task<WebApiResponseResult<List<RoleModel>>> GetRoleListAsync()
            => Task.Run(() => GetRoleList());
        #endregion

        #region AttachFile
        public WebApiResponseResult<List<AttachFileStorageModel>> GetAttachFileStorageList<TSelector>() where TSelector : IDynamicApiClientSelector
            => BaseRepository.Access<List<AttachFileStorageModel>>(GetDaoS<TSelector, IVendorResource, List<AttachFileStorageModel>>("GetAttachFileList"));
        public Task<WebApiResponseResult<List<AttachFileStorageModel>>> GetAttachFileStorageListAsync<TSelector>() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => GetAttachFileStorageList<TSelector>());
        #endregion
    }
}

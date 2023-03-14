using JP.DataHub.AdminWeb.Service.Interface;
using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;
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
    public class CommonAdminService : CommonCrudService, ICommonAdminService
    {
        #region DynamicApi
        public WebApiResponseResult<List<OpenIdCaModel>> GetOpenIdCaList()
            => BaseRepository.Access<List<OpenIdCaModel>>(GetDaoS<ILoginUser, IApiResource, List<OpenIdCaModel>>("GetOpenIdCaList"));
        public Task<WebApiResponseResult<List<OpenIdCaModel>>> GetOpenIdCaListAsync()
            => Task.Run(() => GetOpenIdCaList());
        #endregion

        #region Authority
        public WebApiResponseResult<List<RoleDetailModel>> GetRoleDetail<TSelector>() where TSelector : IDynamicApiClientSelector
            => BaseRepository.Access<List<RoleDetailModel>>(GetDaoS<TSelector, IAuthorityResource, List<RoleDetailModel>>("GetRoleDetail"));
        public Task<WebApiResponseResult<List<RoleDetailModel>>> GetRoleDetailAsync<TSelector>() where TSelector : IDynamicApiClientSelector
            => Task.Run(() => GetRoleDetail<TSelector>());

        public WebApiResponseResult<List<RoleDetailModel>> GetRoleDetailEx<TSelector>(string openId) where TSelector : IDynamicApiClientSelector
            => BaseRepository.Access<List<RoleDetailModel>>(GetDaoS<TSelector, IAuthorityResource, List<RoleDetailModel>>(), openId);
        public Task<WebApiResponseResult<List<RoleDetailModel>>> GetRoleDetailExAsync<TSelector>(string openId) where TSelector : IDynamicApiClientSelector
            => Task.Run(() => GetRoleDetailEx<TSelector>(openId));

        public WebApiResponseResult<bool> IsOperatingVendor(string vendorId)
            => BaseRepository.Get<bool>(GetDaoS<ILoginUser, IAuthorityResource, bool>(), vendorId);
        public Task<WebApiResponseResult<bool>> IsOperatingVendorAsync(string vendorId)
            => Task.Run(() => IsOperatingVendor(vendorId));
        #endregion
    }
} 
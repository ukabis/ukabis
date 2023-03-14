using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Service.Interface
{
    public interface ICommonAdminService : ICommonCrudService
    {
        #region DynamicApi
        WebApiResponseResult<List<OpenIdCaModel>> GetOpenIdCaList();
        Task<WebApiResponseResult<List<OpenIdCaModel>>> GetOpenIdCaListAsync();
        #endregion

        #region Authority
        WebApiResponseResult<List<RoleDetailModel>> GetRoleDetail<TSelector>() where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<RoleDetailModel>>> GetRoleDetailAsync<TSelector>() where TSelector : IDynamicApiClientSelector;

        WebApiResponseResult<List<RoleDetailModel>> GetRoleDetailEx<TSelector>(string openId) where TSelector : IDynamicApiClientSelector;
        Task<WebApiResponseResult<List<RoleDetailModel>>> GetRoleDetailExAsync<TSelector>(string openId) where TSelector : IDynamicApiClientSelector;

        WebApiResponseResult<bool> IsOperatingVendor(string vendorId);
        Task<WebApiResponseResult<bool>> IsOperatingVendorAsync(string vendorId);
        #endregion
    }
}

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
    public interface IAdminRepositoryGroupService : ICommonCrudService
    {
        #region RepositoryGroup
        WebApiResponseResult<RepositoryGroupModel> GetRepositoryGroup(string repositoryGroupId);
        Task<WebApiResponseResult<RepositoryGroupModel>> GetRepositoryGroupAsync(string repositoryGroupId);
        WebApiResponseResult<List<RepositoryGroupModel>> GetRepositoryGroupList(string queryString = null);
        Task<WebApiResponseResult<List<RepositoryGroupModel>>> GetRepositoryGroupListAsync(string queryString = null);
        WebApiResponseResult<List<RepositoryTypeModel>> GetRepositoryGroupTypeList(string queryString = null);
        Task<WebApiResponseResult<List<RepositoryTypeModel>>> GetRepositoryGroupTypeListAsync(string queryString = null);
        WebApiResponseResult DeleteRepositoryGroup(string key, string queryString = null);
        WebApiResponseResult<RepositoryGroupModel> RegisterRepositoryGroup(object data, string queryString = null);
        #endregion

        #region VendorRepositoryGroup
        WebApiResponseResult<List<VendorRepositoryGroupListModel>> GetVendorRepositoryGroupList(string queryString = null);
        Task<WebApiResponseResult<List<VendorRepositoryGroupListModel>>> GetVendorRepositoryGroupListAsync(string queryString = null);
        WebApiResponseResult<List<ActivateVendorRepositoryGroupModel>> ActivateVendorRepositoryGroupList(object data, string querystring = null);
        Task<WebApiResponseResult<List<ActivateVendorRepositoryGroupModel>>> ActivateVendorRepositoryGroupListAsync(object data, string querystring = null);
        #endregion
    }
}
